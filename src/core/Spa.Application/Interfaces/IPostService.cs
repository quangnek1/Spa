using Spa.Application.DTOs;

namespace Spa.Application.Interfaces
{
	public interface IPostService
	{
		Task<IEnumerable<PostDto>> GetAllActivePostsAsync();
		Task<PostDto?> GetPostBySlugAsync(string slug);
		Task<PostDto> CreatePostAsync(CreateUpdatePostDto request);
		Task<bool> UpdatePostAsync(int id, CreateUpdatePostDto request);
		Task<bool> DeletePostAsync(int id);
	}
}
