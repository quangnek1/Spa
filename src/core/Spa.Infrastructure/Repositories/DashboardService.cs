using Microsoft.EntityFrameworkCore;
using Spa.Application.DTOs.Admin;
using Spa.Application.Interfaces;
using Spa.Application.Seedwork;
using Spa.Domain.Enums;

namespace Spa.Infrastructure.Repositories;

public class DashboardService: IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;

    public DashboardService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }
    
    public async Task<DashboardStatsDto> GetAdminDashboardStatsAsync()
    {
        var today = DateTime.Today;

        // Bác lấy IQueryable ra (không track changes để truy vấn cực nhanh)
        var bookingsQuery = _unitOfWork.Bookings.GetAll(false);

        // 1. Tính toán các con số tổng
        var totalBookings = await bookingsQuery.CountAsync();
        
        var bookingsToday = await bookingsQuery
            .CountAsync(b => b.ScheduledStartTime.Date == today);
            
        var upcomingBookings = await bookingsQuery
            .CountAsync(b => b.ScheduledStartTime >= DateTime.Now && b.Status == BookingStatus.Confirmed);

        var totalRevenue = await bookingsQuery
            .Where(b => b.DepositAmount > 0)
            .SumAsync(b => b.DepositAmount);

        // 2. Lấy 5 booking mới nhất
        var recentBookings = await bookingsQuery
            .Include(b => b.ServicePackage)
            .ThenInclude(p => p.Service)
            .OrderByDescending(b => b.CreatedDate)
            .Take(5)
            .Select(b => new RecentBookingDto
            {
                Id = b.Id,
                CustomerName = b.CustomerName ?? "Unknown",
                ServiceName = b.ServicePackage!.Service!.Name,
                ScheduledStartTime = b.ScheduledStartTime,
                Status = b.Status.ToString()
            })
            .ToListAsync();

        return new DashboardStatsDto
        {
            TotalBookings = totalBookings,
            BookingsToday = bookingsToday,
            UpcomingBookings = upcomingBookings,
            TotalRevenue = totalRevenue,
            RecentBookings = recentBookings
        };
    }
}