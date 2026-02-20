using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs.Sync;
using Disnegativos.Shared.Enums;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Metadata;

using Disnegativos.Web.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Disnegativos.Web.Controllers;

[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly DisnegativosDbContext _db;
    private readonly IHubContext<CollaborationHub> _hubContext;

    public SyncController(DisnegativosDbContext db, IHubContext<CollaborationHub> hubContext)
    {
        _db = db;
        _hubContext = hubContext;
    }

    [HttpPost("push")]
    public async Task<ActionResult<SyncBatchResultDto>> Push(SyncBatchDto batch)
    {
        var results = new List<SyncItemResultDto>();

        foreach (var change in batch.Changes)
        {
            try
            {
                await ApplyChangeToServerDb(change);
                results.Add(new SyncItemResultDto(change.SyncLogId, true));

                // Notificar a todos los clientes (Web y MAUI) en tiempo real
                await _hubContext.Clients.All.SendAsync("EntityChanged", change.TableName, change.RecordId, change.OperationType.ToString());
            }
            catch (Exception ex)
            {
                results.Add(new SyncItemResultDto(change.SyncLogId, false, ex.Message));
            }
        }

        return Ok(new SyncBatchResultDto(results));
    }

    private async Task ApplyChangeToServerDb(SyncItemDto change)
    {
        var entityType = _db.Model.GetEntityTypes()
            .FirstOrDefault(t => t.GetTableName() == change.TableName || t.ClrType.Name == change.TableName);

        if (entityType == null) throw new Exception($"Table {change.TableName} not found in model.");

        var clrType = entityType.ClrType;
        
        if (change.OperationType == SyncOperation.Insert || change.OperationType == SyncOperation.Update)
        {
            var entity = await _db.FindAsync(clrType, change.RecordId);
            bool isNew = entity == null;

            if (isNew)
            {
                entity = Activator.CreateInstance(clrType);
                if (entity == null) throw new Exception($"Could not create instance of {clrType.Name}");
                
                // Establecer ID manualmente ya que viene del cliente
                var idProperty = clrType.GetProperty("Id");
                idProperty?.SetValue(entity, change.RecordId);
            }

            if (change.ChangedFields != null)
            {
                var values = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(change.ChangedFields);
                if (values != null)
                {
                    foreach (var kvp in values)
                    {
                        var property = clrType.GetProperty(kvp.Key);
                        if (property != null && property.CanWrite)
                        {
                            var value = JsonSerializer.Deserialize(kvp.Value.GetRawText(), property.PropertyType);
                            property.SetValue(entity, value);
                        }
                    }
                }
            }

            if (isNew) _db.Add(entity!);
            else _db.Update(entity!);
        }
        else if (change.OperationType == SyncOperation.Delete)
        {
            var entity = await _db.FindAsync(clrType, change.RecordId);
            if (entity != null)
            {
                _db.Remove(entity);
            }
        }

        await _db.SaveChangesAsync();
    }

    [HttpGet("pull")]
    public async Task<ActionResult<SyncPullResultDto>> Pull([FromQuery] DateTime since)
    {
        // En una implementación real, tendríamos un log de cambios en el servidor también.
        // Para este MVP, devolvemos una lista vacía o podrías implementar un log similar para Pull.
        return Ok(new SyncPullResultDto(new List<SyncItemDto>()));
    }
}
