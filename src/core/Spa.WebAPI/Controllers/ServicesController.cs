using Microsoft.AspNetCore.Mvc;
using Spa.Application.DTOs.Services;
using Spa.Application.Interfaces;

namespace Spa.WebAPI.Controllers;

public class ServicesController : BaseController
{
    private readonly ISpaManagerService _spaManagerService;

    public ServicesController(ISpaManagerService spaManagerService)
    {
        _spaManagerService = spaManagerService ?? throw new ArgumentNullException(nameof(spaManagerService));
    }
 
    // GET: api/services
    [HttpGet]
    public async Task<IActionResult> GetAllServices(bool activeOnly = false)
    {
        var services = await _spaManagerService.GetAllServicesWithPackagesAsync(activeOnly);
        return Ok(services);
    }

    // GET: api/services/{slug}
    [HttpGet("{slug}")]
    public async Task<IActionResult> GetServiceBySlug(string slug)
    {
        var service = await _spaManagerService.GetServiceBySlugAsync(slug);

        if (service == null)
            return NotFound();

        return Ok(service);
    }
    // GET: api/services/id/{id}
    [HttpGet("id/{id:int}")]
    public async Task<IActionResult> GetServiceById(int id)
    {
        // Nhớ viết thêm hàm GetServiceByIdAsync trong ISpaManagerService nhé
        var service = await _spaManagerService.GetServiceByIdAsync(id);

        if (service == null)
            return NotFound();

        return Ok(service);
    }

    // POST: api/services
    [HttpPost]
    public async Task<IActionResult> CreateService([FromBody] CreateServiceDto request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdService = await _spaManagerService.CreateServiceAsync(request);

        return CreatedAtAction(
            nameof(GetServiceBySlug),
            new { slug = createdService.Slug },
            createdService
        );
    }

    // PUT: api/services/{id}
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, [FromBody] ServiceDto request)
    {
        var result = await _spaManagerService.UpdateServiceAsync(id, request);

        if (!result)
            return NotFound();

        return NoContent();
    }

    // DELETE: api/services/{id}
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var result = await _spaManagerService.DeleteServiceAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
}