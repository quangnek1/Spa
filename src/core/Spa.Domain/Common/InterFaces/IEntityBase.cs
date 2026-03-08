namespace Spa.Domain.Common.InterFaces;

public interface IEntityBase<T>
{
    T Id { get; set; }
}