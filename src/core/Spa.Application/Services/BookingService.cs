using AutoMapper;
using Spa.Application.DTOs.Bookings;
using Spa.Application.Interfaces;
using Spa.Application.Seedwork;
using Spa.Domain.Entities.Bookings;
using Spa.Domain.Enums;

namespace Spa.Application.Services;

public class BookingService : IBookingService
{
	private readonly IUnitOfWork _unitOfWork;
	private readonly IMapper _mapper;

	public BookingService(IUnitOfWork unitOfWork, IMapper mapper)
	{
		_unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
		_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
	}

	public async Task<bool> CheckAvailabilityAsync(DateTime startTime, int durationMinutes)
	{
		var endTime = startTime.AddMinutes(durationMinutes);

		// 1. Lấy cấu hình sức chứa tối đa từ DB (Ví dụ: 3 khách / 1 khung giờ)
		var maxCustomerSetting =
			await _unitOfWork.SystemSettings.GetFirstOrDefaultAsync(s =>
				s.Name == "MaxCustomerPerSlot" && s.Status == true);
		var maxCapacity =
			maxCustomerSetting != null ? int.Parse(maxCustomerSetting.Value) : 3; // Mặc định là 3 nếu chưa cấu hình

		// 2. Thuật toán Overlap: Đếm số lượng Booking ĐANG TỒN TẠI bị giao cắt với khoảng thời gian khách muốn đặt
		var overlappingBookings = await _unitOfWork.Bookings.FindAsync(b =>
				(b.Status == BookingStatus.Pending || b.Status == BookingStatus.Confirmed ||
				 b.Status == BookingStatus.InProgress) &&
				startTime < b.ScheduledEndTime && endTime > b.ScheduledStartTime // Công thức vàng check overlap
		);

		// Nếu số lượng khách đang phục vụ >= sức chứa -> Hết chỗ
		return overlappingBookings.Count() < maxCapacity;
	}

	public async Task<BookingResponseDto> CreateBookingAsync(CreateBookingRequestDto request)
	{
		// 1. Lấy thông tin Gói dịch vụ (Để lấy giá tiền và thời gian)
		var package = await _unitOfWork.ServicePackages.GetFirstOrDefaultAsync(
			p => p.Id == request.ServicePackageId,
			p => p.Service!); // Include Service để lấy tên hiển thị

		if (package == null) throw new Exception("Gói dịch vụ không tồn tại.");

		var endTime = request.ScheduledStartTime.AddMinutes(package.DurationMinutes);

		// 2. Kiểm tra tình trạng phòng / sức chứa
		var isAvailable = await CheckAvailabilityAsync(request.ScheduledStartTime, package.DurationMinutes);
		if (!isAvailable) throw new Exception("Khung giờ này đã kín lịch. Vui lòng chọn giờ khác.");

		// 3. Xử lý Mã giảm giá (Nếu có)
		decimal discountAmount = 0;
		int? validCouponId = null;

		if (!string.IsNullOrEmpty(request.CouponCode))
		{
			var coupon = await _unitOfWork.Coupons.GetFirstOrDefaultAsync(c =>
				c.Code == request.CouponCode &&
				c.IsActive &&
				c.StartDate <= DateTime.Now &&
				c.EndDate >= DateTime.Now &&
				c.CurrentUsage < c.MaxUsage);

			if (coupon != null)
			{
				validCouponId = coupon.Id;
				if (coupon.Type == DiscountType.FixedAmount)
				{
					discountAmount = coupon.DiscountValue;
				}
				else if (coupon.Type == DiscountType.Percentage)
				{
					discountAmount = package.Price * (coupon.DiscountValue / 100);
					if (coupon.MaxDiscountAmount.HasValue && discountAmount > coupon.MaxDiscountAmount.Value)
						discountAmount = coupon.MaxDiscountAmount.Value;
				}

				// Tăng số lần sử dụng của Coupon lên 1
				coupon.CurrentUsage += 1;
				_unitOfWork.Coupons.Update(coupon);
			}
		}

		// 4. Tính toán số tiền cuối cùng
		var finalPrice = package.Price - discountAmount;
		if (finalPrice < 0) finalPrice = 0;

		// 🟢 THÊM LOGIC TÍNH DEPOSIT (CỌC 20%) Ở ĐÂY
		decimal depositAmount = 0;
		if (request.PaymentMethod == PaymentMethod.Stripe)
		{
			// Cọc 20% trên tổng tiền phải trả (sau khi đã trừ discount)
			depositAmount = Math.Round(finalPrice * 0.20m, 2); 
		}

		// 5. Tạo Entity Booking
		var booking = _mapper.Map<Booking>(request);
		// Ghi đè các thông tin nghiệp vụ mà Frontend không được phép tự quyết định
		booking.CustomerName = request.CustomerName ?? "Khách vãng lai";
		booking.CustomerPhone = request.CustomerPhone ?? "";
		booking.CouponId = validCouponId;
		booking.TotalPrice = package.Price;
		booking.DiscountAmount = discountAmount;
		booking.RemainingAmount = finalPrice - depositAmount;
		booking.DepositAmount = depositAmount;
		booking.ScheduledEndTime = endTime;
		booking.DurationMinutes = package.DurationMinutes;
		booking.Status = BookingStatus.Pending;

		// 6. Lưu vào Database (Sử dụng Transaction để đảm bảo an toàn)
		await _unitOfWork.BeginTransactionAsync();
		try
		{
			await _unitOfWork.Bookings.AddAsync(booking);
			await _unitOfWork.SaveChangesAsync(); // Lưu để EF Core sinh ra booking.Id

			await _unitOfWork.CommitTransactionAsync();
		}
		catch (Exception)
		{
			await _unitOfWork.RollbackTransactionAsync();
			throw new Exception("Có lỗi xảy ra trong quá trình đặt lịch. Vui lòng thử lại.");
		}

		// 7. Map sang DTO trả về cho Frontend
		return _mapper.Map<BookingResponseDto>(booking);
	}

	public async Task<PagedResult<BookingResponseDto>?> GetAllBookingPagingAsync(BookingPagingRequest request)
	{
		var result = await _unitOfWork.Bookings.GetPagedBookingsAsync(request);

		return new PagedResult<BookingResponseDto>
		{
			PageIndex = result.PageIndex,
			PageSize = result.PageSize,
			TotalItems = result.TotalItems,
			Items = _mapper.Map<List<BookingResponseDto>>(result.Items)
		};
	}

	public async Task<BookingResponseDto?> GetBookingByIdAsync(int bookingId)
	{
		var booking = await _unitOfWork.Bookings.GetFirstOrDefaultAsync(
			b => b.Id == bookingId,
			b => b.ServicePackage!.Service!);

		if (booking == null) return null;

		return _mapper.Map<BookingResponseDto>(booking);
	}

	public async Task<IEnumerable<BookingResponseDto>> GetCustomerBookingsAsync(Guid userId)
	{
		var bookings = await _unitOfWork.Bookings.FindAsync(
			b => b.UserId == userId,
			b => b.ServicePackage!.Service!);

		var result = _mapper.Map<List<BookingResponseDto>>(bookings.OrderByDescending(b => b.CreatedBy));

		return result;
	}

	public async Task<bool> UpdateBookingStatusAsync(int bookingId, BookingStatus newStatus)
	{
		var booking = await _unitOfWork.Bookings.GetByIdAsync(bookingId);
		if (booking == null) return false;

		booking.Status = newStatus;
		_unitOfWork.Bookings.Update(booking);
		await _unitOfWork.SaveChangesAsync();

		return true;
	}

	public async Task<List<TimeSlotDto>> GetAvailableTimeSlotsAsync(DateTime date, int durationMinutes)
	{
		var slots = new List<TimeSlotDto>();
		// Giả sử Spa mở cửa từ 9h sáng đến 8h tối (21h)
		var openTime = date.Date.AddHours(8); 
		var closeTime = date.Date.AddHours(22);
    
		var currentTimeSlot = openTime;

		// Quét từng khung giờ (mỗi slot cách nhau 30 phút)
		while (currentTimeSlot.AddMinutes(durationMinutes) <= closeTime)
		{
			// Tái sử dụng hàm check Overlap siêu xịn của bác
			var isAvailable = await CheckAvailabilityAsync(currentTimeSlot, durationMinutes);
        
			slots.Add(new TimeSlotDto
			{
				Time = currentTimeSlot.ToString("HH:mm"),
				Available = isAvailable // True nếu còn chỗ, False nếu đã đầy
			});

			currentTimeSlot = currentTimeSlot.AddMinutes(30); // Bước nhảy 30 phút
		}

		return slots;
	}
}