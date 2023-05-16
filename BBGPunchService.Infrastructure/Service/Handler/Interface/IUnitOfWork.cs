using BBGPunchService.Infrastructure.Service.Repository.Interface;

namespace BBGPunchService.Infrastructure.Service.Handler.Interface
{
    public interface IUnitOfWork : IDisposable
    {
        public IPunchingDataRepository IPunchingDataRepo { get; }
        bool Save();
    }
}
