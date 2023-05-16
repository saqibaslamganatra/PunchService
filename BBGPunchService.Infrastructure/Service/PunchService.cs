using BBGPunchService.Infrastructure.Configuration;
using BBGPunchService.Source.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Polly;
using Quartz;
using NLog;
using NLog.Extensions.Logging;

namespace BBGPunchService.Infrastructure.Service
{
    public class PunchService : IJob
    {
        private readonly IServiceScopeFactory _service;
        private readonly  NLog.ILogger _logger;
        private readonly int _batchSize = Convert.ToInt32(AppSettings.BatchSize);
        private readonly int daysToSynch = Convert.ToInt32(AppSettings.DaysToSynch);
        private readonly int daysToRetain = Convert.ToInt32(AppSettings.DaysToRetain);
        //private readonly DateTime _startSyncDate = Convert.ToDateTime(AppSettings.StartSyncDate);   
        private bool flag = true;
        public PunchService(IServiceScopeFactory service, NLog.ILogger logger)
        {
            _service = service;
            _logger = logger;
        }

        public async Task Execute(IJobExecutionContext context)
        {
            if (flag)
            {
                flag = false;
                await DeleteOldPunchs();
                await SavePunchesToTarget();             
                flag = true;
            }
        }
        public async Task DeleteOldPunchs()
        {
            var policy = Policy.Handle<Exception>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

            await policy.ExecuteAsync(async () =>
            {
                try
                {
                    using (var scope = _service.CreateScope())
                    {
                        using (var targetDbContext = scope.ServiceProvider.GetRequiredService<TargetDbContext>())
                        {
                            _logger.Info("Connected to the target database");
                            DateTime endOfDay = DateTime.Today.AddDays(1).AddTicks(-1);
                            var punchesToDelete = await targetDbContext.PunchingData
                                .Where(p => p.PunchDateTime >= DateTime.Today.AddDays(-daysToRetain) && p.PunchDateTime <= endOfDay)
                                .ToListAsync();

                            _logger.Info("Punches deleted from date {0} till date {1}. Total: {2} records are deleted.", DateTime.Today.AddDays(-daysToRetain), DateTime.Today, punchesToDelete.Count);
                            if (punchesToDelete.Any())
                            {
                                targetDbContext.PunchingData.RemoveRange(punchesToDelete);
                                await targetDbContext.SaveChangesAsync();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error occurred during delete old punches process.");
                }
            });
        }

        public async Task SavePunchesToTarget()
        {
            var policy = Policy.Handle<Exception>()
               .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(2));

            await policy.ExecuteAsync(async () =>
            {
                try
                {
                    using (IServiceScope scope = _service.CreateScope())
                    {
                        using (SourceDbContext sourceDbContext = scope.ServiceProvider.GetRequiredService<SourceDbContext>())
                        {
                            _logger.Info("Connected to the source database");
                            using (TargetDbContext targetDbContext = scope.ServiceProvider.GetRequiredService<TargetDbContext>())
                            {
                                _logger.Info("Connected to the target database");

                                int pageSize = _batchSize;
                                DateTime endOfDay = DateTime.Today.AddDays(1).AddTicks(-1);
                                var newPunches = sourceDbContext.Punch.AsNoTracking().Include(p => p.Employee)
                                    .Where(p => p.EmployeeId != null)
                                    .Where(p => p.PunchTime >= DateTime.Today.AddDays(-daysToSynch) && p.PunchTime <= endOfDay)
                                    .Where(p => p.PunchType == 1 || p.PunchType == 2)
                                    .OrderBy(p => p.PunchNumber);

                                while (await newPunches.AnyAsync())
                                {
                                    _logger.Info("Syncing punches started.");

                                    var punchBatch = await newPunches
                                                    .Take(pageSize)
                                                    .ToListAsync();

                                    if (punchBatch.Any())
                                    {
                                        var addingData = punchBatch.Where(p => !targetDbContext.PunchingData.AsTracking().Any(x => x.PunchNumber == p.PunchNumber))
                                        .Select(p => new BBGPunchService.Core.Model.TargetEntity.PunchingData
                                        {
                                            Oid = p.Oid,
                                            EnrolNo = p.Employee?.EnrollNo,
                                            FullName = p.Employee?.EmployeeName,
                                            PunchDateTime = p.PunchTime,
                                            PunchDirection = p.PunchType == 1 ? "In" : "Out",
                                            PunchNumber = p.PunchNumber
                                        });

                                        targetDbContext.PunchingData.AddRange(addingData);
                                        _logger.Info("transfering {0} records", addingData.Count());
                                        await targetDbContext.SaveChangesAsync();
                                            
                                    }

                                    _logger.Info("last punch number {0} ", punchBatch.Last().PunchNumber);
                                    newPunches = newPunches.Where(p => p.PunchNumber > punchBatch.Last().PunchNumber).OrderBy(p => p.PunchNumber);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "An error occurred during the sync process.");
                }
            });
        }
     

    }

}
//First Change Source Connection String to local !!!!!
//public async Task GenerateSampleData()
//{
//    using (IServiceScope scope = _service.CreateScope())
//    {
//        using (SourceDbContext sourceDbContext = scope.ServiceProvider.GetRequiredService<SourceDbContext>())
//        {
//            Random random = new Random();

//            DateTime endDate = DateTime.Now;

//            var lstEmployee = sourceDbContext.Employee.AsNoTracking().Select(x => new { x.EmployeeName, x.EnrollNo }).Distinct().ToList();

//            var topPunchNumber = sourceDbContext.Punch.OrderByDescending(p => p.PunchNumber).Select(p => p.PunchNumber).First();

//            for (int i = 0; i < 1000000000; i++)
//            {
//                int someRandomNumber = random.Next(0, lstEmployee.Select(x => x.EmployeeName).Count());
//                Employee employee = new Employee
//                {

//                    EmployeeName = lstEmployee.Select(x => x.EmployeeName).ElementAt(someRandomNumber),
//                    EnrollNo = lstEmployee.Select(x => x.EnrollNo).ElementAt(someRandomNumber),
//                    Punch = new List<Punch>()
//                };

//                for (int j = 0; j < 10; j++)
//                {
//                    Punch punch = new Punch
//                    {
//                        PunchTime = RandomDate(_startSyncDate, endDate, random),
//                        PunchType = random.Next(1, 2),
//                        PunchNumber = topPunchNumber++
//                    };

//                    employee.Punch.Add(punch);
//                }

//                sourceDbContext.Employee.Add(employee);
//                sourceDbContext.SaveChanges();
//            }


//        }
//    }

//}

//private static DateTime RandomDate(DateTime startDate, DateTime endDate, Random random)
//{
//    TimeSpan timeSpan = endDate - startDate;
//    TimeSpan newSpan = new TimeSpan(0, random.Next(0, (int)timeSpan.TotalMinutes), 0);
//    DateTime newDate = startDate + newSpan;

//    return newDate;
//}