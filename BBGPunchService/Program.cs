using BBGPunchService;
using Microsoft.EntityFrameworkCore;
using Quartz.Impl;
using Quartz.Spi;
using Quartz;
using BBGPunchService.Infrastructure.Service;
using BBGPunchService.Infrastructure.JobFactory;
using BBGPunchService.Infrastructure.Model;
using NLog;
using BBGPunchService.Infrastructure.Configuration;
using BBGPunchService.Source.Data;
using Microsoft.Extensions.DependencyInjection;
using System.Diagnostics;
using Microsoft.Extensions.Hosting.WindowsServices;

var logger = LogManager.GetCurrentClassLogger();

var config = new ConfigurationBuilder()
        .SetBasePath(System.IO.Directory.GetCurrentDirectory()) //From NuGet Package Microsoft.Extensions.Configuration.Json
        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        .Build();

var isService = !(Debugger.IsAttached || args.Contains("--console"));

IHostBuilder builder = Host.CreateDefaultBuilder(args)
         .ConfigureServices((hostContext, services) =>
         {

             IConfiguration configuration = hostContext.Configuration;
             AppSettings.Configuration = configuration;

             AppSettings.SourceConnectionString = configuration.GetValue<string>("SourceConnectionString");

             AppSettings.TargetConnectionString = configuration.GetValue<string>("TargetConnectionString");

             AppSettings.BatchSize = configuration.GetValue<string>("BatchSize");

             AppSettings.DaysToSynch = configuration.GetValue<string>("DaysToSynch");

             AppSettings.DaysToRetain = configuration.GetValue<string>("DaysToRetain");

             AppSettings.StartSyncDate = configuration.GetValue<string>("StartSyncDate");

             var SourceoptionBuilder = new DbContextOptionsBuilder<SourceDbContext>();
             SourceoptionBuilder.UseSqlServer(AppSettings.SourceConnectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure())
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);


             var TargetoptionBuilder = new DbContextOptionsBuilder<TargetDbContext>();
             TargetoptionBuilder.UseSqlServer(AppSettings.TargetConnectionString,
                sqlServerOptions => sqlServerOptions.EnableRetryOnFailure())
                .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);


             services.AddScoped(d =>
             {
                 var sourceDbContext = new SourceDbContext(SourceoptionBuilder.Options);
                 sourceDbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));
                 return sourceDbContext;
             });

             services.AddScoped(d =>
             {
                 var targetDbContext = new TargetDbContext(TargetoptionBuilder.Options);
                 targetDbContext.Database.SetCommandTimeout(TimeSpan.FromMinutes(30));
                 return targetDbContext;
             });


             services.AddSingleton<NLog.ILogger>(NLog.LogManager.GetCurrentClassLogger());

             services.AddSingleton<IJobFactory, JobFactory>();
             services.AddSingleton<ISchedulerFactory, StdSchedulerFactory>();
             using var servicesProvider = new ServiceCollection()
               .AddLogging(loggingBuilder =>
               {
                   // configure Logging with NLog
                   //loggingBuilder.ClearProviders();
                   //loggingBuilder.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                   //loggingBuilder.AddConfiguration(hostContext.Configuration.GetSection("Logging")); //appsettings.json
                   //loggingBuilder.AddConsole(); //Adds a console logger named 'Console' to the factory.
                   //loggingBuilder.AddDebug(); //Adds a debug logger named 'Debug' to the factory.
                   //loggingBuilder.AddEventSourceLogger();
                   loggingBuilder.ClearProviders();
                   loggingBuilder.AddConfiguration(hostContext.Configuration.GetSection("Logging"));
                   loggingBuilder.AddConsole();


               }).BuildServiceProvider();

             #region Adding JobType
             services.AddSingleton<PunchService>();

             #endregion


             #region Adding Jobs 
             List<JobMetadata> jobMetadatas = new List<JobMetadata>
             {
             new JobMetadata(Guid.NewGuid(), typeof(PunchService), "CronExpression", configuration.GetValue<string>("CronExpression"))
             };

             services.AddSingleton(jobMetadatas);

             #endregion

             services.AddHostedService<PunchSyncWorker>();
         });

if (isService)
{
    builder.UseWindowsService();
}

var host = builder.Build();

host.Run();
