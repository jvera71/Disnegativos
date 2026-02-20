using Disnegativos.Shared.Services;
using Disnegativos.Web.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace Disnegativos.Web.Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            // Add device-specific services used by the Disnegativos.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            await builder.Build().RunAsync();
        }
    }
}
