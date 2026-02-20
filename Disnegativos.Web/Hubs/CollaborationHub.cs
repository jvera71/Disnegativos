using Microsoft.AspNetCore.SignalR;
using Disnegativos.Shared.Hubs;
using System.Collections.Concurrent;

namespace Disnegativos.Web.Hubs;

public class CollaborationHub : Hub<ICollaborationHubClient>, ICollaborationHubServer
{
    private static readonly ConcurrentDictionary<string, string> _editingLocks = new();

    public async Task NotifyEntityChanged(string entityType, Guid entityId, string changeType)
    {
        // Notificar a todos los clientes excepto el que realizó el cambio
        await Clients.Others.EntityChanged(entityType, entityId, changeType);
    }

    public async Task StartEditing(string entityType, Guid entityId)
    {
        var key = $"{entityType}:{entityId}";
        var userName = Context.User?.Identity?.Name ?? Context.ConnectionId;
        _editingLocks[key] = userName;

        await Clients.Others.UserEditingEntity(entityType, entityId, userName);
    }

    public async Task StopEditing(string entityType, Guid entityId)
    {
        var key = $"{entityType}:{entityId}";
        var userName = Context.User?.Identity?.Name ?? Context.ConnectionId;
        _editingLocks.TryRemove(key, out _);

        await Clients.Others.UserStoppedEditing(entityType, entityId, userName);
    }

    public async Task JoinAnalysisRoom(Guid analysisId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"analysis:{analysisId}");
    }

    public async Task LeaveAnalysisRoom(Guid analysisId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"analysis:{analysisId}");
    }

    public async Task SendLiveAnalysisUpdate(Guid analysisId, string updateType, string jsonPayload)
    {
        await Clients.OthersInGroup($"analysis:{analysisId}")
            .LiveAnalysisUpdate(analysisId, updateType, jsonPayload);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        // Limpiar bloqueos de edición del usuario desconectado
        var keysToRemove = _editingLocks
            .Where(kv => kv.Value == Context.ConnectionId)
            .Select(kv => kv.Key)
            .ToList();

        foreach (var key in keysToRemove)
        {
            if (_editingLocks.TryRemove(key, out _))
            {
                var parts = key.Split(':');
                if (parts.Length == 2 && Guid.TryParse(parts[1], out var entityId))
                {
                    await Clients.All.UserStoppedEditing(parts[0], entityId, Context.ConnectionId);
                }
            }
        }

        await base.OnDisconnectedAsync(exception);
    }
}
