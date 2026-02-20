# Disnegativos — Concerns Transversales

## 1. Sincronización Offline / Online (MAUI)

### 1.1 Estrategia General

La aplicación MAUI sigue un modelo **offline-first**: todas las operaciones se realizan contra la base de datos SQLite **local** del dispositivo. Los cambios se sincronizan con el servidor cuando hay conectividad.

La aplicación Web (**Disnegativos.Web**) NO necesita sincronización — accede **directamente** a la SQLite del servidor.

```
┌──────────────────────────────────────────────────────────┐
│                Disnegativos.Shared (RCL)                  │
│  Blazor Pages · Services · EF Core · DisnegativosDbContext│
│   ──── Código compartido en MAUI y Web ────               │
└───────────┬──────────────────────────────┬───────────────┘
            │                              │
            ▼                              ▼
 MAUI: cada operación             Web: cada operación
 se guarda en SQLite LOCAL        se guarda en SQLite del SERVIDOR
            │                              │
    ┌───────┴────────┐              (no necesita sync)
    │ SyncLog (cola)  │
    └───────┬────────┘
            │ ¿Hay conexión?
            ├── SÍ → Push pendientes al servidor
            │        Pull cambios del servidor
            │        Notificar vía SignalR
            └── NO → Acumular en cola, esperar
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

> **Importante:** `SyncLog` solo se utiliza en el contexto de MAUI. La versión Web escribe directamente en la BD del servidor y no necesita registrar cambios para sincronizar.

### 1.3 Interceptor de EF Core (pendiente de implementar — solo MAUI)

El interceptor se activa **únicamente en MAUI** para registrar cada cambio en `SyncLog`:

```csharp
// Futuro: Disnegativos/Services/SyncTrackingInterceptor.cs (solo MAUI)
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

Registro condicional en MAUI:

```csharp
// Disnegativos/MauiProgram.cs
builder.Services.AddDbContext<DisnegativosDbContext>((sp, options) =>
{
    options.UseSqlite($"Data Source={dbPath}");
    options.AddInterceptors(new SyncTrackingInterceptor()); // Solo MAUI
});
```

### 1.4 Motor de Sincronización (pendiente de implementar — solo MAUI)

#### Interfaz del servicio

```csharp
// Disnegativos.Shared/Services/Interfaces/ISyncService.cs
public interface ISyncService
{
    bool IsSyncing { get; }
    Task<SyncResult> SyncAsync(CancellationToken ct = default);
    Task PushPendingChangesAsync(CancellationToken ct = default);
    Task PullServerChangesAsync(CancellationToken ct = default);
    event EventHandler<SyncProgressEventArgs>? SyncProgress;
}
```

#### Flujo Push (dispositivo → servidor)

1. Leer `SyncLog` donde `IsSynced = false`, ordenado por `Timestamp`
2. Agrupar en lotes (batch de max 100 registros)
3. Enviar al endpoint `POST /api/sync/push`
4. Servidor procesa, detecta conflictos, responde con resultado
5. Marcar como sincronizados o registrar conflicto
6. Servidor notifica a otros clientes vía SignalR

```csharp
// Futuro: Disnegativos/Services/SyncService.cs
public class SyncService : ISyncService
{
    private readonly DisnegativosDbContext _localDb;
    private readonly HttpClient _httpClient;
    private readonly IConnectivityService _connectivity;

    public async Task PushPendingChangesAsync(CancellationToken ct = default)
    {
        if (!_connectivity.IsOnline) return;

        var pending = await _localDb.SyncLogs
            .Where(s => !s.IsSynced)
            .OrderBy(s => s.Timestamp)
            .Take(100)
            .ToListAsync(ct);

        if (pending.Count == 0) return;

        var batch = new SyncBatchDto
        {
            Changes = pending.Select(MapToDto).ToList()
        };

        var response = await _httpClient.PostAsJsonAsync("/api/sync/push", batch, ct);
        var result = await response.Content.ReadFromJsonAsync<SyncBatchResultDto>(ct);

        foreach (var item in result!.Results)
        {
            var log = pending.First(p => p.Id == item.SyncLogId);
            if (item.Success)
            {
                log.IsSynced = true;
                log.SyncedAt = DateTime.UtcNow;
            }
            else
            {
                log.ConflictData = item.ConflictDetails;
                log.RetryCount++;
            }
        }

        await _localDb.SaveChangesAsync(ct);
    }
}
```

#### Flujo Pull (servidor → dispositivo)

1. Enviar `lastSyncTimestamp` al endpoint `GET /api/sync/pull?since={timestamp}`
2. Servidor retorna delta de cambios desde esa fecha
3. Aplicar cambios al SQLite local (sin generar nuevos SyncLog)
4. Actualizar `lastSyncTimestamp`

```csharp
public async Task PullServerChangesAsync(CancellationToken ct = default)
{
    if (!_connectivity.IsOnline) return;

    var lastSync = Preferences.Get("LastSyncTimestamp", DateTime.MinValue.ToString("O"));
    var response = await _httpClient
        .GetFromJsonAsync<SyncPullResultDto>($"/api/sync/pull?since={lastSync}", ct);

    if (response?.Changes.Count > 0)
    {
        // Aplicar cambios SIN disparar el SyncTrackingInterceptor
        // (usar un flag o un DbContext sin interceptor)
        foreach (var change in response.Changes)
        {
            await ApplyServerChangeAsync(change, ct);
        }

        Preferences.Set("LastSyncTimestamp", DateTime.UtcNow.ToString("O"));
    }
}
```

#### API REST en el servidor

```csharp
// Disnegativos.Web/Controllers/SyncController.cs
[ApiController]
[Route("api/sync")]
public class SyncController : ControllerBase
{
    private readonly DisnegativosDbContext _db;
    private readonly IHubContext<CollaborationHub> _hubContext;

    [HttpPost("push")]
    public async Task<ActionResult<SyncBatchResultDto>> Push(SyncBatchDto batch)
    {
        var results = new List<SyncItemResultDto>();

        foreach (var change in batch.Changes)
        {
            try
            {
                await ApplyChangeToServerDb(change);
                results.Add(new(change.SyncLogId, Success: true));

                // Notificar a TODOS los clientes conectados (Web y MAUI)
                await _hubContext.Clients.All.SendAsync(
                    "EntityChanged", change.TableName, change.RecordId);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                results.Add(new(change.SyncLogId, Success: false,
                    ConflictDetails: ex.Message));
            }
        }

        return Ok(new SyncBatchResultDto { Results = results });
    }

    [HttpGet("pull")]
    public async Task<ActionResult<SyncPullResultDto>> Pull([FromQuery] DateTime since)
    {
        var changes = await _db.SyncLogs // O tabla de cambios en el servidor
            .Where(s => s.Timestamp > since)
            .OrderBy(s => s.Timestamp)
            .ToListAsync();

        return Ok(new SyncPullResultDto { Changes = changes.Select(MapToDto).ToList() });
    }
}
```

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

### 1.5 Detección de Conectividad (pendiente de implementar — solo MAUI)

```csharp
// Disnegativos/Services/ConnectivityService.cs (solo MAUI)
public class ConnectivityService : IConnectivityService
{
    private readonly IConnectivity _connectivity; // MAUI
    private readonly ISyncService _syncService;

    public bool IsOnline => _connectivity.NetworkAccess == NetworkAccess.Internet;

    public event EventHandler<bool>? ConnectivityChanged;

    public ConnectivityService(IConnectivity connectivity, ISyncService syncService)
    {
        _connectivity = connectivity;
        _syncService = syncService;

        _connectivity.ConnectivityChanged += async (s, e) =>
        {
            var online = IsOnline;
            ConnectivityChanged?.Invoke(this, online);

            if (online)
            {
                // Al recuperar conexión → sincronizar todo lo pendiente
                await _syncService.PushPendingChangesAsync();
                await _syncService.PullServerChangesAsync();
            }
        };
    }
}
```

Registro en DI de MAUI:

```csharp
// Disnegativos/MauiProgram.cs
builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
builder.Services.AddSingleton<ISyncService, SyncService>();
```

---

## 2. Colaboración en Tiempo Real (SignalR)

### 2.1 Objetivo

Permitir que **dos o más usuarios** colaboren en tiempo real sobre los mismos datos. Cuando un usuario (sea Web o MAUI conectado) modifica un registro, los demás usuarios ven el cambio inmediatamente sin necesidad de refrescar manualmente.

### 2.2 Arquitectura

```
                     ┌──────────────────────────┐
                     │   CollaborationHub       │
                     │   (SignalR Hub)           │
                     │                          │
                     │   Servidor Web            │
                     └───┬───────┬──────┬───────┘
                         │       │      │
              WebSocket  │       │      │  WebSocket
                         │       │      │
                    ┌────┴──┐ ┌──┴───┐ ┌┴──────┐
                    │Usuario│ │Usuari│ │Usuario│
                    │ Web 1 │ │Web 2 │ │MAUI 1 │
                    │(SSR)  │ │(WASM)│ │(App)  │
                    └───────┘ └──────┘ └───────┘
```

### 2.3 Interfaz del Hub (Tipado Fuerte)

```csharp
// Disnegativos.Shared/Hubs/ICollaborationHub.cs
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
    /// Permite mostrar "Usuario X está editando este registro".
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
```

### 2.4 Implementación del Hub (Servidor)

```csharp
// Disnegativos.Web/Hubs/CollaborationHub.cs
using Microsoft.AspNetCore.SignalR;
using Disnegativos.Shared.Hubs;

namespace Disnegativos.Web.Hubs;

public class CollaborationHub : Hub<ICollaborationHubClient>, ICollaborationHubServer
{
    private static readonly Dictionary<string, string> _editingLocks = new();

    public async Task NotifyEntityChanged(string entityType, Guid entityId, string changeType)
    {
        // Notificar a TODOS los clientes EXCEPTO el que realizó el cambio
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
        _editingLocks.Remove(key);

        await Clients.Others.UserStoppedEditing(entityType, entityId, userName);
    }

    /// <summary>
    /// Análisis en vivo: unirse a una "sala" de un análisis concreto.
    /// Solo reciben actualizaciones los usuarios que estén observando el mismo análisis.
    /// </summary>
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
            _editingLocks.Remove(key);
            var parts = key.Split(':');
            await Clients.All.UserStoppedEditing(parts[0], Guid.Parse(parts[1]), Context.ConnectionId);
        }

        await base.OnDisconnectedAsync(exception);
    }
}
```

### 2.5 Registro en el Servidor

```csharp
// Disnegativos.Web/Program.cs
builder.Services.AddSignalR();

var app = builder.Build();

// ... otro middleware ...

app.MapHub<CollaborationHub>("/hubs/collaboration");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Disnegativos.Shared._Imports).Assembly,
        typeof(Disnegativos.Web.Client._Imports).Assembly);
```

### 2.6 Configuración del Cliente SignalR

#### En Disnegativos.Web (usuarios web)

```csharp
// Disnegativos.Web/Program.cs (o en ServiceCollectionExtensions)
builder.Services.AddScoped(sp =>
{
    var navigation = sp.GetRequiredService<NavigationManager>();
    return new HubConnectionBuilder()
        .WithUrl(navigation.ToAbsoluteUri("/hubs/collaboration"))
        .WithAutomaticReconnect()
        .Build();
});
```

#### En Disnegativos (MAUI — solo cuando hay conexión)

```csharp
// Disnegativos/MauiProgram.cs
builder.Services.AddSingleton(sp =>
{
    var serverUrl = "https://tu-servidor.com"; // Configuración
    return new HubConnectionBuilder()
        .WithUrl($"{serverUrl}/hubs/collaboration")
        .WithAutomaticReconnect(new[]
            { TimeSpan.Zero, TimeSpan.FromSeconds(2), TimeSpan.FromSeconds(10),
              TimeSpan.FromSeconds(30), TimeSpan.FromMinutes(1) })
        .Build();
});
```

### 2.7 Uso en Componentes Blazor (Containers)

```csharp
// Ejemplo: EventsPage.razor.cs (Container)
public partial class EventsPage : ComponentBase, IAsyncDisposable
{
    [Inject] private IEventService EventService { get; set; } = default!;
    [Inject] private HubConnection HubConnection { get; set; } = default!;

    private List<EventDto> _events = [];
    private bool _isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsAsync();

        // ── Suscribirse a cambios en tiempo real ──
        HubConnection.On<string, Guid, string>("EntityChanged",
            async (entityType, entityId, changeType) =>
        {
            if (entityType == nameof(Event))
            {
                await LoadEventsAsync();
                await InvokeAsync(StateHasChanged);
            }
        });

        // Conectar si aún no lo está
        if (HubConnection.State == HubConnectionState.Disconnected)
        {
            try { await HubConnection.StartAsync(); }
            catch { /* Offline — no pasa nada, funciona con datos locales */ }
        }
    }

    private async Task HandleSaveAsync(EventEditDto dto)
    {
        await EventService.SaveEventAsync(dto);

        // Notificar a otros usuarios del cambio
        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync(
                "NotifyEntityChanged", nameof(Event), dto.Id, "Updated");
        }

        await LoadEventsAsync();
    }

    private async Task HandleDeleteAsync(Guid eventId)
    {
        await EventService.DeleteEventAsync(eventId);

        if (HubConnection.State == HubConnectionState.Connected)
        {
            await HubConnection.InvokeAsync(
                "NotifyEntityChanged", nameof(Event), eventId, "Deleted");
        }

        await LoadEventsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        // No desconectar el hub aquí — es singleton/scoped y se reutiliza
    }
}
```

### 2.8 Caso de uso: Análisis en vivo colaborativo

El escenario principal de SignalR es el **análisis deportivo en tiempo real**, donde un analista registra acciones y otros usuarios ven las acciones aparecer al instante:

```csharp
// AnalysisLivePage.razor.cs
protected override async Task OnInitializedAsync()
{
    // Unirse a la "sala" del análisis
    await HubConnection.InvokeAsync("JoinAnalysisRoom", AnalysisId);

    // Recibir acciones registradas por otros analistas
    HubConnection.On<Guid, string, string>("LiveAnalysisUpdate",
        async (analysisId, updateType, jsonPayload) =>
    {
        switch (updateType)
        {
            case "ActionCreated":
                var action = JsonSerializer.Deserialize<MatchActionDto>(jsonPayload);
                _actions.Add(action!);
                break;

            case "PeriodChanged":
                _currentPeriod = JsonSerializer.Deserialize<int>(jsonPayload);
                break;

            case "TimerUpdated":
                _timerState = JsonSerializer.Deserialize<TimerState>(jsonPayload);
                break;
        }
        await InvokeAsync(StateHasChanged);
    });
}

private async Task RegisterAction(MatchActionDto action)
{
    // Guardar en BD
    await AnalysisService.AddActionAsync(action);

    // Difundir a la sala
    await HubConnection.InvokeAsync("SendLiveAnalysisUpdate",
        AnalysisId, "ActionCreated", JsonSerializer.Serialize(action));
}
```

### 2.9 Resumen: Quién usa qué

| Componente | Web | MAUI Online | MAUI Offline |
|---|---|---|---|
| **EF Core → SQLite servidor** | ✅ Directo | ❌ | ❌ |
| **EF Core → SQLite local** | ❌ | ✅ | ✅ |
| **SyncService (Push/Pull)** | ❌ | ✅ | ⏳ (Al reconectar) |
| **SyncLog** | ❌ | ✅ | ✅ |
| **SyncTrackingInterceptor** | ❌ | ✅ | ✅ |
| **SignalR Hub (servidor)** | ✅ | — | — |
| **SignalR Client (recibir)** | ✅ | ✅ | ❌ |
| **ConnectivityService** | ❌ | ✅ | ✅ |

---

## 3. Multi-idioma (i18n) ✅ Implementado

### 3.1 Estrategia

Se usan **archivos de recursos .resx** estándar de .NET con `IStringLocalizer<T>`. Los archivos se encuentran en `Disnegativos.Shared/Resources/`:

```
Disnegativos.Shared/
└── Resources/
    ├── SharedResources.cs             # Clase marcadora
    ├── SharedResources.resx           # Idioma por defecto: español
    └── SharedResources.en.resx        # Inglés
```

> Extensible a más idiomas añadiendo `SharedResources.{code}.resx` (ej: `SharedResources.ca.resx` para catalán)

### 3.2 Uso en Componentes Blazor

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

### 3.3 Configuración

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

### 3.4 Persistencia del Idioma

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

## 4. Multi Zona Horaria ✅ Implementado

### 4.1 Regla de Oro

> **Todas las fechas se almacenan en UTC en la base de datos.**
> La conversión a la zona horaria del usuario se hace exclusivamente en la capa de presentación.

### 4.2 Implementación

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

### 4.3 Uso en Servicios

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

---

## 5. Testing (pendiente de implementar)

### 5.1 Estrategia de Tests

| Proyecto | Framework | Qué se testea |
|---|---|---|
| `Disnegativos.Services.Tests` | xUnit + Moq | Servicios de negocio, lógica de sincronización |
| `Disnegativos.UI.Tests` | xUnit + bUnit | Componentes Presentational y Containers |

### 5.2 Tests de Servicios

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

### 5.3 Tests de Componentes Blazor (bUnit)

```csharp
public class EventListViewTests : TestContext
{
    [Fact]
    public void RendersEventNames()
    {
        var events = new List<EventDto>
        {
            new() { Id = Guid.NewGuid(), Name = "Partido 1" },
            new() { Id = Guid.NewGuid(), Name = "Partido 2" }
        };

        var cut = RenderComponent<EventListView>(parameters =>
            parameters.Add(p => p.Events, events)
                      .Add(p => p.IsLoading, false));

        Assert.Contains("Partido 1", cut.Markup);
        Assert.Contains("Partido 2", cut.Markup);
    }
}
```

---

## 6. Deployment por Plataforma

### 6.1 Plataformas Soportadas

| Plataforma | Tipo | Proyecto | BD | Sync |
|---|---|---|---|---|
| **Windows** | MAUI Blazor Hybrid | `Disnegativos` | SQLite local | Push/Pull + SignalR |
| **macOS** | MAUI Blazor Hybrid | `Disnegativos` | SQLite local | Push/Pull + SignalR |
| **Android** | MAUI Blazor Hybrid | `Disnegativos` | SQLite local | Push/Pull + SignalR |
| **iOS** | MAUI Blazor Hybrid | `Disnegativos` | SQLite local | Push/Pull + SignalR |
| **Web** | Blazor Web + WASM | `Disnegativos.Web` | SQLite servidor (directo) | SignalR |

### 6.2 Configuración por Plataforma

```csharp
// ═══ Disnegativos/MauiProgram.cs (MAUI — SQLite LOCAL + Sync + SignalR Client) ═══
builder.Services.AddSingleton<IFormFactor, FormFactor>();
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
builder.Services.AddDisnegativosSharedServices($"Data Source={dbPath}");
builder.Services.AddMauiBlazorWebView();

// Solo MAUI: servicios de sincronización y conectividad
builder.Services.AddSingleton<IConnectivity>(Connectivity.Current);
builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();
builder.Services.AddSingleton<ISyncService, SyncService>();

// SignalR Client para colaboración en tiempo real
builder.Services.AddSingleton(sp => new HubConnectionBuilder()
    .WithUrl("https://servidor.ejemplo.com/hubs/collaboration")
    .WithAutomaticReconnect()
    .Build());

// ═══ Disnegativos.Web/Program.cs (Web — SQLite SERVIDOR + SignalR Hub) ═══
builder.Services.AddSingleton<IFormFactor, FormFactor>();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=disnegativos.db";
builder.Services.AddDisnegativosSharedServices(connectionString);
builder.Services.AddRazorComponents()
    .AddInteractiveWebAssemblyComponents();

// SignalR Hub para colaboración en tiempo real
builder.Services.AddSignalR();

// ...
app.MapHub<CollaborationHub>("/hubs/collaboration");

app.MapRazorComponents<App>()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
        typeof(Disnegativos.Shared._Imports).Assembly,
        typeof(Disnegativos.Web.Client._Imports).Assembly);
```

### 6.3 Interfaz IFormFactor

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
