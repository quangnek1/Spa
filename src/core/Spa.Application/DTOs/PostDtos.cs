namespace Spa.Application.DTOs;

public class PostDtos
{
}

public class PostDto
{
    public int Id { get; set; }
    public string Title { get; set; } = default!;
    public string Slug { get; set; } = default!;
    public string Image { get; set; } = default!;
    public string ShortDescription { get; set; } = default!;
    public string Content { get; set; } = default!;
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = default!;
    public DateTimeOffset CreatedDate { get; set; }
}

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