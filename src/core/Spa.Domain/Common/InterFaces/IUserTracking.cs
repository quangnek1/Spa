namespace Spa.Domain.Common.InterFaces;

public interface IUserTracking
{
    public string CreatedBy { get; set; }
    public string ModifiedBy { get; set; }
}