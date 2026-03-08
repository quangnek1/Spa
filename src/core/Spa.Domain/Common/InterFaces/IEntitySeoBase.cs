namespace Spa.Domain.Common.InterFaces;
public interface IEntitySeoBase
{
    string? Slug { get; set; }
    string? SeoTitle { get; set; }
    string? MetaKeywords { get; set; }
    string? MetaDescription { get; set; }
}