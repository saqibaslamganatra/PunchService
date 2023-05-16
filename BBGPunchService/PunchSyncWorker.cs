using BBGPunchService.Infrastructure.Model;
using Quartz;
using Quartz.Spi;

namespace BBGPunchService
{
    class PunchSyncWorker : IHostedService
    {
        public IScheduler? Scheduler { get; set; }
        private readonly IJobFactory jobFactory;
        private readonly List<JobMetadata> jobMetadatas;
        private readonly ISchedulerFactory schedulerFactory;
        private readonly ILogger<PunchSyncWorker> _logger;

        public PunchSyncWorker(ILogger<PunchSyncWorker> logger,ISchedulerFactory schedulerFactory, List<JobMetadata> jobMetadatas, IJobFactory jobFactory)
        {
            this.jobFactory = jobFactory;
            this.schedulerFactory = schedulerFactory;
            this.jobMetadatas = jobMetadatas;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            //Create Schedular
            Scheduler = await schedulerFactory.GetScheduler(cancellationToken);
            Scheduler.JobFactory = jobFactory;

            //Suporrt for Multiple Jobs
            jobMetadatas?.ForEach(jobMetadata =>
            {
                //Create Job
                IJobDetail jobDetail = CreateJob(jobMetadata);
                //Create trigger
                ITrigger trigger = CreateTrigger(jobMetadata);
                //Schedule Job
                Scheduler.ScheduleJob(jobDetail, trigger, cancellationToken).GetAwaiter();
                //Start The Schedular
            });
            await Scheduler.Start(cancellationToken);
        }

        private ITrigger CreateTrigger(JobMetadata jobMetadata)
        {

            return TriggerBuilder.Create()
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithCronSchedule(jobMetadata.CronExpression)
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        private IJobDetail CreateJob(JobMetadata jobMetadata)
        {
            return JobBuilder.Create(jobMetadata.JobType)
                .WithIdentity(jobMetadata.JobId.ToString())
                .WithDescription(jobMetadata.JobName)
                .Build();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (Scheduler != null)
            {
                await Scheduler.Shutdown(cancellationToken);
            }
        }
    }

}
