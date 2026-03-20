using Microsoft.AspNetCore.Http;
using Spa.Application.Interfaces;


namespace Spa.Application.Services;

public class UploadService: IUploadService
{

    public UploadService()
    {
    }

    public async Task<string> SaveFileAsync(IFormFile file, string folderName, string webRootPath)
    {
        if (file == null) throw new ArgumentNullException(nameof(file));

        // Kiểm tra xem webRootPath có bị null không (đề phòng trường hợp chưa tạo folder wwwroot)
        if (string.IsNullOrEmpty(webRootPath)) 
            throw new Exception("Thư mục wwwroot chưa được khởi tạo.");

        var path = Path.Combine(webRootPath, folderName);

        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
        var fullPath = Path.Combine(path, fileName);

        using (var stream = new FileStream(fullPath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        return fileName;
    }

    public void DeleteFile(string fileName, string folderName,string webRootPath)
    {
        var path = Path.Combine(webRootPath, folderName, fileName);
        if (File.Exists(path))
        {
            File.Delete(path);
        }
    }
}