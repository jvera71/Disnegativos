using System;
using Disnegativos.Shared.Enums;

namespace Disnegativos.Shared.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public Guid? ServerId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid? ArchivedByUserId { get; set; }
    public SyncStatus SyncStatus { get; set; }
}
