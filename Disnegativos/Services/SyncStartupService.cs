using Disnegativos.Shared.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Disnegativos.Services;

/// <summary>
/// Servicio de arranque que ejecuta la sincronización inicial en background.
/// Se lanza al iniciar la aplicación MAUI sin bloquear la carga de la UI.
/// Implementa ISyncStatusNotifier para que los componentes Blazor del Shared
/// puedan recibir el estado sin dependencia circular.
/// </summary>
public class SyncStartupService : IHostedService, ISyncStatusNotifier
{
    private readonly ISyncService _syncService;
    private readonly ILogger<SyncStartupService> _logger;

    public event EventHandler<SyncStatusEventArgs>? StatusChanged;

    public SyncStartupService(ISyncService syncService, ILogger<SyncStartupService> logger)
    {
        _syncService = syncService;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                Notify("Verificando sincronización con servidor...", false);
                await _syncService.InitialSyncOnStartupAsync(cancellationToken);
                Notify("Sincronización completada ✓", true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en SyncStartupService");
                Notify("Error al sincronizar. Usando datos locales.", true);
            }
        }, cancellationToken);

        await Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

    private void Notify(string message, bool completed)
        => StatusChanged?.Invoke(this, new SyncStatusEventArgs(message, completed));
}

