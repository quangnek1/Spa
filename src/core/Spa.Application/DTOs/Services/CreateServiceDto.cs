using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Services;

namespace Spa.Application.DTOs.Services;

public class CreateServiceDto : IMapFrom<Service>
{
    public int CategoryId { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Image { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public List<ServicePackageDto>? Packages { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<CreateServiceDto, Service>().ReverseMap();
        profile.CreateMap<ServicePackageDto, ServicePackage>();
    }
}