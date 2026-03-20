using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spa.Application.DTOs.Categories;
using Spa.Application.Seedwork;
using Spa.Domain.Entities.Services;

namespace Spa.WebAPI.Controllers;

public class CategoriesController : BaseController
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CategoriesController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }
    // GET: api/categories
    [HttpGet]
    [AllowAnonymous] // Cho phép khách vãng lai xem danh mục ngoài trang chủ
    public async Task<IActionResult> GetAllCategories()
    {
        // Lấy toàn bộ danh mục, sắp xếp theo Sort
        var categories = await _unitOfWork.Categories.GetAll(false)
                                        .OrderBy(c => c.Sort)
                                        .ToListAsync();
        
        var categoryDtos = _mapper.Map<IEnumerable<CategoryDto>>(categories);
        return Ok(categoryDtos);
    }

    // GET: api/categories/active
    [HttpGet("active")]
    [AllowAnonymous]
    public async Task<IActionResult> GetActiveCategories()
    {
        // Chỉ lấy những danh mục đang bật Status = true (Dùng cho Dropdown Frontend)
        var categories = await _unitOfWork.Categories.FindAsync(c => c.Status == true);
        var sortedCategories = categories.OrderBy(c => c.Sort);
        
        return Ok(_mapper.Map<IEnumerable<CategoryDto>>(sortedCategories));
    }

    // GET: api/categories/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return NotFound(new { message = "Không tìm thấy danh mục" });

        return Ok(_mapper.Map<CategoryDto>(category));
    }

    // POST: api/categories
    [HttpPost]
    [Authorize(Roles = "Administrator")] // Chỉ Admin mới được thêm
    public async Task<IActionResult> CreateCategory([FromBody] CategoryDto request)
    {
        var category = _mapper.Map<Category>(request);
        
        await _unitOfWork.Categories.AddAsync(category);
        await _unitOfWork.SaveChangesAsync();

        // Trả về DTO của object vừa tạo (bao gồm cả Id do EF Core sinh ra)
        var createdDto = _mapper.Map<CategoryDto>(category);
        return CreatedAtAction(nameof(GetCategoryById), new { id = category.Id }, createdDto);
    }

    // PUT: api/categories/{id}
    [HttpPut("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto request)
    {
        if (id != request.Id) return BadRequest(new { message = "ID không khớp" });

        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return NotFound(new { message = "Không tìm thấy danh mục" });

        // Map đè dữ liệu từ request sang entity đang có trong DB
        _mapper.Map(request, category);
        
        _unitOfWork.Categories.Update(category);
        await _unitOfWork.SaveChangesAsync();

        return NoContent(); // Thành công nhưng không cần trả về body
    }

    // DELETE: api/categories/{id}
    [HttpDelete("{id:int}")]
    [Authorize(Roles = "Administrator")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _unitOfWork.Categories.GetByIdAsync(id);
        if (category == null) return NotFound();

        // 🚨 Lưu ý: Trong thực tế, nếu Category này đã có Service bên trong, 
        // SQL Server sẽ báo lỗi khóa ngoại (Foreign Key constraint). 
        // Bác có thể phải bắt try-catch chỗ này hoặc check xem có Service nào đang dùng không.
        
        _unitOfWork.Categories.Remove(category);
        await _unitOfWork.SaveChangesAsync();

        return NoContent();
    }
}