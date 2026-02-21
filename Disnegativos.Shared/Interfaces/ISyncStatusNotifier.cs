namespace Disnegativos.Shared.Interfaces;

/// <summary>
/// Permite a los componentes Blazor del Shared suscribirse al progreso
/// de sincronización inicial sin depender del proyecto MAUI directamente.
/// </summary>
public interface ISyncStatusNotifier
{
    event EventHandler<SyncStatusEventArgs>? StatusChanged;
}

public record SyncStatusEventArgs(string Message, bool Completed);
