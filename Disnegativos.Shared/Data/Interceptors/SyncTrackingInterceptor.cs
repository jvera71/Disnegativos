using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.Models;
using Disnegativos.Shared.Enums;

namespace Disnegativos.Shared.Data.Interceptors;

public class SyncTrackingInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context;
        if (context == null) return base.SavingChangesAsync(eventData, result, ct);

        // Evitar bucles infinitos si estamos guardando el propio SyncLog
        var entries = context.ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            .ToList();

        if (!entries.Any()) return base.SavingChangesAsync(eventData, result, ct);

        foreach (var entry in entries)
        {
            var syncLog = new SyncLog
            {
                Id = Guid.NewGuid(),
                TableName = entry.Metadata.GetTableName() ?? entry.Entity.GetType().Name,
                RecordId = entry.Entity.Id,
                OperationType = MapState(entry.State),
                Timestamp = DateTime.UtcNow,
                IsSynced = false,
                ChangedFields = SerializeChanges(entry)
            };

            context.Set<SyncLog>().Add(syncLog);
            
            // Actualizar el estado de la entidad
            entry.Entity.SyncStatus = SyncStatus.Pending;
            entry.Entity.UpdatedAt = DateTime.UtcNow;
        }

        return base.SavingChangesAsync(eventData, result, ct);
    }

    private static SyncOperation MapState(EntityState state) => state switch
    {
        EntityState.Added => SyncOperation.Insert,
        EntityState.Modified => SyncOperation.Update,
        EntityState.Deleted => SyncOperation.Delete,
        _ => throw new ArgumentOutOfRangeException(nameof(state))
    };

    private static string SerializeChanges(Microsoft.EntityFrameworkCore.ChangeTracking.EntityEntry entry)
    {
        if (entry.State == EntityState.Deleted) return string.Empty;

        var values = new Dictionary<string, object?>();
        foreach (var property in entry.Properties)
        {
            if (entry.State == EntityState.Added || property.IsModified)
            {
                values[property.Metadata.Name] = property.CurrentValue;
            }
        }

        return JsonSerializer.Serialize(values);
    }
}
