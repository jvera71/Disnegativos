namespace Disnegativos.Shared.Hubs;

/// <summary>
/// Métodos que el servidor puede invocar en los clientes.
/// </summary>
public interface ICollaborationHubClient
{
    /// <summary>
    /// Notifica que una entidad ha sido creada, modificada o eliminada.
    /// </summary>
    Task EntityChanged(string entityType, Guid entityId, string changeType);

    /// <summary>
    /// Notifica que un usuario ha empezado a editar un registro.
    /// </summary>
    Task UserEditingEntity(string entityType, Guid entityId, string userName);

    /// <summary>
    /// Notifica que un usuario ha dejado de editar un registro.
    /// </summary>
    Task UserStoppedEditing(string entityType, Guid entityId, string userName);

    /// <summary>
    /// Notifica una actualización en un análisis en vivo (acciones, timer, etc.)
    /// </summary>
    Task LiveAnalysisUpdate(Guid analysisId, string updateType, string jsonPayload);
}

/// <summary>
/// Métodos que los clientes pueden invocar en el servidor.
/// </summary>
public interface ICollaborationHubServer
{
    Task NotifyEntityChanged(string entityType, Guid entityId, string changeType);
    Task StartEditing(string entityType, Guid entityId);
    Task StopEditing(string entityType, Guid entityId);
    Task JoinAnalysisRoom(Guid analysisId);
    Task LeaveAnalysisRoom(Guid analysisId);
    Task SendLiveAnalysisUpdate(Guid analysisId, string updateType, string jsonPayload);
}
