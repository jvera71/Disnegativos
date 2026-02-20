using Disnegativos.Shared.Services;
using Disnegativos.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Disnegativos.Shared.DependencyInjection;

namespace Disnegativos.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Add device-specific services used by the Disnegativos.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Disnegativos.Shared common services (Radzen, DbContext SQLite, TimeZoneService, EventService)
            // Note: DbContext with SQLite in WASM requires specific setup; using placeholder if not directly used.
            builder.Services.AddDisnegativosSharedServices("Data Source=disnegativos.db");

            await builder.Build().RunAsync();
        }
    }
}
