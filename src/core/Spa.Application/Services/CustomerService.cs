using Microsoft.AspNetCore.Identity;
using Spa.Application.DTOs.Customers;
using Spa.Application.Interfaces;
using Spa.Application.Seedwork;
using Spa.Domain.Entities.Identity;

namespace Spa.Application.Services;

public class CustomerService : ICustomerService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomerService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync()
    {
        // 1. Lấy tất cả User có Role là "Customer"
        var customersInRole = await _userManager.GetUsersInRoleAsync("Customer");

        // 2. Vì GetUsersInRoleAsync thường không đi kèm Include(Bookings) 
        // nên ta cần dùng Queryable để tính toán cho chính xác
        var query = _userManager.Users
            .Where(u => customersInRole.Select(r => r.Id).Contains(u.Id));

        return query.Select(u => new CustomerDto
        {
            Id = u.Id,
            FirstName = u.FirstName,
            LastName = u.LastName,
            Email = u.Email,
            Phone = u.PhoneNumber, // 🟢 Đổi Phone -> PhoneNumber (Của Identity)
            TotalBookings = u.Bookings != null ? u.Bookings.Count() : 0,
            TotalSpent = u.Bookings != null 
                ? u.Bookings.Sum(b => b.TotalPrice) 
                : 0
        }).ToList();
    }
}