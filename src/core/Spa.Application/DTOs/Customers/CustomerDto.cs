namespace Spa.Application.DTOs.Customers;

public class CustomerDto
{
    public Guid Id { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public int TotalBookings { get; set; }
    public decimal TotalSpent { get; set; }
}