using Spa.Domain.Common.InterFaces;

namespace Spa.Domain.Common;

public class EntityBase<T> : IEntityBase<T>
{
    public T Id { get; set; }
}