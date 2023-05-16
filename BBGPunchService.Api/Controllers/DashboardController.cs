using BBGPunchService.Api.Dto;
using BBGPunchService.Core.Model.API;
using BBGPunchService.Core.Model.TargetEntity;
using BBGPunchService.Infrastructure.Helper;
using BBGPunchService.Infrastructure.Service.Handler.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using NetTopologySuite.Geometries;
using NLog;
using System.Runtime.CompilerServices;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BBGPunchService.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private static readonly NLog.ILogger Logger = LogManager.GetCurrentClassLogger();
        public DashboardController(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }
        [HttpGet("top10-punches-weekly")]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeePunchInfo>>> GetTop10PunchesWeekly()
        {
            DateTime now = DateTime.UtcNow;
            DateTime startOfWeek = now.AddDays(-(int)now.DayOfWeek).Date; // Start of the week (Sunday)
            DateTime endOfWeek = startOfWeek.AddDays(7).AddTicks(-1); // End of the week (Saturday)
            string? cacheKey = $"top10punchesweekly_{startOfWeek.ToShortDateString()}_{endOfWeek.ToShortDateString()}";

            List<EmployeePunchInfo>? cachedData = await _cacheService.GetAsync<List<EmployeePunchInfo>>(cacheKey);

            if (cachedData == null)
            {
                List<EmployeePunchInfo>? top10Punchers = _unitOfWork.IPunchingDataRepo.GetPunchingDataQueryable()
                    .Where(p => p.PunchDateTime >= startOfWeek && p.PunchDateTime <= endOfWeek)
                    .GroupBy(p => p.EnrolNo)
                    .Select(g => new EmployeePunchInfo
                    {
                        EmployeeName = g.FirstOrDefault().FullName,
                        PunchesCount = g.Count()
                    })
                    .OrderByDescending(p => p.PunchesCount)
                    .Take(10)
                    .ToList();

                await _cacheService.SetAsync(cacheKey, top10Punchers, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return top10Punchers;
            }

            return cachedData;
        }


        [HttpGet("top10-punches-monthly")]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeePunchInfo>>> GetTop10PunchesMonthly()
        {
            DateTime now = DateTime.UtcNow;
            DateTime firstDayOfMonth = new DateTime(now.Year, now.Month, 1);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            string? cacheKey = $"top10punchesmonthly_{firstDayOfMonth.ToShortDateString()}_{lastDayOfMonth.ToShortDateString()}";
            List<EmployeePunchInfo>? cachedData = await _cacheService.GetAsync<List<EmployeePunchInfo>>(cacheKey);

            if (cachedData == null)
            {
                List<EmployeePunchInfo>? top10Punchers = _unitOfWork.IPunchingDataRepo
                    .GetPunchingDataQueryable()
                    .Where(p => p.PunchDateTime >= firstDayOfMonth && p.PunchDateTime <= lastDayOfMonth)
                    .GroupBy(p => p.EnrolNo)
                    .Select(g => new EmployeePunchInfo
                    {
                        EmployeeName = g.FirstOrDefault().FullName,
                        PunchesCount = g.Count()
                    })
                    .OrderByDescending(p => p.PunchesCount)
                    .Take(10)
                    .ToList();

                await _cacheService.SetAsync(cacheKey, top10Punchers, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return top10Punchers;
            }

            return cachedData;
        }


        [HttpGet("top10-punches-quarterly")]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeePunchInfo>>> GetTop10PunchesQuarterly()
        {
            DateTime now = DateTime.UtcNow;
            int currentQuarter = (now.Month - 1) / 3 + 1;
            DateTime firstDayOfQuarter = new DateTime(now.Year, (currentQuarter - 1) * 3 + 1, 1);
            DateTime lastDayOfQuarter = firstDayOfQuarter.AddMonths(3).AddDays(-1);
            string? cacheKey = $"top10punchesquarterly_{firstDayOfQuarter.ToShortDateString()}_{lastDayOfQuarter.ToShortDateString()}";

            List<EmployeePunchInfo>? cachedData = await _cacheService.GetAsync<List<EmployeePunchInfo>>(cacheKey);

            if (cachedData == null)
            {
                List<EmployeePunchInfo>? top10Punchers = _unitOfWork.IPunchingDataRepo.GetPunchingDataQueryable()
                    .Where(p => p.PunchDateTime >= firstDayOfQuarter && p.PunchDateTime <= lastDayOfQuarter)
                    .GroupBy(p => p.EnrolNo)
                    .Select(g => new EmployeePunchInfo
                    {
                        EmployeeName = g.FirstOrDefault().FullName,
                        PunchesCount = g.Count()
                    })
                    .OrderByDescending(p => p.PunchesCount)
                    .Take(10)
                    .ToList();

                await _cacheService.SetAsync(cacheKey, top10Punchers, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                return top10Punchers;
            }

            return cachedData;
        }


        [HttpGet("top10-punches-yearly")]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EmployeePunchInfo>>> GetTop10PunchesYearly()
        {
            DateTime now = DateTime.UtcNow;
            DateTime firstDayOfYear = new DateTime(now.Year, 1, 1);
            DateTime lastDayOfYear = new DateTime(now.Year, 12, 31);

            string cacheKey = $"top10punchesyearly_{firstDayOfYear.ToShortDateString()}_{lastDayOfYear.ToShortDateString()}";
            List<EmployeePunchInfo>? cachedData = await _cacheService.GetAsync<List<EmployeePunchInfo>>(cacheKey);

            if (cachedData != null)
            {
                return cachedData;
            }

            List<EmployeePunchInfo>? top10Punchers = await _unitOfWork.IPunchingDataRepo.GetPunchingDataQueryable()
                .Where(p => p.PunchDateTime >= firstDayOfYear && p.PunchDateTime <= lastDayOfYear)
                .GroupBy(p => p.EnrolNo)
                .Select(g => new EmployeePunchInfo
                {
                    EmployeeName = g.FirstOrDefault().FullName,
                    PunchesCount = g.Count()
                })
                .OrderByDescending(p => p.PunchesCount)
                .Take(10)
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, top10Punchers, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
            });

            return top10Punchers;
        }

        [HttpGet("top-widgets")]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetTopWidgetsAsync()
        {
            try
            {
                IQueryable<PunchingData>? punchingData = _unitOfWork.IPunchingDataRepo.GetPunchingDataQueryable();

                // Filter records based on PunchDirection property
                var summary = await punchingData
                    .Where(p => p.PunchDirection == "In" || p.PunchDirection == "Out")
                    .GroupBy(p => p.PunchDirection)
                    .Select(g => new
                    {
                        Direction = g.Key,
                        TotalPunches = g.Count(),
                        UniqueEmployees = g.Select(p => p.EnrolNo).Distinct().Count()
                    })
                    .ToListAsync();

                string? topInPunchCacheKey = "topwidgets_totalin_" + DateTime.Now.Date.ToShortDateString();
                string? topOutPunchCacheKey = "topwidgets_totalout_" + DateTime.Now.Date.ToShortDateString();
                string? totalEmployeesCacheKey = "topwidgets_totalemployees_" + DateTime.Now.Date.ToShortDateString();
                string? totalNumberOfRecordsCacheKey = "topwidgets_totalrecords_" + DateTime.Now.Date.ToShortDateString();



                //  var totalInPunch = summary.FirstOrDefault(s => s.Direction == "In")?.TotalPunches ?? 0;
                //   var totalOutPunch = summary.FirstOrDefault(s => s.Direction == "Out")?.TotalPunches ?? 0;
                //    var totalEmployees = summary.Sum(s => s.UniqueEmployees);
                //  var totalNumberOfRecords = punchingData.Count();


                int totalInPunch = await _cacheService.GetOrAddAsync(topInPunchCacheKey, async () =>
                {
                    return summary.FirstOrDefault(s => s.Direction == "In")?.TotalPunches ?? 0;
                }, TimeSpan.FromMinutes(10));

                int totalOutPunch = await _cacheService.GetOrAddAsync(topOutPunchCacheKey, async () =>
                {
                    return summary.FirstOrDefault(s => s.Direction == "Out")?.TotalPunches ?? 0;
                }, TimeSpan.FromMinutes(10));

                int totalEmployees = await _cacheService.GetOrAddAsync(totalEmployeesCacheKey, async () =>
                {
                    return summary.Sum(s => s.UniqueEmployees);
                }, TimeSpan.FromMinutes(10));

                int totalNumberOfRecords = await _cacheService.GetOrAddAsync(totalNumberOfRecordsCacheKey, async () =>
                {
                    return await punchingData.CountAsync();
                }, TimeSpan.FromMinutes(10));


                List<Widget> widgets = new List<Widget> {
            new Widget {
                icon = "faLocation",
                top = totalInPunch.ToString(),
                bottom = "Total In Punch",
                color = "green"
            },
            new Widget {
                icon = "faShop",
                top = totalOutPunch.ToString(),
                bottom = "Total Out Punch",
                color = "yellow"
            },
            new Widget {
                icon = "faBoxes",
                top = totalEmployees.ToString(),
                bottom = "Total Employees",
                color = "red"
            },
            new Widget {
                icon = "faMoneyBill",
                top = totalNumberOfRecords.ToString(),
                bottom = "Total Number Of Records",
                color = "blue"
            }
        };

                return Ok(widgets);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "An error occurred while getting top widgets.");
                return BadRequest();
            }
        }
    }

}
