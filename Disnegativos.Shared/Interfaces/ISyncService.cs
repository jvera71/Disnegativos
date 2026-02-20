using Disnegativos.Shared.DTOs.Sync;

namespace Disnegativos.Shared.Interfaces;

public interface ISyncService
{
    bool IsSyncing { get; }
    Task PushPendingChangesAsync(CancellationToken ct = default);
    Task PullServerChangesAsync(CancellationToken ct = default);
    Task SyncAsync(CancellationToken ct = default);
}

public interface IConnectivityService
{
    bool IsOnline { get; }
    event EventHandler<bool>? ConnectivityChanged;
}
