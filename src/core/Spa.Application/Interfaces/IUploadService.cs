using Microsoft.AspNetCore.Http;

namespace Spa.Application.Interfaces;

public interface IUploadService
{
    Task<string> SaveFileAsync(IFormFile file, string folderName, string webRootPath);
    void DeleteFile(string fileName, string folderName,string webRootPath);
}