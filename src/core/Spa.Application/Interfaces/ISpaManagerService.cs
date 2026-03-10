using Spa.Application.DTOs;

namespace Spa.Application.Interfaces
{
	public interface ISpaManagerService
	{
		// Lấy tất cả dịch vụ (kèm theo các gói 60p, 90p)
		Task<IEnumerable<ServiceDto>> GetAllServicesWithPackagesAsync();

		// Lấy chi tiết 1 dịch vụ theo Slug (Dùng cho trang chi tiết Next.js)
		Task<ServiceDto?> GetServiceBySlugAsync(string slug);
	}
}
