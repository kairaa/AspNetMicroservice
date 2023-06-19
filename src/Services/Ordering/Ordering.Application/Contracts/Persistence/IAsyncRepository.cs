using Ordering.Domain.Common;
using System.Linq.Expressions;

namespace Ordering.Application.Contracts.Persistence
{
    public interface IAsyncRepository<T> where T : EntityBase
    {
        #region GetMethods
        //returns the all data with given entity
        Task<IReadOnlyList<T>> GetAllAsync();
        //returns data with filter
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate);
        //returns data with orderby, like etc.
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                        Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                        string includeString = null,
                                        bool disableTracking = true);
        Task<IReadOnlyList<T>> GetAsync(Expression<Func<T, bool>> predicate = null,
                                       Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
                                       List<Expression<Func<T, object>>> includes = null,
                                       bool disableTracking = true);
        Task<T> GetByIdAsync(int id);
        #endregion
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
    }
}
