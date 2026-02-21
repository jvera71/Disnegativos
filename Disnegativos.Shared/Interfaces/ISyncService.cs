namespace Disnegativos.Shared.Interfaces;

public interface ISyncService
{
    bool IsSyncing { get; }

    /// <summary>
    /// Sincronización completa para primera instalación o si la BD local está vacía.
    /// Descarga TODAS las entidades del servidor y las aplica a la BD local.
    /// </summary>
    Task FullSyncAsync(CancellationToken ct = default);

    /// <summary>
    /// Sincronización diferencial: descarga cambios del servidor desde el último sync.
    /// Se usa en ejecuciones normales (no primera instalación).
    /// </summary>
    Task PullServerChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Sube los cambios locales pendientes al servidor.
    /// </summary>
    Task PushPendingChangesAsync(CancellationToken ct = default);

    /// <summary>
    /// Ciclo completo: Push local + Pull remoto. Llamado periódicamente o al recuperar conexión.
    /// </summary>
    Task SyncAsync(CancellationToken ct = default);

    /// <summary>
    /// Punto de entrada al iniciar la app: decide si hacer Full Sync o Delta Sync.
    /// </summary>
    Task InitialSyncOnStartupAsync(CancellationToken ct = default);
}

public interface IConnectivityService
{
    bool IsOnline { get; }
    event EventHandler<bool>? ConnectivityChanged;
}
