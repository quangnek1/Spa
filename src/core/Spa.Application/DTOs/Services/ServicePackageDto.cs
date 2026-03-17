using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Services;

namespace Spa.Application.DTOs.Services;

public class ServicePackageDto  : IMapFrom<ServicePackage>
{
    public int Id { get; set; }
    public int DurationMinutes { get; set; }
    public decimal Price { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ServicePackageDto, ServicePackage>().ReverseMap();
    }
}