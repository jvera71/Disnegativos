# Disnegativos — Concerns Transversales

## 1. Sincronización Offline / Online

### 1.1 Estrategia General

La aplicación sigue un modelo **offline-first**: todas las operaciones se realizan contra la base de datos SQLite local. Los cambios se sincronizan con el servidor cuando hay conectividad.

```
┌─────────────────────────────────────────────────┐
│                  DISPOSITIVO                     │
│                                                  │
│  ┌──────────┐    ┌───────────┐   ┌────────────┐ │
│  │ UI/Blazor│───>│ Services  │──>│ SQLite     │ │
│  │ Pages    │    │           │   │ (EF Core)  │ │
│  └──────────┘    └───────────┘   └─────┬──────┘ │
│                                        │        │
│                                  ┌─────┴──────┐ │
│                                  │ SyncLog    │ │
│                                  │ (Changes)  │ │
│                                  └─────┬──────┘ │
│                                        │        │
└────────────────────────────────────────┼────────┘
                                         │
                              ┌──────────┴──────────┐
                              │  Sync Engine         │
                              │  (pendiente)         │
                              │  - Change Detection  │
                              │  - Conflict Resolver  │
                              │  - Batch Upload      │
                              │  - Delta Download    │
                              └──────────┬──────────┘
                                         │ HTTPS/REST
                              ┌──────────┴──────────┐
                              │     SERVIDOR         │
                              │  ASP.NET Web API     │
                              │  SQLite / PostgreSQL  │
                              └─────────────────────┘
```

### 1.2 Change Tracking Local

#### Tabla SyncLog ✅ Implementado

Cada operación CUD (Create/Update/Delete) registra una entrada en `SyncLog`. La entidad se encuentra en `Disnegativos.Shared.Data.Entities.SyncLog`:

```csharp
// Disnegativos.Shared/Data/Entities/SyncLog.cs
namespace Disnegativos.Shared.Data.Entities;

public class SyncLog
{
    public Guid Id { get; set; }
    public string TableName { get; set; } = string.Empty;
    public Guid RecordId { get; set; }
    public SyncOperation OperationType { get; set; }  // Insert, Update, Delete
    public string? ChangedFields { get; set; }          // JSON
    public DateTime Timestamp { get; set; }             // UTC
    public bool IsSynced { get; set; }
    public DateTime? SyncedAt { get; set; }
    public string? ConflictData { get; set; }
    public int RetryCount { get; set; }
}
```

El `DbSet<SyncLog>` está registrado en `DisnegativosDbContext`:

```csharp
// Disnegativos.Shared/Data/DisnegativosDbContext.cs
public DbSet<SyncLog> SyncLogs => Set<SyncLog>();
```

### 1.3 Interceptor de EF Core (pendiente de implementar)

```csharp
// Futuro: Disnegativos.Shared/Data/Interceptors/SyncTrackingInterceptor.cs
public class SyncTrackingInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken ct = default)
    {
        var context = eventData.Context!;
        foreach (var entry in context.ChangeTracker.Entries<BaseEntity>())
        {
            if (entry.State is EntityState.Added or EntityState.Modified or EntityState.Deleted)
            {
                context.Set<SyncLog>().Add(new SyncLog
                {
                    Id = Guid.NewGuid(),
                    TableName = entry.Metadata.GetTableName()!,
                    RecordId = entry.Entity.Id,
                    OperationType = MapState(entry.State),
                    ChangedFields = SerializeChanges(entry),
                    Timestamp = DateTime.UtcNow
                });
            }
        }
        return base.SavingChangesAsync(eventData, result, ct);
    }
}
```

### 1.4 Motor de Sincronización (pendiente de implementar)

#### Flujo Push (dispositivo → servidor)

1. Detectar conectividad (`IConnectivity` en MAUI)
2. Leer `SyncLog` donde `IsSynced = false`, ordenado por `Timestamp`
3. Agrupar en lotes (batch de max 100 registros)
4. Enviar al endpoint `POST /api/sync/push`
5. Servidor procesa, detecta conflictos, responde con resultado
6. Marcar como sincronizados o registrar conflicto

#### Flujo Pull (servidor → dispositivo)

1. Enviar `lastSyncTimestamp` al endpoint `GET /api/sync/pull?since={timestamp}`
2. Servidor retorna delta de cambios desde esa fecha
3. Aplicar cambios al SQLite local
4. Actualizar `lastSyncTimestamp`

#### Resolución de Conflictos

| Estrategia | Cuándo |
|---|---|
| **Last-write-wins** | Para datos no críticos (notas, configuración de UI) |
| **Server-wins** | Para datos compartidos entre usuarios |
| **Manual resolution** | Para acciones y análisis: presenta ambas versiones al usuario |

```csharp
public interface IConflictResolver
{
    Task<ConflictResolution> ResolveAsync(SyncConflict conflict);
}

public enum ConflictResolution
{
    KeepLocal,
    KeepServer,
    Merge,
    AskUser
}
```

### 1.5 Detección de Conectividad (pendiente de implementar)

```csharp
// Futuro: Disnegativos/Services/ConnectivityService.cs (MAUI)
public class ConnectivityService : IConnectivityService
{
    private readonly IConnectivity _connectivity; // MAUI

    public bool IsOnline => _connectivity.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<bool>? ConnectivityChanged;

    public ConnectivityService(IConnectivity connectivity)
    {
        _connectivity = connectivity;
        _connectivity.ConnectivityChanged += (s, e) =>
        {
            ConnectivityChanged?.Invoke(this, IsOnline);
            if (IsOnline)
                _ = TriggerSyncAsync(); // Fire-and-forget sync
        };
    }
}
```

---

## 2. Multi-idioma (i18n) ✅ Implementado

### 2.1 Estrategia

Se usan **archivos de recursos .resx** estándar de .NET con `IStringLocalizer<T>`. Los archivos se encuentran en `Disnegativos.Shared/Resources/`:

```
Disnegativos.Shared/
└── Resources/
    ├── SharedResources.cs             # Clase marcadora
    ├── SharedResources.resx           # Idioma por defecto: español
    └── SharedResources.en.resx        # Inglés
```

> Extensible a más idiomas añadiendo `SharedResources.{code}.resx` (ej: `SharedResources.ca.resx` para catalán)

### 2.2 Uso en Componentes Blazor

```razor
@inject IStringLocalizer<SharedResources> L

<RadzenButton Text="@L["Save"]" />
<RadzenText>@L["Event_Name_Label"]</RadzenText>
```

El `_Imports.razor` de `Disnegativos.Shared` ya incluye los usings necesarios:

```razor
@using Microsoft.Extensions.Localization
@using Disnegativos.Shared.Resources
```

### 2.3 Configuración

```csharp
// En Program.cs (tanto MAUI como Web)
builder.Services.AddLocalization(options =>
    options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "es", "en", "ca", "fr" };
    options.SetDefaultCulture("es");
    options.AddSupportedCultures(supportedCultures);
    options.AddSupportedUICultures(supportedCultures);
});
```

### 2.4 Persistencia del Idioma

El idioma preferido se almacena en `User.PreferredLocale` y se aplica al inicio de sesión. En MAUI se persiste también en `Preferences`.

```csharp
public class LocaleService : ILocaleService
{
    public void SetCulture(string locale)
    {
        var culture = new CultureInfo(locale);
        CultureInfo.DefaultThreadCurrentCulture = culture;
        CultureInfo.DefaultThreadCurrentUICulture = culture;
    }
}
```

---

## 3. Multi Zona Horaria ✅ Implementado

### 3.1 Regla de Oro

> **Todas las fechas se almacenan en UTC en la base de datos.**
> La conversión a la zona horaria del usuario se hace exclusivamente en la capa de presentación.

### 3.2 Implementación

La interfaz `ITimeZoneService` está en `Disnegativos.Shared.Interfaces` y la implementación en `Disnegativos.Shared.Services.Implementations`:

```csharp
// Disnegativos.Shared/Interfaces/ITimeZoneService.cs
namespace Disnegativos.Shared.Interfaces;

public interface ITimeZoneService
{
    void SetUserTimeZone(string timeZoneId);
    DateTime ToUserTime(DateTime utcDateTime);
    DateTime ToUtc(DateTime localDateTime);
    string FormatForUser(DateTime utcDateTime, string format = "g");
}
```

```csharp
// Disnegativos.Shared/Services/Implementations/TimeZoneService.cs
using System.Globalization;
using Disnegativos.Shared.Interfaces;

namespace Disnegativos.Shared.Services.Implementations;

public class TimeZoneService : ITimeZoneService
{
    private TimeZoneInfo _userTimeZone = TimeZoneInfo.Utc;

    public void SetUserTimeZone(string timeZoneId)
    {
        try
        {
            _userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            _userTimeZone = TimeZoneInfo.Utc;
        }
    }

    public DateTime ToUserTime(DateTime utcDateTime)
        => TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, _userTimeZone);

    public DateTime ToUtc(DateTime localDateTime)
        => TimeZoneInfo.ConvertTimeToUtc(localDateTime, _userTimeZone);

    public string FormatForUser(DateTime utcDateTime, string format = "g")
        => ToUserTime(utcDateTime).ToString(format, CultureInfo.CurrentCulture);
}
```

Registrado en DI:

```csharp
// Disnegativos.Shared/DependencyInjection/ServiceCollectionExtensions.cs
services.AddScoped<ITimeZoneService, TimeZoneService>();
```

### 3.3 Uso en Servicios

El `EventService` utiliza `ITimeZoneService` para la conversión UTC ↔ hora local:

```csharp
// En EventService.GetEventForEditAsync: UTC → hora local del usuario
var combinedUtc = e.StartDate.Add(e.StartTime);
var localDateTime = _timeZoneService.ToUserTime(combinedUtc);
return new EventEditDto
{
    LocalStartDate = localDateTime.Date,
    LocalStartTime = localDateTime.TimeOfDay,
    // ...
};

// En EventService.SaveEventAsync: hora local → UTC para guardar en BD
var localDateTime = dto.LocalStartDate!.Value.Add(dto.LocalStartTime!.Value);
var utcDateTime = _timeZoneService.ToUtc(localDateTime);
evt.StartDate = utcDateTime.Date;
evt.StartTime = utcDateTime.TimeOfDay;
```

### 3.4 Componente Blazor Helper (pendiente de implementar)

```razor
<!-- Futuro: TimeDisplay.razor -->
@inject ITimeZoneService TimeZoneService

<span>@TimeZoneService.FormatForUser(UtcDateTime, Format)</span>

@code {
    [Parameter] public DateTime UtcDateTime { get; set; }
    [Parameter] public string Format { get; set; } = "g";
}
```

---

## 4. Testing (pendiente de implementar)

### 4.1 Estrategia de Tests

| Proyecto | Framework | Qué se testea |
|---|---|---|
| `Disnegativos.Services.Tests` | xUnit + Moq | Servicios de negocio, lógica de sincronización |
| `Disnegativos.UI.Tests` | xUnit + bUnit | Componentes Presentational y Containers |

### 4.2 Tests de Servicios

```csharp
public class EventServiceTests
{
    private readonly Mock<DisnegativosDbContext> _contextMock;
    private readonly Mock<ITimeZoneService> _timeZoneMock;
    private readonly EventService _sut;

    public EventServiceTests()
    {
        _contextMock = new();
        _timeZoneMock = new();
        _sut = new EventService(_contextMock.Object, _timeZoneMock.Object);
    }

    [Fact]
    public async Task GetAllEventsAsync_ReturnsEvents()
    {
        // Arrange & Act & Assert
        // ...
    }
}
```

### 4.3 Tests de Componentes Blazor (bUnit)

```csharp
public class EventListViewTests : TestContext
{
    [Fact]
    public void RendersEventNames()
    {
        // Arrange
        var events = new List<EventDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Partido 1" },
            new() { Id = Guid.NewGuid(), Name = "Partido 2" }
        };

        // Act
        var cut = RenderComponent<EventListView>(parameters =>
            parameters.Add(p => p.Events, events)
                      .Add(p => p.IsLoading, false));

        // Assert
        Assert.Contains("Partido 1", cut.Markup);
        Assert.Contains("Partido 2", cut.Markup);
    }

    [Fact]
    public void DeleteButton_InvokesCallback()
    {
        // Arrange
        var eventId = Guid.NewGuid();
        Guid? deletedId = null;

        var cut = RenderComponent<EventListView>(parameters =>
            parameters.Add(p => p.Events, new List<EventDto>
                { new() { Id = eventId, Name = "Test" } })
                .Add(p => p.IsLoading, false)
                .Add(p => p.OnDelete, (Guid id) => { deletedId = id; }));

        // Act
        cut.Find("[data-testid='delete-btn']").Click();

        // Assert
        Assert.Equal(eventId, deletedId);
    }
}
```

### 4.4 Configuración de bUnit con Radzen

```csharp
// Clase base para tests con Radzen
public abstract class RadzenTestBase : TestContext
{
    protected RadzenTestBase()
    {
        Services.AddScoped<DialogService>();
        Services.AddScoped<NotificationService>();
        Services.AddScoped<TooltipService>();
        Services.AddScoped<ContextMenuService>();
        JSInterop.SetupVoid("Radzen.createDatePicker", _ => true);
        JSInterop.SetupVoid("Radzen.createGrid", _ => true);
    }
}
```

---

## 5. Deployment por Plataforma

### 5.1 Plataformas Soportadas

| Plataforma | Tipo | Proyecto | Paquete |
|---|---|---|---|
| **Windows** | MAUI Blazor Hybrid | `Disnegativos` | MSIX |
| **macOS** | MAUI Blazor Hybrid | `Disnegativos` | .app / .pkg |
| **Android** | MAUI Blazor Hybrid | `Disnegativos` | APK / AAB |
| **iOS** | MAUI Blazor Hybrid | `Disnegativos` | IPA (App Store) |
| **Web** | Blazor Web + WASM | `Disnegativos.Web` + `Disnegativos.Web.Client` | Docker / Azure |

### 5.2 Configuración por Plataforma

```csharp
// Disnegativos/MauiProgram.cs (MAUI)
builder.Services.AddSingleton<IFormFactor, FormFactor>();
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
builder.Services.AddDisnegativosSharedServices($"Data Source={dbPath}");
builder.Services.AddMauiBlazorWebView();

// Disnegativos.Web/Program.cs (Web Server)
builder.Services.AddSingleton<IFormFactor, FormFactor>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=disnegativos.db";
builder.Services.AddDisnegativosSharedServices(connectionString);
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();
```

### 5.3 Assemblies Compartidos

El host web registra los assemblies compartidos para que las páginas de `Disnegativos.Shared` se rendericen correctamente:

```csharp
// Disnegativos.Web/Program.cs
app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Disnegativos.Shared._Imports).Assembly,
        typeof(Disnegativos.Web.Client._Imports).Assembly);
```

### 5.4 Interfaz IFormFactor

Servicio abstracto para detectar la plataforma de ejecución:

```csharp
// Disnegativos.Shared/Services/IFormFactor.cs
namespace Disnegativos.Shared.Services;

public interface IFormFactor
{
    string GetFormFactor();
    string GetPlatform();
}
```

Cada proyecto host (`Disnegativos`, `Disnegativos.Web`, `Disnegativos.Web.Client`) proporciona su propia implementación de `FormFactor`.
