using Microsoft.AspNetCore.Mvc;
using Spa.Application.DTOs.Posts;
using Spa.Application.Interfaces;

namespace Spa.WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;

    public PostsController(IPostService postService)
    {
        _postService = postService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var posts = await _postService.GetAllActivePostsAsync();
        return Ok(posts);
    }

    [HttpGet("{slug}")]
    public async Task<IActionResult> GetBySlug(string slug)
    {
        var post = await _postService.GetPostBySlugAsync(slug);
        if (post == null) return NotFound();
        return Ok(post);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUpdatePostDto request)
    {
        var result = await _postService.CreatePostAsync(request);
        return Ok(result);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] CreateUpdatePostDto request)
    {
        var result = await _postService.UpdatePostAsync(id, request);
        if (!result) return NotFound();
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _postService.DeletePostAsync(id);
        if (!result) return NotFound();
        return NoContent();
    }
}