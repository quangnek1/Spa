using AutoMapper;
using Spa.Application.Mappings;
using Spa.Domain.Entities.Services;

namespace Spa.Application.DTOs.Categories;

public class CategoryDto : IMapFrom<Category>
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string? SeoTitle { get; set; }
    public string? MetaKeywords { get; set; }
    public string? MetaDescription { get; set; }
    public int? ParentId { get; set; }
    public int Sort { get; set; }
    public bool Status { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Category, CategoryDto>().ReverseMap();
    }
}