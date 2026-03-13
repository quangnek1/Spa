using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Spa.Application.Seedwork;
using Spa.Infrastructure.Data;

namespace Spa.Infrastructure.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected DbSet<T> dbSet;

    public GenericRepository(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        dbSet = _context.Set<T>() ??  throw new ArgumentNullException(nameof(context));
    }

    public async Task<T?> GetByIdAsync(object id)
    {
        return await dbSet.FindAsync(id);
    }

	public IQueryable<T> GetAll(bool trackChanges = false) => !trackChanges ? _context.Set<T>().AsNoTracking() : _context.Set<T>();
	public IQueryable<T> GetAll(bool trackChanges = false, params Expression<Func<T, object>>[] includeProperties)
	{
		var items = GetAll(trackChanges);
		items = includeProperties.Aggregate(items, (current, includeProperty) => current.Include(includeProperty));
		return items;
	}


	public async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> expression,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = dbSet;
        foreach (var include in includes) query = query.Include(include);

        return await query.Where(expression).ToListAsync();
    }

    public async Task<T?> GetFirstOrDefaultAsync(Expression<Func<T, bool>> expression,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = dbSet;
        foreach (var include in includes) query = query.Include(include);

        return await query.FirstOrDefaultAsync(expression);
    }

    public async Task AddAsync(T entity)
    {
        await dbSet.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await dbSet.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        dbSet.Update(entity);
    }

    public void Remove(T entity)
    {
        dbSet.Remove(entity);
    }

    public void RemoveRange(IEnumerable<T> entities)
    {
        dbSet.RemoveRange(entities);
    }

	
}