using System.ComponentModel.DataAnnotations.Schema;
using Spa.Domain.Common;

namespace Spa.Domain.Entities.Post;

public class Post : EntitySeoAuditBase<int>
{
    [Column(TypeName = "nvarchar(250)")] public string Title { get; set; } = default!;

    [Column(TypeName = "nvarchar(250)")] public string Image { get; set; } = default!;

    [Column(TypeName = "nvarchar(500)")]
    public string Summary { get; set; } = default!; // Mô tả ngắn hiển thị ở danh sách

    [Column(TypeName = "nvarchar(max)")]
    public string Content { get; set; } = default!; // Nội dung chi tiết (HTML từ Editor)

    public int ViewCount { get; set; } // Đếm lượt xem để làm mục "Bài viết nổi bật"
    public bool IsHot { get; set; }
    public bool Status { get; set; }
    public DateTime? PublishedDate { get; set; } // Hẹn giờ đăng bài

    public int? PostCategoryId { get; set; }
    public PostCategory? PostCategory { get; set; }
}