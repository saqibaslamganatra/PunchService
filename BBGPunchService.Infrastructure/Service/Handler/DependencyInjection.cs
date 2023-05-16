using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using BBGPunchService.Infrastructure.Service.Handler.Interface;
using BBGPunchService.Infrastructure.Service.Handler.Implementation;
using BBGPunchService.Infrastructure.Service.Repository.Interface;
using BBGPunchService.Infrastructure.Service.Repository.Implementation;
using Microsoft.Extensions.Caching.Distributed;

namespace BBGPunchService.Infrastructure.Service.Handler
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDIService(this IServiceCollection services, IConfiguration configuration)
        {

            // services.AddSingleton<ILogger, Logger<>>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IPunchingDataRepository, PunchingDataRepository>();
            services.AddSingleton<ICacheService,CacheService>();

            return services;
        }
    }
}
