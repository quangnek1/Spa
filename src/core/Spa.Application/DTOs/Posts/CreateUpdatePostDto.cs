namespace Spa.Application.DTOs.Posts;

public class CreateUpdatePostDto
{
	public string Title { get; set; } = default!;
	public string Slug { get; set; } = default!;
	public string Image { get; set; } = default!;
	public string ShortDescription { get; set; } = default!;
	public string Content { get; set; } = default!;
	public int CategoryId { get; set; }
	public bool Status { get; set; } = true;
}