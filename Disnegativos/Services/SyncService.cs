using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Interfaces;
using Microsoft.Extensions.Logging;
using Dotmim.Sync;
using Dotmim.Sync.Sqlite;
using Dotmim.Sync.Web.Client;
using Dotmim.Sync.Enumerations;

namespace Disnegativos.Services;

public class SyncService : ISyncService
{
    private readonly IDbContextFactory<DisnegativosDbContext> _dbFactory;
    private readonly HttpClient _httpClient;
    private readonly IConnectivityService _connectivity;
    private readonly ILogger<SyncService> _logger;
    private readonly string _localConnectionString;

    public bool IsSyncing { get; private set; }

    public SyncService(
        IDbContextFactory<DisnegativosDbContext> dbFactory,
        HttpClient httpClient,
        IConnectivityService connectivity,
        ILogger<SyncService> logger)
    {
        _dbFactory = dbFactory;
        _httpClient = httpClient;
        _connectivity = connectivity;
        _logger = logger;
        
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
        _localConnectionString = $"Data Source={dbPath}";
    }

    public async Task InitialSyncOnStartupAsync(CancellationToken ct = default)
    {
        if (!_connectivity.IsOnline)
        {
            _logger.LogInformation("InitialSync: sin conexión, usando datos locales.");
            return;
        }

        _logger.LogInformation("InitialSync: ejecutando sincronización inicial...");
        await SyncAsync(ct);
    }

    public async Task SyncAsync(CancellationToken ct = default)
    {
        if (IsSyncing || !_connectivity.IsOnline) return;

        try
        {
            IsSyncing = true;
            _logger.LogInformation("Sync: Iniciando sincronización con Dotmim.Sync...");

            // Definir proveedores
            var clientProvider = new SqliteSyncProvider(_localConnectionString);
            
            // Orquestador web apuntando al controlador SyncController del servidor
            var syncUrl = new Uri(_httpClient.BaseAddress!, "api/sync").ToString();
            var proxyClientProvider = new WebRemoteOrchestrator(syncUrl);

            // Agente de sincronización
            var agent = new SyncAgent(clientProvider, proxyClientProvider);

            // Ejecutar sincronización
            // Note: Usamos la versión sin parámetros por ahora para asegurar compilación
            // e investigamos por qué las propiedades no se encuentran.
            var result = await agent.SynchronizeAsync();

            _logger.LogInformation("Sync completado con éxito.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error durante la sincronización con Dotmim.Sync");
        }
        finally
        {
            IsSyncing = false;
        }
    }

    public Task FullSyncAsync(CancellationToken ct = default) => SyncAsync(ct);
    public Task PullServerChangesAsync(CancellationToken ct = default) => SyncAsync(ct);
    public Task PushPendingChangesAsync(CancellationToken ct = default) => SyncAsync(ct);
}
