using System.Linq.Expressions;

namespace Theatre.DAL.Abstractions;

public interface IRepository<TEntity> where TEntity : class
{    Task<IEnumerable<TEntity>> GetAllAsync();
  Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate);
  Task<TEntity?> GetByIdAsync(int id);
    Task  AddAsync(TEntity entity);
    void Update(TEntity entity);
    void Remove(TEntity entity);
}