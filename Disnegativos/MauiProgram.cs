using Disnegativos.Services;
using Disnegativos.Shared.Services;
using Microsoft.Extensions.Logging;
using Disnegativos.Shared.DependencyInjection;
using Disnegativos.Shared.Data.Interceptors;
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

            // Disnegativos.Shared common services (Radzen, DbContext SQLite, TimeZoneService, EventService)
            var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
            builder.Services.AddDisnegativosSharedServices($"Data Source={dbPath}");

            // Sobrescribir registro de DbContext para añadir interceptor (solo en MAUI)
            builder.Services.AddDbContextFactory<Disnegativos.Shared.Data.DisnegativosDbContext>((sp, options) =>
            {
                options.UseSqlite($"Data Source={dbPath}");
                options.AddInterceptors(new SyncTrackingInterceptor());
            });

            // Servicios específicos de MAUI (conectividad y sync)
            builder.Services.AddSingleton(Microsoft.Maui.Networking.Connectivity.Current);
            builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
            
            // HttpClient para el SyncService (ajústese la URL base según sea necesario)
            var serverUrl = "http://localhost:5000/";
            builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serverUrl) });
            builder.Services.AddScoped<ISyncService, SyncService>();

            // Configuración del cliente SignalR (MAUI)
            builder.Services.AddSingleton(sp =>
            {
                return new HubConnectionBuilder()
                    .WithUrl($"{serverUrl}hubs/collaboration")
                    .WithAutomaticReconnect()
                    .Build();
            });

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
