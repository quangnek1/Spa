using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common.InterFaces;

namespace Spa.Domain.Entities.Services;

public class Category : EntitySeoAuditBase<int>
{
  [Column(TypeName = "nvarchar(250)")]
  public string Name { get; set; }
  public int ParrentId { get; set; }
  public int Sort { get; set; }
  public bool Status { get; set; }
}