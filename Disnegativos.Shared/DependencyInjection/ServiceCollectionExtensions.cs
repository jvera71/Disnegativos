using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;
using Disnegativos.Shared.Services.Implementations;

namespace Disnegativos.Shared.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDisnegativosSharedServices(this IServiceCollection services, string connectionString)
    {
        // 1. Radzen Components
        services.AddRadzenComponents();

        // 2. DisnegativosDbContext (SQLite)
        services.AddDbContextFactory<DisnegativosDbContext>((sp, options) =>
            options.UseSqlite(connectionString));

        // Registrar también el DbContext normal para inyección directa
        services.AddScoped(sp => sp.GetRequiredService<IDbContextFactory<DisnegativosDbContext>>().CreateDbContext());

        // 3. ITimeZoneService
        services.AddScoped<ITimeZoneService, TimeZoneService>();

        // 4. IEventService
        services.AddScoped<IEventService, EventService>();

        return services;
    }
}
