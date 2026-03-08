namespace Spa.Domain.Common.InterFaces;

public interface ISoftDelete
{
    public bool IsDeleted { get; set; }
}