using System.ComponentModel.DataAnnotations;

namespace Spa.Domain.Enums;

public enum ContactStatus
{
    [Display(Name = "Mới")] New = 1,
    [Display(Name = "Đang xử lý")] InProgress = 2,
    [Display(Name = "Đã giải quyết")] Resolved = 3
}