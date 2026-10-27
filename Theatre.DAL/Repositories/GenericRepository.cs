using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using Theatre.DAL.Abstractions;
namespace Theatre.DAL.Repositories;
public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly TheatreDbContext _context;
    protected readonly DbSet<TEntity> _dbSet;
    public GenericRepository(TheatreDbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }
    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }
    public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        return await _dbSet.Where(predicate).ToListAsync();
    }
    public async Task<TEntity?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }
    public async Task AddAsync(TEntity entity)
    {
        await _dbSet.AddAsync(entity);
    }
    public void Update(TEntity entity)
    {
        _dbSet.Update(entity);
    }
    public void Remove(TEntity entity)
    {
        _dbSet.Remove(entity);
    }
}