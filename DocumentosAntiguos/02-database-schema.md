# Disnegativos — Modelo de Datos (EF Core Code-First)

## 1. Enfoque Code-First

Las **entidades C#** son la fuente de verdad del modelo de datos. EF Core genera y aplica las migraciones SQLite automáticamente. No se escribe SQL manualmente.

### Flujo de trabajo

```
Entidad C# (POCO) → Fluent API Config → EF Migration → SQLite Schema
```

```bash
# Crear/actualizar migración
dotnet ef migrations add NombreMigracion --project Disnegativos.Shared

# Aplicar migración (se ejecuta automáticamente al iniciar en dev)
dotnet ef database update --project Disnegativos.Shared
```

### Aplicación automática de migraciones

```csharp
// En startup (tanto MAUI como Web)
using var scope = app.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<DisnegativosDbContext>();
await db.Database.MigrateAsync(); // Aplica migraciones pendientes
```

## 2. Convenciones Generales

| Regla | Detalle |
|---|---|
| **PK** | Todas las tablas: `Id` de tipo `Guid` |
| **Nombres** | PascalCase, en inglés, singular |
| **Soft Delete** | `IsArchived` (bool), `ArchivedAt` (DateTime?), `ArchivedByUserId` (Guid?) |
| **Auditoría** | `CreatedAt`, `UpdatedAt` (DateTime UTC) |
| **Sincronización** | `SyncStatus` (enum: Synced/Pending/Conflict), `ServerId` (Guid?) |
| **FK** | Nombre: `{EntidadRelacionada}Id` |
| **Índices** | En todas las FKs y campos de búsqueda frecuente |
| **Fechas** | Siempre UTC en la base de datos |
| **Proveedor** | `Microsoft.EntityFrameworkCore.Sqlite` (v9.0.0) |

## 3. Entidad Base y Configuración Fluent API

Todas las entidades heredan de `BaseEntity` (ubicada en `Disnegativos.Shared.Models`):

```csharp
// Disnegativos.Shared/Models/BaseEntity.cs
using Disnegativos.Shared.Enums;

namespace Disnegativos.Shared.Models;

public abstract class BaseEntity
{
    public Guid Id { get; set; }                  // PK
    public Guid? ServerId { get; set; }            // ID en el servidor (null si no sincronizado)
    public DateTime CreatedAt { get; set; }        // UTC
    public DateTime UpdatedAt { get; set; }        // UTC
    public bool IsArchived { get; set; }
    public DateTime? ArchivedAt { get; set; }
    public Guid? ArchivedByUserId { get; set; }
    public SyncStatus SyncStatus { get; set; }     // Synced, Pending, Conflict
}
```

```csharp
// Disnegativos.Shared/Enums/SyncStatus.cs
namespace Disnegativos.Shared.Enums;

public enum SyncStatus
{
    Synced = 0,
    Pending = 1,
    Conflict = 2
}
```

### Configuración base con Fluent API

```csharp
// Disnegativos.Shared/Data/Configurations/BaseEntityConfiguration.cs
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Configurations;

public abstract class BaseEntityConfiguration<T> : IEntityTypeConfiguration<T>
    where T : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        builder.HasKey(e => e.Id);
        builder.Property(e => e.Id).ValueGeneratedNever(); // Guid generado en código
        builder.Property(e => e.CreatedAt).IsRequired();
        builder.Property(e => e.UpdatedAt).IsRequired();
        builder.HasIndex(e => e.IsArchived);
        builder.HasIndex(e => e.SyncStatus);
        builder.HasQueryFilter(e => !e.IsArchived); // Soft-delete global filter
    }
}
```

### DbContext

```csharp
// Disnegativos.Shared/Data/DisnegativosDbContext.cs
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Data.Entities;
using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data;

public class DisnegativosDbContext : DbContext
{
    // Organización y Tenancy
    public DbSet<Tenant> Tenants => Set<Tenant>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<SubCustomer> SubCustomers => Set<SubCustomer>();
    public DbSet<Country> Countries => Set<Country>();
    public DbSet<ServicePlan> ServicePlans => Set<ServicePlan>();
    public DbSet<User> Users => Set<User>();

    // Core Domain
    public DbSet<Team> Teams => Set<Team>();
    public DbSet<Competition> Competitions => Set<Competition>();
    public DbSet<Event> Events => Set<Event>();

    // Tools
    public DbSet<SyncLog> SyncLogs => Set<SyncLog>();

    public DisnegativosDbContext(DbContextOptions<DisnegativosDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Aplica todas las configuraciones Fluent API del assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DisnegativosDbContext).Assembly);
    }

    public override Task<int> SaveChangesAsync(CancellationToken ct = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.Id = entry.Entity.Id == Guid.Empty
                        ? Guid.NewGuid() : entry.Entity.Id;
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;

                // ── SOFT DELETE ──────────────────────────────────
                // Intercepta cualquier Delete y lo convierte en Update
                // con IsArchived = true. El registro NUNCA se borra físicamente.
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsArchived = true;
                    entry.Entity.ArchivedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return base.SaveChangesAsync(ct);
    }
}
```

### Resumen: Soft Delete — las 3 piezas

| Pieza | Dónde | Qué hace |
|---|---|---|
| **1. Campos** | `BaseEntity` (`Disnegativos.Shared.Models`) | `IsArchived`, `ArchivedAt`, `ArchivedByUserId` en todas las entidades |
| **2. Query Filter** | `BaseEntityConfiguration<T>` (`Disnegativos.Shared.Data.Configurations`) | `HasQueryFilter(e => !e.IsArchived)` — las consultas EF ignoran automáticamente los registros archivados |
| **3. Interceptor de Delete** | `DisnegativosDbContext.SaveChangesAsync` (`Disnegativos.Shared.Data`) | Convierte `EntityState.Deleted` → `EntityState.Modified` + `IsArchived = true` |

### Uso en servicios

```csharp
// Borrar (soft) — simplemente llamar a Remove, el DbContext lo convierte en soft-delete
public async Task DeleteEventAsync(Guid id)
{
    var dbEvent = await _context.Events.FindAsync(id);
    if (dbEvent != null)
    {
        _context.Events.Remove(dbEvent); // El SaveChanges lo convierte en soft-delete
        await _context.SaveChangesAsync();
    }
}

// Consultar — los archivados se excluyen automáticamente por el QueryFilter
public async Task<List<EventDto>> GetAllEventsAsync()
    => // ... Solo devuelve IsArchived == false

// Consultar INCLUYENDO archivados (cuando sea necesario)
public async Task<List<Event>> GetAllIncludingArchivedAsync()
    => await _context.Events.IgnoreQueryFilters().ToListAsync();

// Restaurar un registro borrado
public async Task RestoreAsync(Guid id)
{
    var entity = await _context.Events
        .IgnoreQueryFilters()
        .FirstOrDefaultAsync(e => e.Id == id)
        ?? throw new InvalidOperationException("Event not found");
    entity.IsArchived = false;
    entity.ArchivedAt = null;
    entity.ArchivedByUserId = null;
    await _context.SaveChangesAsync();
}
```

### Repositorio Genérico

```csharp
// Disnegativos.Shared/Data/Repositories/Repository.cs
using Microsoft.EntityFrameworkCore;
using Disnegativos.Shared.Interfaces;
using Disnegativos.Shared.Models;

namespace Disnegativos.Shared.Data.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly DisnegativosDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(DisnegativosDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
        => await _dbSet.FindAsync(id);

    public virtual async Task<List<T>> GetAllAsync()
        => await _dbSet.ToListAsync();

    public virtual async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate)
        => await _dbSet.Where(predicate).ToListAsync();

    public virtual async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _dbSet.Remove(entity); // El interceptor se encargará del soft-delete
            await _context.SaveChangesAsync();
        }
    }
}
```

### Registro del DbContext

```csharp
// Disnegativos.Shared/DependencyInjection/ServiceCollectionExtensions.cs
services.AddDbContext<DisnegativosDbContext>(options =>
    options.UseSqlite(connectionString));
```

## 4. Diagrama Entidad-Relación (Simplificado)

```
┌──────────┐     ┌──────────────┐     ┌─────────────┐
│  Tenant  │───<│   Customer   │───<│ SubCustomer  │
└──────────┘     └──────────────┘     └─────────────┘
                       │
                 ┌─────┴──────┐
                 │ServicePlan │
                 └────────────┘
                       │
        ┌──────────────┼───────────────┐
        │              │               │
  ┌─────┴─────┐ ┌─────┴──────┐ ┌──────┴──────┐
  │   User    │ │SportDisc.  │ │ Competition │
  └───────────┘ └────────────┘ └─────────────┘
                      │               │
                ┌─────┴──────┐  ┌─────┴─────┐
                │  Category  │  │   Event    │
                └────────────┘  └───────────┘
                                      │
                               ┌──────┼──────┐
                               │      │      │
                        ┌──────┴┐ ┌───┴────┐ ┌┴────────┐
                        │Analysis│ │ Team   │ │ Entity  │
                        └───────┘ └────────┘ └─────────┘
                            │         │
                     ┌──────┴──┐  ┌───┴───┐
                     │ Action  │  │Player │
                     └─────────┘  └───────┘
                            │
                     ┌──────┴──────┐
                     │   Report    │
                     └─────────────┘
                            │
                     ┌──────┴──┐
                     │  Slide  │
                     └─────────┘
```

## 5. Entidades del Dominio

> **Nota:** Todas las entidades se ubican en el namespace `Disnegativos.Shared.Data.Entities` y heredan de `Disnegativos.Shared.Models.BaseEntity`. Las tablas SQL a continuación representan el esquema que EF Core generará. La fuente de verdad son las clases POCO + Fluent API.

### 5.1 Organización y Tenancy

#### Tenant

```csharp
// Disnegativos.Shared/Data/Entities/Tenant.cs
namespace Disnegativos.Shared.Data.Entities;

public class Tenant : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public ICollection<Customer> Customers { get; set; } = [];
}
```

```sql
CREATE TABLE Tenant (
    Id              TEXT PRIMARY KEY,   -- Guid
    Name            TEXT NOT NULL,
    IsActive        INTEGER NOT NULL DEFAULT 1,
    -- Campos de BaseEntity
    ServerId        TEXT,
    CreatedAt       TEXT NOT NULL,
    UpdatedAt       TEXT NOT NULL,
    IsArchived      INTEGER NOT NULL DEFAULT 0,
    ArchivedAt      TEXT,
    ArchivedByUserId TEXT,
    SyncStatus      INTEGER NOT NULL DEFAULT 0
);
```

#### Customer

```csharp
// Disnegativos.Shared/Data/Entities/Customer.cs
namespace Disnegativos.Shared.Data.Entities;

public class Customer : BaseEntity
{
    public Guid TenantId { get; set; }
    public Tenant Tenant { get; set; } = null!;

    public string Name { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? FiscalAddress { get; set; }
    public string? ExtendedAddress { get; set; }
    public Guid? CountryId { get; set; }
    public Country? Country { get; set; }
    public string? Province { get; set; }
    public string? City { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; }  // NIF
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Website { get; set; }
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }
    public string? LogoFileId { get; set; }
    public bool IsOnline { get; set; }
    public string? Notes { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime StatusDate { get; set; }
    public ICollection<SubCustomer> SubCustomers { get; set; } = [];
}
```

#### SubCustomer

```sql
CREATE TABLE SubCustomer (
    Id              TEXT PRIMARY KEY,
    CustomerId      TEXT NOT NULL REFERENCES Customer(Id),
    Name            TEXT NOT NULL,
    Address         TEXT,
    ExtendedAddress TEXT,
    CountryId       TEXT REFERENCES Country(Id),
    Province        TEXT,
    City            TEXT,
    PostalCode      TEXT,
    TaxId           TEXT,
    Phone           TEXT,
    Email           TEXT,
    Website         TEXT,
    LogoFileId      TEXT,
    IsOnline        INTEGER NOT NULL DEFAULT 0,
    Notes           TEXT,
    Status          TEXT NOT NULL,
    StatusDate      TEXT NOT NULL,
    -- Campos de BaseEntity (omitidos por brevedad)
    ...
);
```

### 5.2 Usuarios y Roles

#### User

```sql
CREATE TABLE User (
    Id              TEXT PRIMARY KEY,
    TenantId        TEXT NOT NULL REFERENCES Tenant(Id),
    CustomerId      TEXT NOT NULL REFERENCES Customer(Id),
    SubCustomerId   TEXT REFERENCES SubCustomer(Id),
    ServicePlanId   TEXT NOT NULL REFERENCES ServicePlan(Id),
    Email           TEXT NOT NULL,
    PasswordHash    TEXT,
    FirstName       TEXT NOT NULL,
    LastName        TEXT NOT NULL,
    CountryId       TEXT NOT NULL REFERENCES Country(Id),
    DateOfBirth     TEXT,
    Gender          INTEGER,
    Phone           TEXT,
    NotificationEmail TEXT,
    UserType        TEXT,
    PreferredLocale TEXT NOT NULL DEFAULT 'es',
    TimeZoneId      TEXT NOT NULL DEFAULT 'Europe/Madrid',
    AvatarFileId    TEXT,
    IsActive        INTEGER NOT NULL DEFAULT 1,
    ActivationDate  TEXT,
    ExpirationDate  TEXT,
    IsBlocked       INTEGER NOT NULL DEFAULT 0,
    BlockedDate     TEXT,
    Notes           TEXT,
    -- Campos de BaseEntity
    ...
);
```

### 5.3 Referencia y Catálogos

#### Country

```sql
CREATE TABLE Country (
    Id              TEXT PRIMARY KEY,   -- Código ISO (ej: "ES")
    NameKey         TEXT NOT NULL,       -- Clave de recurso i18n
    NationalityKey  TEXT NOT NULL,
    LanguageCode    TEXT NOT NULL,
    SortOrder       INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

#### SportDiscipline (pendiente de implementar)

```sql
CREATE TABLE SportDiscipline (
    Id                  TEXT PRIMARY KEY,
    NameKey             TEXT NOT NULL,
    ImageFileId         TEXT,
    SquadSize           INTEGER NOT NULL,
    FieldPlayerCount    INTEGER NOT NULL,
    PeriodCount         INTEGER NOT NULL,
    OvertimeCount       INTEGER,
    PeriodDuration      TEXT NOT NULL,       -- TimeSpan como texto
    OvertimeDuration    TEXT NOT NULL,
    HasGoalkeeper       INTEGER NOT NULL DEFAULT 1,
    AutoOvertime        INTEGER NOT NULL DEFAULT 0,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    ActivationDate      TEXT NOT NULL,
    Notes               TEXT,
    FieldColor          TEXT,
    -- Campos de BaseEntity
    ...
);
```

#### SportCategory (pendiente de implementar)

```sql
CREATE TABLE SportCategory (
    Id                  TEXT PRIMARY KEY,
    SportDisciplineId   TEXT NOT NULL REFERENCES SportDiscipline(Id),
    CustomerId          TEXT REFERENCES Customer(Id),
    Name                TEXT NOT NULL,
    -- Campos de BaseEntity
    ...
);
```

#### FieldPosition (pendiente de implementar)

```sql
CREATE TABLE FieldPosition (
    Id                  TEXT PRIMARY KEY,
    SportDisciplineId   TEXT NOT NULL REFERENCES SportDiscipline(Id),
    CustomerId          TEXT REFERENCES Customer(Id),
    Name                TEXT NOT NULL,
    -- Campos de BaseEntity
    ...
);
```

### 5.4 Entidades y Equipos

#### Organization (pendiente de implementar)

```sql
CREATE TABLE Organization (
    Id                  TEXT PRIMARY KEY,
    TenantId            TEXT NOT NULL REFERENCES Tenant(Id),
    CustomerId          TEXT NOT NULL REFERENCES Customer(Id),
    ServicePlanId       TEXT NOT NULL REFERENCES ServicePlan(Id),
    UserId              TEXT NOT NULL REFERENCES User(Id),
    SportDisciplineId   TEXT NOT NULL REFERENCES SportDiscipline(Id),
    Name                TEXT NOT NULL,
    Address             TEXT,
    ExtendedAddress     TEXT,
    PostalCode          TEXT,
    City                TEXT,
    Province            TEXT,
    CountryId           TEXT REFERENCES Country(Id),
    Latitude            REAL,
    Longitude           REAL,
    Email               TEXT,
    Phone               TEXT,
    Website             TEXT,
    LogoFileId          TEXT,
    IsOrgChart          INTEGER NOT NULL DEFAULT 0,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    ActivationDate      TEXT,
    Notes               TEXT,
    -- Campos de BaseEntity
    ...
);
```

#### Team ✅ Implementado

```csharp
// Disnegativos.Shared/Data/Entities/Team.cs
namespace Disnegativos.Shared.Data.Entities;

public class Team : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    public Guid UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Alias { get; set; }
    public string? ImageFileId { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
    public string? Notes { get; set; }
}
```

#### Player (pendiente de implementar)

```sql
CREATE TABLE Player (
    Id                  TEXT PRIMARY KEY,
    TenantId            TEXT NOT NULL REFERENCES Tenant(Id),
    CustomerId          TEXT NOT NULL REFERENCES Customer(Id),
    ServicePlanId       TEXT NOT NULL REFERENCES ServicePlan(Id),
    SportDisciplineId   TEXT NOT NULL REFERENCES SportDiscipline(Id),
    UserId              TEXT NOT NULL REFERENCES User(Id),
    OrganizationId      TEXT REFERENCES Organization(Id),
    DefaultTeamId       TEXT REFERENCES Team(Id),
    FieldPositionId     TEXT REFERENCES FieldPosition(Id),
    PlayerType          INTEGER NOT NULL,
    FirstName           TEXT NOT NULL,
    LastName            TEXT,
    Nickname            TEXT,
    DateOfBirth         TEXT NOT NULL,
    Gender              INTEGER NOT NULL,
    CountryId           TEXT REFERENCES Country(Id),
    SecondCountryId     TEXT REFERENCES Country(Id),
    Email               TEXT,
    Phone               TEXT,
    JerseyNumber        TEXT,
    Height              TEXT,
    Weight              TEXT,
    PreferredFoot       TEXT,
    ImageFileId         TEXT,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    ActivationDate      TEXT NOT NULL,
    Notes               TEXT,
    -- Campos de BaseEntity
    ...
);
```

#### TeamPlayer (pendiente de implementar — relación M:N)

```sql
CREATE TABLE TeamPlayer (
    Id                  TEXT PRIMARY KEY,
    TeamId              TEXT NOT NULL REFERENCES Team(Id),
    PlayerId            TEXT NOT NULL REFERENCES Player(Id),
    FieldPositionId     TEXT REFERENCES FieldPosition(Id),
    JerseyNumber        TEXT,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    ActivationDate      TEXT,
    StartDate           TEXT,
    EndDate             TEXT,
    -- Campos de BaseEntity
    ...
);
```

### 5.5 Competiciones y Eventos

#### Competition ✅ Implementado

```csharp
// Disnegativos.Shared/Data/Entities/Competition.cs
namespace Disnegativos.Shared.Data.Entities;

public class Competition : BaseEntity
{
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }
    public Guid? UserId { get; set; }
    public Guid? TeamId { get; set; }

    public string Title { get; set; } = string.Empty;
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public string? Notes { get; set; }
    public string? Color { get; set; }
    public bool IsActive { get; set; } = true;
    public bool IsPrivate { get; set; } = false;
    public bool ShowInCalendar { get; set; } = true;
    public string? ImageFileId { get; set; }

    public ICollection<Event> Events { get; set; } = [];
}
```

#### Event ✅ Implementado

```csharp
// Disnegativos.Shared/Data/Entities/Event.cs
namespace Disnegativos.Shared.Data.Entities;

public class Event : BaseEntity
{
    public Guid TenantId { get; set; }
    public Guid CustomerId { get; set; }
    public Guid ServicePlanId { get; set; }
    public Guid SportDisciplineId { get; set; }
    public Guid SportCategoryId { get; set; }

    public Guid CompetitionId { get; set; }
    public Competition Competition { get; set; } = null!;

    public Guid UserId { get; set; }

    public Guid HomeTeamId { get; set; }
    public Team HomeTeam { get; set; } = null!;

    public Guid? AwayTeamId { get; set; }
    public Team? AwayTeam { get; set; }

    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsHomeGame { get; set; }
    public string? Result { get; set; }

    public DateTime StartDate { get; set; }    // UTC
    public TimeSpan StartTime { get; set; }
    public TimeSpan? MeetingTime { get; set; }

    public string? Matchday { get; set; }
    public string? Phase { get; set; }
    public string? Notes { get; set; }
    public string? JsonData { get; set; }

    public bool IsActive { get; set; } = true;
    public DateTime ActivationDate { get; set; }
}
```

### 5.6 Plantillas de Análisis (Keypads) — pendiente de implementar

#### Template

```sql
CREATE TABLE Template (
    Id                  TEXT PRIMARY KEY,
    CustomerId          TEXT NOT NULL REFERENCES Customer(Id),
    ServicePlanId       TEXT NOT NULL REFERENCES ServicePlan(Id),
    SportDisciplineId   TEXT REFERENCES SportDiscipline(Id),
    UserId              TEXT NOT NULL REFERENCES User(Id),
    Name                TEXT NOT NULL,
    Description         TEXT,
    Author              TEXT,
    IsDefault           INTEGER NOT NULL DEFAULT 0,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    Status              INTEGER NOT NULL DEFAULT 0,
    ToggleableBlocks    INTEGER NOT NULL DEFAULT 0,
    CollapsibleBlocks   INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

#### Panel

```sql
CREATE TABLE Panel (
    Id                  TEXT PRIMARY KEY,
    TemplateId          TEXT NOT NULL REFERENCES Template(Id),
    UserId              TEXT REFERENCES User(Id),
    Name                TEXT NOT NULL,
    Description         TEXT,
    BackgroundColor     TEXT,
    ForegroundColor     TEXT,
    FontSize            INTEGER,
    SortOrder           INTEGER NOT NULL DEFAULT 0,
    IsVisible           INTEGER NOT NULL DEFAULT 1,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    SecondsBeforeClip   INTEGER NOT NULL DEFAULT 0,
    SecondsAfterClip    INTEGER NOT NULL DEFAULT 0,
    LevelCount          INTEGER,
    Padding             INTEGER,
    -- Campos de BaseEntity
    ...
);
```

#### Block

```sql
CREATE TABLE Block (
    Id                  TEXT PRIMARY KEY,
    PanelId             TEXT NOT NULL REFERENCES Panel(Id),
    UserId              TEXT REFERENCES User(Id),
    Name                TEXT NOT NULL,
    Description         TEXT,
    BackgroundColor     TEXT,
    ForegroundColor     TEXT,
    FontSize            INTEGER,
    SortOrder           INTEGER NOT NULL DEFAULT 0,
    IsVisible           INTEGER NOT NULL DEFAULT 1,
    IsHeaderVisible     INTEGER NOT NULL DEFAULT 1,
    ShowCounter         INTEGER NOT NULL DEFAULT 0,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    IsFixed             INTEGER NOT NULL DEFAULT 0,
    IsOptionMode        INTEGER NOT NULL DEFAULT 0,
    SecondsBeforeClip   INTEGER NOT NULL DEFAULT 0,
    SecondsAfterClip    INTEGER NOT NULL DEFAULT 0,
    ShowStatistics      INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

#### Button

```sql
CREATE TABLE Button (
    Id                  TEXT PRIMARY KEY,
    BlockId             TEXT NOT NULL REFERENCES Block(Id),
    ConceptId           TEXT REFERENCES Concept(Id),
    ParentButtonId      TEXT REFERENCES Button(Id),
    Name                TEXT NOT NULL,
    ButtonCategoryType  INTEGER NOT NULL DEFAULT 0,
    BackgroundColor     TEXT,
    ForegroundColor     TEXT,
    SecondsBeforeClip   REAL NOT NULL DEFAULT 2.0,
    SecondsAfterClip    REAL NOT NULL DEFAULT 2.0,
    SortOrder           INTEGER NOT NULL DEFAULT 0,
    IsVisible           INTEGER NOT NULL DEFAULT 1,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    IsFavorite          INTEGER NOT NULL DEFAULT 0,
    IsAttribute         INTEGER NOT NULL DEFAULT 0,
    IsInOut             INTEGER NOT NULL DEFAULT 0,
    IsToggle            INTEGER NOT NULL DEFAULT 0,
    HasChildren         INTEGER NOT NULL DEFAULT 0,
    RequiresTeam        INTEGER NOT NULL DEFAULT 0,
    RequiresPlayer      INTEGER NOT NULL DEFAULT 0,
    RequiresZone        INTEGER NOT NULL DEFAULT 0,
    ShowStatistics      INTEGER NOT NULL DEFAULT 0,
    Shortcut            TEXT,
    Description         TEXT,
    BackgroundImage     TEXT,
    -- Campos de BaseEntity
    ...
);
```

### 5.7 Análisis y Acciones — pendiente de implementar

#### Analysis

```sql
CREATE TABLE Analysis (
    Id                  TEXT PRIMARY KEY,
    EventId             TEXT NOT NULL REFERENCES Event(Id),
    TemplateId          TEXT NOT NULL REFERENCES Template(Id),
    UserId              TEXT NOT NULL REFERENCES User(Id),
    AnalyzedTeamId      TEXT REFERENCES Team(Id),
    OpponentTeamId      TEXT REFERENCES Team(Id),
    Name                TEXT NOT NULL,
    TotalDuration       TEXT NOT NULL,
    HasVideo            INTEGER NOT NULL DEFAULT 0,
    IsMultiVideo        INTEGER NOT NULL DEFAULT 0,
    IsLive              INTEGER NOT NULL DEFAULT 0,
    IsPrivate           INTEGER NOT NULL DEFAULT 0,
    IsFinished          INTEGER NOT NULL DEFAULT 0,
    FinishedDate        TEXT,
    AnalyzeOpponents    INTEGER NOT NULL DEFAULT 0,
    AnalyzeBoth         INTEGER NOT NULL DEFAULT 0,
    Status              INTEGER,
    Notes               TEXT,
    JsonConfig          TEXT,
    CurrentPeriod       INTEGER,
    -- Campos de BaseEntity
    ...
);
```

#### AnalysisMedia

```sql
CREATE TABLE AnalysisMedia (
    Id              TEXT PRIMARY KEY,
    AnalysisId      TEXT NOT NULL REFERENCES Analysis(Id),
    FilePath        TEXT NOT NULL,
    FileName        TEXT NOT NULL,
    MediaType       TEXT NOT NULL,      -- Video, Audio, Image
    Description     TEXT,
    OffsetMs        INTEGER,
    DurationMs      INTEGER,
    SortOrder       INTEGER NOT NULL DEFAULT 0,
    Thumbnail       TEXT,
    IsUploaded      INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

#### GamePeriod

```sql
CREATE TABLE GamePeriod (
    Id              TEXT PRIMARY KEY,
    AnalysisId      TEXT NOT NULL REFERENCES Analysis(Id),
    PeriodIndex     INTEGER NOT NULL,
    PeriodName      TEXT NOT NULL,
    IsMatchStart    INTEGER NOT NULL DEFAULT 0,
    IsPeriodStart   INTEGER NOT NULL DEFAULT 0,
    IsMatchEnd      INTEGER NOT NULL DEFAULT 0,
    IsActive        INTEGER NOT NULL DEFAULT 1,
    -- Campos de BaseEntity
    ...
);
```

#### MatchAction

```sql
CREATE TABLE MatchAction (
    Id                  TEXT PRIMARY KEY,
    AnalysisId          TEXT NOT NULL REFERENCES Analysis(Id),
    EventId             TEXT NOT NULL REFERENCES Event(Id),
    ButtonId            TEXT NOT NULL REFERENCES Button(Id),
    UserId              TEXT REFERENCES User(Id),
    TeamId              TEXT REFERENCES Team(Id),
    GamePeriodId        TEXT REFERENCES GamePeriod(Id),
    ButtonName          TEXT,
    ButtonColor         TEXT,
    Notes               TEXT,
    MediaUrl            TEXT,
    Timestamp           TEXT NOT NULL,
    TimestampEnd        TEXT,
    TimestampMs         REAL,
    TimestampEndMs      REAL,
    SecondsBeforeClip   REAL NOT NULL DEFAULT 0,
    SecondsAfterClip    REAL NOT NULL DEFAULT 0,
    VideoPositionMs     INTEGER NOT NULL DEFAULT 0,
    VideoStartMs        INTEGER NOT NULL DEFAULT 0,
    VideoEndMs          INTEGER NOT NULL DEFAULT 0,
    IsFavorite          INTEGER NOT NULL DEFAULT 0,
    IsInOut             INTEGER,
    JsonAttributes      TEXT,
    GeoLocation         TEXT,
    GeoLocationEnd      TEXT,
    Distance            TEXT,
    SortOrder           INTEGER,
    SortOrderCreation   INTEGER,
    Score               TEXT,
    ParentActionId      TEXT REFERENCES MatchAction(Id),
    RelatedActionId     TEXT REFERENCES MatchAction(Id),
    -- Campos de BaseEntity
    ...
);
```

#### ActionPlayer (relación M:N)

```sql
CREATE TABLE ActionPlayer (
    Id              TEXT PRIMARY KEY,
    ActionId        TEXT NOT NULL REFERENCES MatchAction(Id),
    PlayerId        TEXT NOT NULL REFERENCES Player(Id),
    SortOrder       INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

### 5.8 Informes y Diapositivas — pendiente de implementar

#### Report

```sql
CREATE TABLE Report (
    Id                  TEXT PRIMARY KEY,
    CustomerId          TEXT NOT NULL REFERENCES Customer(Id),
    ServicePlanId       TEXT NOT NULL REFERENCES ServicePlan(Id),
    UserId              TEXT NOT NULL REFERENCES User(Id),
    EventId             TEXT REFERENCES Event(Id),
    AnalysisId          TEXT REFERENCES Analysis(Id),
    SportDisciplineId   TEXT REFERENCES SportDiscipline(Id),
    Name                TEXT NOT NULL,
    ReportDate          TEXT NOT NULL,
    TotalDurationSec    REAL,
    MediaUrl            TEXT,
    Notes               TEXT,
    IsPrivate           INTEGER NOT NULL DEFAULT 0,
    Password            TEXT,
    ShowHeaders         INTEGER NOT NULL DEFAULT 1,
    ShowNotes           INTEGER NOT NULL DEFAULT 1,
    ShowSlideNumbers    INTEGER NOT NULL DEFAULT 1,
    ShowChapters        INTEGER NOT NULL DEFAULT 1,
    ShowBody            INTEGER NOT NULL DEFAULT 1,
    ShowCover           INTEGER NOT NULL DEFAULT 1,
    ShowCredits         INTEGER NOT NULL DEFAULT 1,
    ShowPlayerOnAction  INTEGER NOT NULL DEFAULT 0,
    ShowActionTime      INTEGER NOT NULL DEFAULT 0,
    ShowBadge           INTEGER NOT NULL DEFAULT 0,
    SendStatus          INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

#### Slide

```sql
CREATE TABLE Slide (
    Id                  TEXT PRIMARY KEY,
    ReportId            TEXT NOT NULL REFERENCES Report(Id),
    UserId              TEXT NOT NULL REFERENCES User(Id),
    ActionId            TEXT REFERENCES MatchAction(Id),
    TeamId              TEXT REFERENCES Team(Id),
    PlayerId            TEXT REFERENCES Player(Id),
    ButtonId            TEXT REFERENCES Button(Id),
    SlideType           INTEGER NOT NULL DEFAULT 0,
    Notes               TEXT,
    SortOrder           INTEGER NOT NULL DEFAULT 0,
    Duration            REAL NOT NULL DEFAULT 5.0,
    ChapterHtml         TEXT,
    BodyHtml            TEXT,
    HeaderHtml          TEXT,
    NoteHtml            TEXT,
    ShowChapter         INTEGER NOT NULL DEFAULT 0,
    ShowBody            INTEGER NOT NULL DEFAULT 0,
    ShowHeader          INTEGER NOT NULL DEFAULT 0,
    ShowNote            INTEGER NOT NULL DEFAULT 0,
    ShowSlideNumber     INTEGER NOT NULL DEFAULT 1,
    ShowBackgroundImage INTEGER NOT NULL DEFAULT 0,
    BackgroundImagePath TEXT,
    JsonDrawings        TEXT,
    JsonZoom            TEXT,
    IsFavorite          INTEGER NOT NULL DEFAULT 0,
    IsExpanded          INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

### 5.9 Conceptos y Categorías — pendiente de implementar

#### Concept

```sql
CREATE TABLE Concept (
    Id                  TEXT PRIMARY KEY,
    CustomerId          TEXT REFERENCES Customer(Id),
    PanelId             TEXT REFERENCES Panel(Id),
    Name                TEXT NOT NULL,
    CategoryType        INTEGER,
    IsToggle            INTEGER NOT NULL DEFAULT 0,
    RequiresTeam        INTEGER NOT NULL DEFAULT 0,
    RequiresPlayer      INTEGER NOT NULL DEFAULT 0,
    RequiresZone        INTEGER NOT NULL DEFAULT 0,
    ShowStatistics      INTEGER NOT NULL DEFAULT 0,
    IsOptionMode        INTEGER NOT NULL DEFAULT 0,
    OptionGroupName     TEXT,
    -- Campos de BaseEntity
    ...
);
```

#### ActionConcept

```sql
CREATE TABLE ActionConcept (
    Id              TEXT PRIMARY KEY,
    ActionId        TEXT NOT NULL REFERENCES MatchAction(Id),
    ConceptId       TEXT NOT NULL REFERENCES Concept(Id),
    ButtonId        TEXT REFERENCES Button(Id),
    ConceptName     TEXT,
    SortOrder       INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

### 5.10 Servicio y Configuración

#### ServicePlan ✅ Implementado

```sql
CREATE TABLE ServicePlan (
    Id                  TEXT PRIMARY KEY,
    SecondsBeforeClip   INTEGER NOT NULL DEFAULT 5,
    SecondsAfterClip    INTEGER NOT NULL DEFAULT 5,
    MaxTemplates        INTEGER NOT NULL DEFAULT 10,
    MaxPanels           INTEGER NOT NULL DEFAULT 20,
    MaxBlocks           INTEGER NOT NULL DEFAULT 50,
    MaxButtons          INTEGER NOT NULL DEFAULT 200,
    MaxAnalyses         INTEGER NOT NULL DEFAULT 100,
    AllowFieldView      INTEGER NOT NULL DEFAULT 1,
    AllowOrgChart       INTEGER NOT NULL DEFAULT 0,
    DeviceOnly          INTEGER NOT NULL DEFAULT 0,
    BucketName          TEXT,
    AllowMediaUtils     INTEGER NOT NULL DEFAULT 0,
    AllowDrawings       INTEGER NOT NULL DEFAULT 0,
    AllowConcatVideo    INTEGER NOT NULL DEFAULT 0,
    AllowMultiVideo     INTEGER NOT NULL DEFAULT 0,
    -- Campos de BaseEntity
    ...
);
```

### 5.11 Personas (Staff sin ser jugadores) — pendiente de implementar

#### Person

```sql
CREATE TABLE Person (
    Id                  TEXT PRIMARY KEY,
    CustomerId          TEXT NOT NULL REFERENCES Customer(Id),
    ServicePlanId       TEXT NOT NULL REFERENCES ServicePlan(Id),
    SportDisciplineId   TEXT NOT NULL REFERENCES SportDiscipline(Id),
    RoleId              TEXT REFERENCES PersonRole(Id),
    FirstName           TEXT NOT NULL,
    LastName            TEXT NOT NULL,
    DateOfBirth         TEXT NOT NULL,
    Gender              INTEGER NOT NULL,
    CountryId           TEXT REFERENCES Country(Id),
    Email               TEXT,
    Phone               TEXT,
    ImageFileId         TEXT,
    IsActive            INTEGER NOT NULL DEFAULT 1,
    Notes               TEXT,
    -- Campos de BaseEntity
    ...
);
```

#### PersonRole

```sql
CREATE TABLE PersonRole (
    Id              TEXT PRIMARY KEY,
    Name            TEXT NOT NULL,
    SportDisciplineId TEXT REFERENCES SportDiscipline(Id),
    IsActive        INTEGER NOT NULL DEFAULT 1,
    Notes           TEXT,
    -- Campos de BaseEntity
    ...
);
```

### 5.12 Tabla de Control de Sincronización

#### SyncLog ✅ Implementado

```sql
CREATE TABLE SyncLog (
    Id              TEXT PRIMARY KEY,
    TableName       TEXT NOT NULL,
    RecordId        TEXT NOT NULL,
    OperationType   INTEGER NOT NULL,    -- 0=Insert, 1=Update, 2=Delete
    ChangedFields   TEXT,                -- JSON con los campos modificados
    Timestamp       TEXT NOT NULL,        -- UTC
    IsSynced        INTEGER NOT NULL DEFAULT 0,
    SyncedAt        TEXT,
    ConflictData    TEXT,
    RetryCount      INTEGER NOT NULL DEFAULT 0
);
```

## 6. Estado de Implementación

| Entidad | Estado | Ubicación |
|---|---|---|
| `Tenant` | ✅ Implementado | `Data/Entities/Tenant.cs` |
| `Customer` | ✅ Implementado | `Data/Entities/Customer.cs` |
| `SubCustomer` | ✅ Implementado | `Data/Entities/SubCustomer.cs` |
| `Country` | ✅ Implementado | `Data/Entities/Country.cs` |
| `ServicePlan` | ✅ Implementado | `Data/Entities/ServicePlan.cs` |
| `User` | ✅ Implementado | `Data/Entities/User.cs` |
| `Team` | ✅ Implementado | `Data/Entities/Team.cs` |
| `Competition` | ✅ Implementado | `Data/Entities/Competition.cs` |
| `Event` | ✅ Implementado | `Data/Entities/Event.cs` |
| `SyncLog` | ✅ Implementado | `Data/Entities/SyncLog.cs` |
| `SportDiscipline` | ✅ Implementado | `Data/Entities/SportDiscipline.cs` |
| `SportCategory` | ✅ Implementado | `Data/Entities/SportCategory.cs` |
| `FieldPosition` | ✅ Implementado | `Data/Entities/FieldPosition.cs` |
| `Organization` | ✅ Implementado | `Data/Entities/Organization.cs` |
| `Player` | ✅ Implementado | `Data/Entities/Player.cs` |
| `TeamPlayer` | ✅ Implementado | `Data/Entities/TeamPlayer.cs` |
| `Template` | ✅ Implementado | `Data/Entities/Template.cs` |
| `Panel` | ✅ Implementado | `Data/Entities/Panel.cs` |
| `Block` | ✅ Implementado | `Data/Entities/Block.cs` |
| `Button` | ✅ Implementado | `Data/Entities/Button.cs` |
| `Concept` | ✅ Implementado | `Data/Entities/Concept.cs` |
| `Analysis` | ✅ Implementado | `Data/Entities/Analysis.cs` |
| `AnalysisMedia` | ✅ Implementado | `Data/Entities/AnalysisMedia.cs` |
| `GamePeriod` | ✅ Implementado | `Data/Entities/GamePeriod.cs` |
| `MatchAction` | ✅ Implementado | `Data/Entities/MatchAction.cs` |
| `ActionPlayer` | ✅ Implementado | `Data/Entities/ActionPlayer.cs` |
| `ActionConcept` | ✅ Implementado | `Data/Entities/ActionConcept.cs` |
| `Report` | ✅ Implementado | `Data/Entities/Report.cs` |
| `Slide` | ✅ Implementado | `Data/Entities/Slide.cs` |
| `Person` | ✅ Implementado | `Data/Entities/Person.cs` |
| `PersonRole` | ✅ Implementado | `Data/Entities/PersonRole.cs` |

## 7. Campos Eliminados / Redundancias Resueltas

| Campo Original | Tabla Original | Resolución |
|---|---|---|
| `idGrupo` | Múltiples | → `TenantId` (normalizado como FK) |
| `idCliente` + `idSubCliente` + `idServicio` | Múltiples | Eliminados de tablas hijas; se navegan vía jerarquía |
| `id*Cliente` (varchar) + `id*` (integer) | Múltiples | Unificado: solo `Id` (Guid). El `ServerId` cubre la referencia al servidor |
| `imagen`, `imagenPath`, `ImagenNombre` | Múltiples | Unificado: `ImageFileId` (referencia a almacenamiento) |
| `archivado`, `fechaArchivado`, `idUsuarioArchivado` | Todas | Unificado: `IsArchived`, `ArchivedAt`, `ArchivedByUserId` |
| `nombreBoton`, `color`, `colorFuente`, `colorFondo` | Acciones | Desnormalizados para performance en acciones; límites claros |
| `idsPadresCliente`, `idsPadres`, `namePadres` | Botones, Acciones | Eliminados: se navegan vía `ParentButtonId` recursivo |
| `json*` (múltiples) | Análisis, Acciones | Consolidados en un único `JsonConfig` / `JsonAttributes` |
| `isselected` | Múltiples | Eliminado: es estado de UI, no pertenece a BD |
| `url`, `url2`, `url3`, `url4` | Análisis | → Tabla `AnalysisMedia` (1:N) |
| `delay`, `delay2`, etc. | Análisis | → Incluido en `AnalysisMedia.OffsetMs` |
| Campos duplicados ES/EN | Pais, Idiomas, Literales | → Sistema i18n con claves de recurso |
