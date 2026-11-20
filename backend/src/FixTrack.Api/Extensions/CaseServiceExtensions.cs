using FixTrack.Application.Cases;
using FixTrack.Infrastructure.Cases;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace FixTrack.Api.Extensions;

public static class CaseServiceExtensions
{
    // OWNER: Rashad - feature/cases-core
    // In-memory repository (singleton, so data lives for the whole process and resets on restart)
    // and the case service (scoped, because it depends on the current requester).
    public static IServiceCollection AddCaseServices(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddSingleton<ICaseRepository, InMemoryCaseRepository>();
        services.AddScoped<ICaseService, CaseService>();

        return services;
    }
}
