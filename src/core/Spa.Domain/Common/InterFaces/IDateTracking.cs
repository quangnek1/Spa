namespace Spa.Domain.Common.InterFaces;

public interface IDateTracking
{
    DateTimeOffset CreatedDate { get; set; }
    DateTimeOffset? LastModifiedDate { get; set; }
}