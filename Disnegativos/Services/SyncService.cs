using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.DTOs.Sync;
using Disnegativos.Shared.Interfaces;
using Microsoft.Extensions.Logging;

namespace Disnegativos.Services;

public class SyncService : ISyncService
{
    private readonly IDbContextFactory<DisnegativosDbContext> _dbFactory;
    private readonly HttpClient _httpClient;
    private readonly IConnectivityService _connectivity;
    private readonly ILogger<SyncService> _logger;

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
    }

    public async Task SyncAsync(CancellationToken ct = default)
    {
        if (IsSyncing || !_connectivity.IsOnline) return;

        try
        {
            IsSyncing = true;
            await PushPendingChangesAsync(ct);
            await PullServerChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during synchronization");
        }
        finally
        {
            IsSyncing = false;
        }
    }

    public async Task PushPendingChangesAsync(CancellationToken ct = default)
    {
        using var db = _dbFactory.CreateDbContext();
        
        var pending = await db.SyncLogs
            .Where(s => !s.IsSynced)
            .OrderBy(s => s.Timestamp)
            .Take(50)
            .ToListAsync(ct);

        if (!pending.Any()) return;

        var batch = new SyncBatchDto(pending.Select(s => new SyncItemDto(
            s.Id,
            s.TableName,
            s.RecordId,
            s.OperationType,
            s.ChangedFields,
            s.Timestamp
        )).ToList());

        var response = await _httpClient.PostAsJsonAsync("api/sync/push", batch, ct);
        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadFromJsonAsync<SyncBatchResultDto>(ct);
            if (result != null)
            {
                foreach (var itemResult in result.Results)
                {
                    var log = pending.FirstOrDefault(p => p.Id == itemResult.SyncLogId);
                    if (log != null && itemResult.Success)
                    {
                        log.IsSynced = true;
                        log.SyncedAt = DateTime.UtcNow;
                    }
                    else if (log != null)
                    {
                        log.ConflictData = itemResult.ConflictDetails;
                        log.RetryCount++;
                    }
                }
                await db.SaveChangesAsync(ct);
            }
        }
    }

    public async Task PullServerChangesAsync(CancellationToken ct = default)
    {
        // Implementación básica de Pull por tiempo (se ampliaría en el futuro)
        // Por ahora se deja como placeholder para completar el patrón.
        await Task.CompletedTask;
    }
}
