namespace Spa.Application.DTOs.Services;

public class ServiceDto
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Image { get; set; } = default!;
    public string Description { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;

    // Gom các Package thành danh sách giá cơ bản để hiển thị
    public List<ServicePackageDto> Packages { get; set; } = new();
}