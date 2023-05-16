using BBGPunchService.Infrastructure.Service.Handler.Interface;
using BBGPunchService.Infrastructure.Service.Repository.Interface;
using BBGPunchService.Source.Data;

namespace BBGPunchService.Infrastructure.Service.Handler.Implementation
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly TargetDbContext _dbContext;
        //private readonly ILogger _logger;
        public IPunchingDataRepository IPunchingDataRepo { get; }

        public UnitOfWork(TargetDbContext dbContext, IPunchingDataRepository IpunchingRepo)// ,ILoggerFactory loggerFactory,)
        {
            _dbContext = dbContext;
            //  _logger = loggerFactory.CreateLogger("logs");
            IPunchingDataRepo = IpunchingRepo;           
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dbContext.Dispose();
            }
        }

        public bool Save()
        {
            return (_dbContext.SaveChanges() >= 0);
        }


    }
}
