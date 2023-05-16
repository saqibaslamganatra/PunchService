using Microsoft.Extensions.Configuration;

namespace BBGPunchService.Infrastructure.Configuration
{
    public static class AppSettings
    {
        public static IConfiguration? Configuration { get; set; }
        public static string? SourceConnectionString { get; set; }
        public static string? TargetConnectionString { get; set; }
        public static string? BatchSize { get; set; }
        public static string? DaysToSynch { get; set; }
        public static string? DaysToRetain { get; set; }
        public static string? StartSyncDate { get; set; }

    }
}
