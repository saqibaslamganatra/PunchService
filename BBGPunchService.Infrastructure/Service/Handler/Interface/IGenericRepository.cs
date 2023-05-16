using System.Linq.Expressions;


namespace BBGPunchService.Infrastructure.Service.Handler.Interface
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(Guid id);       
        Task<IEnumerable<T>> GetAllAsync();
        Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression);
        Task AddAsync(T entity);
        Task<bool> Add(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        Task<bool> Upsert(T entity);
        void UpdateRange(IEnumerable<T> entities);        
        void Delete(T entity);
        Task Delete(Guid id);
        void RemoveRange(IEnumerable<T> entities);
        int Count();

    }
}
