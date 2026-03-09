using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;
using Spa.Domain.Entities.Reviews;

namespace Spa.Domain.Entities.Services;

public class Service : EntitySeoAuditBase<int>
{
    public int CategoryId { get; set; }
    [Column(TypeName = "nvarchar(250)")]
    public string Name { get; set; }
    [Column(TypeName = "nvarchar(250)")] 
    public string Image { get; set; }
	[Column(TypeName = "nvarchar(250)")]
	public string ShortDescription { get; set; }
	[Column(TypeName = "nvarchar(max)")] 
    public string Description { get; set; }
    public bool? Hot { get; set; }
    public bool Status { get; set; }

	public Category Category { get; set; }
	public ICollection<ServicePackage>? Packages { get; set; }
	public ICollection<ServiceImage>? Images { get; set; } // Link tới bảng Gallery
	public ICollection<Review>? Reviews { get; set; }      // Link tới bảng Đánh giá

}