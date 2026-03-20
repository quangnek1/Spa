using Spa.Application.DTOs.Customers;

namespace Spa.Application.Interfaces;

public interface ICustomerService
{
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync();
}