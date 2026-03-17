using AutoMapper;
using Spa.Application.DTOs;
using Spa.Application.DTOs.Services;
using Spa.Application.Interfaces;
using Spa.Application.Seedwork;
using Spa.Domain.Entities.Services;

namespace Spa.Application.Services;

public class SpaManagerService : ISpaManagerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public SpaManagerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<IEnumerable<ServiceDto>> GetAllServicesWithPackagesAsync()
    {
        // Nhờ hàm FindAsync nâng cấp, ta có thể Include bảng Packages cực kỳ dễ dàng
        var services = await _unitOfWork.Services.FindAsync(
            s => s.Status == true,
            s => s.Packages!);

        return services.Select(s => new ServiceDto
        {
            Id = s.Id,
            Name = s.Name,
            Slug = s.Slug!,
            Image = s.Image,
            ShortDescription = s.ShortDescription,
            Packages = s.Packages!.Select(p => new ServicePackageDto
            {
                Id = p.Id,
                DurationMinutes = p.DurationMinutes,
                Price = p.Price
            }).ToList()
        });
    }

    public async Task<ServiceDto?> GetServiceBySlugAsync(string slug)
    {
        var service = await _unitOfWork.Services.GetFirstOrDefaultAsync(
            s => s.Slug == slug && s.Status == true,
            s => s.Packages!); // Có thể phẩy thêm s => s.Images, s => s.Reviews

        if (service == null) return null;

        return new ServiceDto
        {
            Id = service.Id,
            Name = service.Name,
            Slug = service.Slug!,
            Image = service.Image,
            Description = service.Description, // Trang chi tiết lấy full description
            Packages = service.Packages!.Select(p => new ServicePackageDto
            {
                Id = p.Id,
                DurationMinutes = p.DurationMinutes,
                Price = p.Price
            }).ToList()
        };
    }

    public async Task<ServiceDto> CreateServiceAsync(CreateServiceDto request)
    {
        var serviceEntity = _mapper.Map<Service>(request);
        await _unitOfWork.Services.AddAsync(serviceEntity);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ServiceDto>(serviceEntity);
    }

    public async Task<bool> UpdateServiceAsync(int id, ServiceDto request)
    {
        var serviceEntity = await _unitOfWork.Services.GetFirstOrDefaultAsync(s => s.Id == id);
        if (serviceEntity == null) return false;
        _mapper.Map(request, serviceEntity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DeleteServiceAsync(int id)
    {
        var serviceEntity = await _unitOfWork.Services.GetFirstOrDefaultAsync(s => s.Id == id);
        if (serviceEntity == null) return false;

        _unitOfWork.Services.Remove(serviceEntity);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }
}