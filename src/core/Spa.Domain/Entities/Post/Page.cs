using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.Post;

public class Page : EntitySeoAuditBase<int>
{
    [Column(TypeName = "nvarchar(250)")] public string Title { get; set; } = default!;

    [Column(TypeName = "nvarchar(max)")] public string Content { get; set; } = default!;

    public bool Status { get; set; }
}