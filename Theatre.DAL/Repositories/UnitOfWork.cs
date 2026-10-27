using System.Collections;
using Theatre.DAL.Abstractions;
using Theatre.DAL.Repositories;

namespace Theatre.DAL;

public class UnitOfWork : IUnitOfWork
{
    private readonly TheatreDbContext _context;
    private Hashtable? _repositories;
    private bool _disposed = false;
    public UnitOfWork(TheatreDbContext context)
    {
        _context = context;
    }
    public IRepository<TEntity>  GetRepository<TEntity>() where TEntity : class
    {
        _repositories ??= new Hashtable();

        var type = typeof(TEntity).Name;

        if (!_repositories.ContainsKey(type))
        {
            var repositoryType = typeof(GenericRepository<>);
            var repositoryInstance = Activator.CreateInstance(
                repositoryType.MakeGenericType(typeof(TEntity)), _context);

            _repositories.Add(type, repositoryInstance);
        }

        return (IRepository<TEntity>)_repositories[type]!;
    }
    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _context.Dispose();
            }
            _disposed = true;
        }
    }
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}