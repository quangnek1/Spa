namespace Spa.Domain.Repositories;

public interface IUnitOfWork: IDisposable
{
    // Cung cấp các Repository cụ thể ở đây (Ví dụ)
    IGenericRepository<Entities.Bookings.Booking> Bookings { get; }
    IGenericRepository<Entities.Services.Service> Services { get; }
    
    // Hàm lưu tất cả thay đổi cùng 1 lúc
    Task<int> SaveChangesAsync();

}