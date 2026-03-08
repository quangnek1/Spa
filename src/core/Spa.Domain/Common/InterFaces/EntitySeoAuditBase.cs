using System.ComponentModel.DataAnnotations.Schema;

namespace Spa.Domain.Common.InterFaces;

public class EntitySeoAuditBase<T> : EntityAuditBase<T>, IEntitySeoBase
{
    [Column(TypeName = "nvarchar(250)")]
    public string? Slug { get; set; }
    [Column(TypeName = "nvarchar(250)")]
    public string? SeoTitle { get; set; }
    [Column(TypeName = "nvarchar(500)")]
    public string? MetaKeywords { get; set; }
    [Column(TypeName = "nvarchar(500)")]
    public string? MetaDescription { get; set; }
}