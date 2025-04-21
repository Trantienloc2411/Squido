using System.Linq.Expressions;
using Microsoft.Data.SqlClient;

namespace WebApplication1.DAOs.Interfaces;

public interface IGenericRepository<T> where T : class
{
    IEnumerable<T> Get(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        string includeProperties = "",
        int? pageIndex = null,
        int? pageSize = null);

    T GetByID(object id);
    Task AddAsync(T entity);
    Task DeleteAsync(T entity);
    void Update(T entity);

    void Insert(T entity);

    bool Delete(object id);

    void Delete(T entityToDelete);
    bool Update(object id, T entityToUpdate);

    IEnumerable<T> GetAll();
    IEnumerable<TResult> ExecuteStoredProcedure<TResult>(string storedProcedure, params SqlParameter[] parameters) where TResult : class, new();
    Task<T> GetSingleWithIncludeAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties);

    Task<ICollection<T>> GetAllWithIncludeAsync(Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includeProperties);
    void AddRange(ICollection<T> entities);
}