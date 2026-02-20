using Disnegativos.Shared.Services;
using Disnegativos.Web.Components;
using Disnegativos.Web.Services;
using Disnegativos.Shared.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;

namespace Disnegativos
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveWebAssemblyComponents();

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            // Configuración del cliente SignalR para uso dentro de la web (Blazor Server/WASM)
            builder.Services.AddScoped(sp =>
            {
                var navigation = sp.GetRequiredService<NavigationManager>();
                return new HubConnectionBuilder()
                    .WithUrl(navigation.ToAbsoluteUri("/hubs/collaboration"))
                    .WithAutomaticReconnect()
                    .Build();
            });

            // Add device-specific services used by the Disnegativos.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Disnegativos.Shared common services (Radzen, DbContext SQLite, TimeZoneService, EventService)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=disnegativos.db";
            builder.Services.AddDisnegativosSharedServices(connectionString);

            var app = builder.Build();

            // Aplicar migraciones automáticamente
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Disnegativos.Shared.Data.DisnegativosDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
            app.UseHttpsRedirection();

            app.UseAntiforgery();

            app.MapStaticAssets();
            app.MapRazorComponents<App>()
                .AddInteractiveWebAssemblyRenderMode()
                .AddAdditionalAssemblies(
                    typeof(Disnegativos.Shared._Imports).Assembly,
                    typeof(Disnegativos.Web.Client._Imports).Assembly);

            app.MapHub<Disnegativos.Web.Hubs.CollaborationHub>("/hubs/collaboration");
            app.MapControllers();
            app.Run();
        }
    }
}
