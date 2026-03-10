using System.Linq.Expressions;

namespace Spa.Domain.Repositories;

public interface IGenericRepository<T> where T : class
{
	Task<T?> GetByIdAsync(object id);
	Task<IEnumerable<T>> GetAllAsync();

	// Nâng cấp: Cho phép truyền lambda expression để lọc và Include các bảng con
	Task<IEnumerable<T>> FindAsync(
		Expression<Func<T, bool>> expression,
		params Expression<Func<T, object>>[] includes);

	Task<T?> GetFirstOrDefaultAsync(
		Expression<Func<T, bool>> expression,
		params Expression<Func<T, object>>[] includes);

	Task AddAsync(T entity);
	Task AddRangeAsync(IEnumerable<T> entities);
	void Update(T entity);
	void Remove(T entity);
	void RemoveRange(IEnumerable<T> entities);
}