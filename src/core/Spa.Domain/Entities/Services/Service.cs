using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common.InterFaces;

namespace Spa.Domain.Entities.Services;

public class Service<T> : EntitySeoAuditBase<int>
{
    public int CategoryId { get; set; }
    [Column(TypeName = "nvarchar(250)")]
    public string Name { get; set; }
    [Column(TypeName = "nvarchar(250)")] 
    public string Image { get; set; }
    [Column(TypeName = "nvarchar(250)")] 
    public string Description { get; set; }
    [Column(TypeName = "nvarchar(250)")]
    public string ShortDescription { get; set; }
    public DateTime Hot { get; set; }
    public bool Status { get; set; }
    
}