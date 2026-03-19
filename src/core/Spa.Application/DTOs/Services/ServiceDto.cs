using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Services;

namespace Spa.Application.DTOs.Services;

public class ServiceDto : IMapFrom<Service>
{
    public int Id { get; set; } = default!;
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Image { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public bool? Hot { get; set; } = default!;
    public int CategoryId { get; set; } 

    // Gom các Package thành danh sách giá cơ bản để hiển thị
    public List<ServicePackageDto>? Packages { get; set; } = new();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<ServiceDto, Service>().ReverseMap();
    }
}