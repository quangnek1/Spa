using Microsoft.AspNetCore.Mvc;
using Spa.Application.Interfaces;

namespace Spa.WebAPI.Controllers;

public class UploadController : BaseController
{
    private readonly IUploadService _uploadService;
    private readonly IWebHostEnvironment _env;

    public UploadController(IUploadService uploadService, IWebHostEnvironment env)
    {
        _uploadService = uploadService;
        _env = env;
    }

    [HttpPost("service-image")]
    public async Task<IActionResult> UploadServiceImage(IFormFile file)
    {
        try 
        {
            // Lấy trực tiếp WebRootPath từ env của Controller truyền sang
            var fileName = await _uploadService.SaveFileAsync(file, "services", _env.WebRootPath);
            return Ok(new { fileName = fileName });
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}