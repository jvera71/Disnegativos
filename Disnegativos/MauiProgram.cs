using Disnegativos.Services;
using Disnegativos.Shared.Services;
using Microsoft.Extensions.Logging;
using Disnegativos.Shared.DependencyInjection;
using Disnegativos.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR.Client;

namespace Disnegativos
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            // Add device-specific services used by the Disnegativos.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Disnegativos.Shared common services (Radzen, DbContext SQLite, TimeZoneService, EventService, HubConnection)
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
            var serverUrl = "http://localhost:5000/";
            builder.Services.AddDisnegativosSharedServices($"Data Source={dbPath}", $"{serverUrl}hubs/collaboration");

            builder.Services.AddDbContextFactory<Disnegativos.Shared.Data.DisnegativosDbContext>((sp, options) =>
            {
                options.UseSqlite($"Data Source={dbPath}");
            });

            // Servicios específicos de MAUI (conectividad y sync)
            builder.Services.AddSingleton(Microsoft.Maui.Networking.Connectivity.Current);
            builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
            
            // HttpClient para el SyncService (ajústese la URL base según sea necesario)
            builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(serverUrl) });

            // SyncService como Singleton para que lo pueda consumir el HostedService
            builder.Services.AddSingleton<ISyncService, SyncService>();

            // SyncStartupService: sincronización inicial al arrancar + notificador de estado para la UI
            builder.Services.AddSingleton<SyncStartupService>();
            builder.Services.AddSingleton<ISyncStatusNotifier>(sp => sp.GetRequiredService<SyncStartupService>());
            builder.Services.AddHostedService(sp => sp.GetRequiredService<SyncStartupService>());

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            // Aplicar migraciones automáticamente
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Disnegativos.Shared.Data.DisnegativosDbContext>();
                dbContext.Database.Migrate();
            }

            return app;
        }
    }
}
