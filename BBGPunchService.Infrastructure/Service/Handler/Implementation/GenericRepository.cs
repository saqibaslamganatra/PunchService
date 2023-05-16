using BBGPunchService.Infrastructure.Service.Handler.Interface;
using BBGPunchService.Source.Data;
using System.Linq.Expressions;
using System.Data.Entity;
using BBGPunchService.Core.Model.TargetEntity;

namespace BBGPunchService.Infrastructure.Service.Handler.Implementation
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly TargetDbContext _dbContext;
        protected GenericRepository(TargetDbContext _dbContext)
        {
            this._dbContext = _dbContext;       
        }

        public virtual async Task<bool> Add(T entity)
        {
            await _dbContext.AddAsync(entity);
            return true;
        }

        public virtual async Task AddAsync(T entity)
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public virtual  async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public virtual async Task Delete(Guid id)
        {
           var entity = await _dbContext.Set<T>().FindAsync(id);
             _dbContext.Set<T>().Remove(entity);
        }

        public virtual void Delete(T entity)
        {
            _dbContext.Set<T>().Remove(entity);
        }
        public virtual  async Task<IEnumerable<T>> Find(Expression<Func<T, bool>> expression)
        {
            return await _dbContext.Set<T>().Where(expression).ToListAsync();            
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetByIdAsync(Guid id)
        {
            return await _dbContext.Set<T>().FindAsync(id);
        }
        public void RemoveRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public virtual void Update(T entity)
        {
            _dbContext.Set<T>().Update(entity);
        }
        public virtual void UpdateRange(IEnumerable<T> entities)
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

        public Task<bool> Upsert(T entity)
        {
            throw new NotImplementedException();
        }
        public int Count()
        {
            return _dbContext.PunchingData.Count();
        }



    }
}
