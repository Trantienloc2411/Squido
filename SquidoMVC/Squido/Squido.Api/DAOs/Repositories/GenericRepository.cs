using System.Data;
using System.Linq.Expressions;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DAOs.Interfaces;
using WebApplication1.Models;

namespace WebApplication1.DAOs.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : class
{
    protected SquidoDbContext _context;
    protected DbSet<T> _dbSet;

#pragma warning disable IDE0290 // Use primary constructor
    public GenericRepository(SquidoDbContext context)
#pragma warning restore IDE0290 // Use primary constructor
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn'T match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8767 // Nullability of reference types in type of parameter doesn'T match implicitly implemented member (possibly because of nullability attributes).
    public virtual IEnumerable<T> Get(
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn'T match implicitly implemented member (possibly because of nullability attributes).
#pragma warning restore CS8767 // Nullability of reference types in type of parameter doesn'T match implicitly implemented member (possibly because of nullability attributes).
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
       Expression<Func<T, bool>> filter = null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
       string includeProperties = "",
       int? pageIndex = null,
       int? pageSize = null)
    {
        IQueryable<T> query = _dbSet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (var includeProperty in includeProperties.Split
            (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy != null)
        {
            query = orderBy(query);
        }

        if (pageIndex.HasValue && pageSize.HasValue)
        {
            int validPageIndex = pageIndex.Value > 0 ? pageIndex.Value - 1 : 0;
            int validPageSize = pageSize.Value > 0 ? pageSize.Value : 10;

            query = query.Skip(validPageIndex * validPageSize).Take(validPageSize);
        }

        return query.ToList();
    }

    public virtual T GetByID(object id)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return _dbSet.Find(id);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public virtual void Insert(T entity)
    {
        if (entity == null) return;
        _dbSet.Add(entity);
    }

    public virtual bool Delete(object id)
    {
        T entityToDelete = GetByID(id);
        if (entityToDelete == null) return false;
        Delete(entityToDelete);
        return true;
    }
    public virtual bool Update(object id, T entityUpdate)
    {
        T entity = GetByID(id);
        if (entity == null) return false;
        Update(entityUpdate);
        return true;
    }

    public virtual void Delete(T entityToDelete)
    {
        if (_context.Entry(entityToDelete).State == EntityState.Detached)
        {
            _dbSet.Attach(entityToDelete);
        }
        _dbSet.Remove(entityToDelete);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        await Task.Run(() => _dbSet.Remove(entity));
    }

    public virtual void Update(T entityToUpdate)
    {
        var trackedEntities = _context.ChangeTracker.Entries<T>().ToList();
        foreach (var trackedEntity in trackedEntities)
        {
            trackedEntity.State = EntityState.Detached;
        }
        _dbSet.Attach(entityToUpdate);
        _context.Entry(entityToUpdate).State = EntityState.Modified;
    }

    public IEnumerable<T> GetAll()
    {
        return _dbSet.ToList();
    }

    public async Task<ICollection<T>> GetAllWithIncludeAsync(Expression<Func<T, bool>> predicate, params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }
        return await query.Where(predicate).ToListAsync();
    }

    public async Task<T> GetSingleWithIncludeAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties)
    {
        IQueryable<T> query = _dbSet;

        foreach (var includeProperty in includeProperties)
        {
            query = query.Include(includeProperty);
        }

#pragma warning disable CS8603 // Possible null reference return.
        return await query.FirstOrDefaultAsync(predicate);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public void AddRange(ICollection<T> entities)
    {
        _dbSet.AddRange(entities);
    }

    public IEnumerable<TResult> ExecuteStoredProcedure<TResult>(string storedProcedure, params SqlParameter[] parameters) where TResult : class, new()
    {
        using (var command = _context.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = storedProcedure;
            command.CommandType = CommandType.StoredProcedure;

            if (parameters != null && parameters.Length > 0)
            {
                command.Parameters.AddRange(parameters);
            }

            _context.Database.OpenConnection();

            using (var result = command.ExecuteReader())
            {
                var entities = new List<TResult>();
                while (result.Read())
                {
                    var entity = new TResult();
                    foreach (var property in typeof(TResult).GetProperties())
                    {
                        if (!result.IsDBNull(result.GetOrdinal(property.Name)))
                        {
                            property.SetValue(entity, result[property.Name]);
                        }
                    }
                    entities.Add(entity);
                }
                return entities;
            }
        }
    }
    public Task UpdateAsync(T entity)
    {
        ArgumentNullException.ThrowIfNull(entity);

        // Get the primary key value (assumes key property is named "Id")
        var keyProperty = typeof(T).GetProperty("Id");
        if (keyProperty == null)
            throw new InvalidOperationException($"Entity '{typeof(T).Name}' does not have a property named 'Id'.");

        var key = keyProperty.GetValue(entity);
        if (key == null)
            throw new InvalidOperationException("Entity key cannot be null.");

        // Check if this entity is already being tracked
        var trackedEntity = _context.ChangeTracker.Entries<T>()
                                    .FirstOrDefault(e =>
                                        e.State != EntityState.Detached &&
                                        key.Equals(keyProperty.GetValue(e.Entity)));

        if (trackedEntity != null)
        {
            // Detach the tracked entity
            trackedEntity.State = EntityState.Detached;
        }

        // Attach and mark the new entity as modified
        _context.Set<T>().Attach(entity);
        _context.Entry(entity).State = EntityState.Modified;

        return Task.CompletedTask;
    }

    public Task<int> CountAsync(Expression<Func<T, bool>> predicate)
    {
        ArgumentNullException.ThrowIfNull(predicate);

        return _dbSet.CountAsync(predicate);
    }
}