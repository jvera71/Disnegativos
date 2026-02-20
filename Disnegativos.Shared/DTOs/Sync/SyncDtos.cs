using Disnegativos.Shared.Enums;

namespace Disnegativos.Shared.DTOs.Sync;

public record SyncItemDto(
    Guid SyncLogId,
    string TableName,
    Guid RecordId,
    SyncOperation OperationType,
    string? ChangedFields,
    DateTime Timestamp
);

public record SyncBatchDto(
    List<SyncItemDto> Changes
);

public record SyncItemResultDto(
    Guid SyncLogId,
    bool Success,
    string? ConflictDetails = null
);

public record SyncBatchResultDto(
    List<SyncItemResultDto> Results
);

public record SyncPullResultDto(
    List<SyncItemDto> Changes
);
