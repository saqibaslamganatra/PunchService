using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Spi;
using System.Collections.Concurrent;


namespace BBGPunchService.Infrastructure.JobFactory
{
    public class JobFactory : IJobFactory
    {
        private readonly IServiceProvider service;

        protected readonly ConcurrentDictionary<IJob, IServiceScope> _scopes = new ConcurrentDictionary<IJob, IServiceScope>();


        public JobFactory(IServiceProvider serviceProvider)
        {
            service = serviceProvider;
        }
        public IJob NewJob(TriggerFiredBundle bundle, IScheduler scheduler)
        {
            var jobDetail = bundle.JobDetail;
            var job = service.GetService(jobDetail.JobType);
            if (job == null)
            {
                throw new Exception("Failed to create the job, job is null");
            }
            return (IJob)job;
        }

        public void ReturnJob(IJob job)
        {
            if (_scopes.TryRemove(job, out var scope))
            {
                // The Dispose() method ends the scope lifetime.
                // Once Dispose is called, any scoped services that have been resolved from ServiceProvider will be disposed.
                scope.Dispose();
            }
        }
    }
}
