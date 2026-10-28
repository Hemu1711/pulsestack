using Microsoft.Extensions.DependencyInjection;
using PulseTrack.Application.Interfaces;
using PulseTrack.Application.Services;
using PulseTrack.Infrastructure.Cache;
using PulseTrack.Infrastructure.Repositories;

namespace PulseTrack.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddMemoryCache();
        services.AddSingleton<ICacheService, MemoryDistributedCacheService>();
        services.AddSingleton<IWorkOrderRepository, DapperWorkOrderRepository>();
        services.AddSingleton<IPartsCatalogRepository, DapperPartsCatalogRepository>();
        services.AddScoped<WorkOrderService>();
        services.AddScoped<PartsCatalogService>();

        return services;
    }
}
