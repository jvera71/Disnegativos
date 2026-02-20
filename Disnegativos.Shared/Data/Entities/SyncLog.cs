using System;

namespace Disnegativos.Shared.Data.Entities;

// No hereda de BaseEntity porque tiene distinta estructura para sync
public class SyncLog
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public int OperationType { get; set; } // 0=Insert, 1=Update, 2=Delete
    public string? ChangedFields { get; set; } // JSON
    public DateTime Timestamp { get; set; }
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
    public string? ConflictData { get; set; }
    public int RetryCount { get; set; }
}
