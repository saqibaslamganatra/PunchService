using BBGPunchService.Core.Model.TargetEntity;
using BBGPunchService.Infrastructure.Service.Handler.Interface;

namespace BBGPunchService.Infrastructure.Service.Repository.Interface
{
    public interface IPunchingDataRepository : IGenericRepository<BBGPunchService.Core.Model.TargetEntity.PunchingData>
    {
        IEnumerable<PunchingData> GetPunchingData();
        IQueryable<PunchingData> GetPunchingDataQueryable();

        Task<PunchingData> FindByEmployeeNoAsync(string employeeNo);
    }
}
