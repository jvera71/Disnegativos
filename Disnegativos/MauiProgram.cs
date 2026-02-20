using Disnegativos.Services;
using Disnegativos.Shared.Services;
using Microsoft.Extensions.Logging;
using Disnegativos.Shared.DependencyInjection;

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

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
