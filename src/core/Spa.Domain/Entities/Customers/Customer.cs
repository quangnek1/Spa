using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.Customers;

public class Customer : EntityBase<int>
{
	[Column(TypeName = "nvarchar(250)")]
	public string Name { get; set; }
	public DateTime? DateOfBirth { get; set; }
	[Column(TypeName = "nvarchar(250)")]
	public string? Address { get; set; }
	[Column(TypeName = "nvarchar(250)")]
	[EmailAddress]
	public string? Email { get; set; }
	[Column(TypeName = "nvarchar(20)")]
	public string? Phone { get; set; }
	public bool Status { get; set; }
}