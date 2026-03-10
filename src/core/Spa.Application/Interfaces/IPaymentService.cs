using Spa.Application.DTOs;

namespace Spa.Application.Interfaces
{
	public interface IPaymentService
	{
		Task<bool> ProcessBookingPaymentAsync(ProcessPaymentRequestDto request);
	}
}
