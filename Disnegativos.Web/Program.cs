using Disnegativos.Shared.Services;
using Disnegativos.Web.Components;
using Disnegativos.Web.Services;
using Disnegativos.Shared.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.AspNetCore.Components;
using Dotmim.Sync.Sqlite;
using Disnegativos.Shared.Data;


namespace Disnegativos
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorComponents()
                .AddInteractiveServerComponents();

            builder.Services.AddControllers();
            builder.Services.AddSignalR();

            // Add device-specific services used by the Disnegativos.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            // Disnegativos.Shared common services (Radzen, DbContext SQLite, TimeZoneService, EventService, HubConnection)
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=disnegativos.db";
            builder.Services.AddDisnegativosSharedServices(connectionString, isServer: true);

            // Configure Dotmim.Sync
            var setup = SyncSetupProvider.GetSyncSetup();
            builder.Services.AddSyncServer<SqliteSyncProvider>(connectionString, setup);


            var app = builder.Build();

            // Aplicar migraciones automáticamente
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<Disnegativos.Shared.Data.DisnegativosDbContext>();
                dbContext.Database.Migrate();

#if DEBUG
                // Poblar con datos de prueba si la BD está vacía (solo DEBUG)
                await Disnegativos.Web.Data.DatabaseSeeder.SeedIfEmptyAsync(dbContext);
#endif
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
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
                .AddInteractiveServerRenderMode()
                .AddAdditionalAssemblies(
                    typeof(Disnegativos.Shared._Imports).Assembly);

            app.MapHub<Disnegativos.Web.Hubs.CollaborationHub>("/hubs/collaboration");
            app.MapControllers();
            app.Run();
        }
    }
}
