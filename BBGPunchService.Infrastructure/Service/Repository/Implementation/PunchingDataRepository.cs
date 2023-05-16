using BBGPunchService.Core.Model.TargetEntity;
using BBGPunchService.Infrastructure.Service.Handler.Implementation;
using BBGPunchService.Infrastructure.Service.Repository.Interface;
using BBGPunchService.Source.Data;
using Microsoft.EntityFrameworkCore;

namespace BBGPunchService.Infrastructure.Service.Repository.Implementation
{
    class PunchingDataRepository : GenericRepository<BBGPunchService.Core.Model.TargetEntity.PunchingData>, IPunchingDataRepository
    {
        public PunchingDataRepository(TargetDbContext _dbContext) : base(_dbContext) { }

        IEnumerable<BBGPunchService.Core.Model.TargetEntity.PunchingData> IPunchingDataRepository.GetPunchingData()
        {
            return _dbContext.PunchingData.OrderByDescending(x => x.PunchNumber);
        }

        public IQueryable<PunchingData> GetPunchingDataQueryable()
        {
            return _dbContext.PunchingData.OrderByDescending(x => x.PunchNumber).AsNoTracking();
        }
        public async Task<PunchingData> FindByEmployeeNoAsync(string employeeNo)
        {
            return await _dbContext.PunchingData.FirstOrDefaultAsync(x => x.EnrolNo == employeeNo);
        }

        //public async Task<IPagedList<PunchingData>>q GetPunchData(int pageIndex, int pageSize)
        //{
        //    var query = _dbContext.PunchingData.AsNoTracking();
        //    var totalItems = await query.CountAsync();
        //    var items = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync()
        //        ;
        //    return IPagedList<PunchingData>(items, pageIndex, pageSize, totalItems);


        //}

    }
}
