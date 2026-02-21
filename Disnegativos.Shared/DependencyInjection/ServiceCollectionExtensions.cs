using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Radzen;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Services.Interfaces;
using Disnegativos.Shared.Services.Implementations;

namespace Disnegativos.Shared.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDisnegativosSharedServices(
        this IServiceCollection services,
        string connectionString,
        string? hubUrl = null,
        bool isServer = false)   // true → Web, false → MAUI
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

        // 5. ICountryService
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IRefereeService, RefereeService>();
        services.AddScoped<ICompetitionService, CompetitionService>();
        services.AddScoped<IOrganizationService, OrganizationService>();
        services.AddScoped<ITeamService, TeamService>();
        services.AddScoped<IPlayerService, PlayerService>();

        // 7. Localization Services
        services.AddLocalization();

        // 6. HubConnection Client (SignalR)
        services.AddScoped(sp =>
        {
            var hubBuilder = new HubConnectionBuilder()
                .WithAutomaticReconnect();

            if (!string.IsNullOrEmpty(hubUrl))
            {
                hubBuilder.WithUrl(hubUrl);
            }
            else
            {
                var navigation = sp.GetRequiredService<NavigationManager>();
                hubBuilder.WithUrl(navigation.ToAbsoluteUri("/hubs/collaboration"));
            }

            return hubBuilder.Build();
        });

        return services;
    }
}
