# Disnegativos — Documento de Diseño y Arquitectura

## 1. Visión General

**Disnegativos** es una aplicación multiplataforma de análisis deportivo que permite a los usuarios registrar, analizar y generar informes sobre eventos deportivos. La aplicación se ejecuta en **Windows, Android, iOS y macOS** mediante .NET 10 MAUI Blazor Hybrid, y también ofrece una versión **web** accesible desde el navegador.

### 1.1 Objetivos Clave

| Objetivo | Descripción |
|---|---|
| **Multiplataforma** | Windows, Android, iOS, macOS + Web |
| **Offline-first (MAUI)** | Trabajo en desconectado con SQLite local y sincronización automática al reconectar |
| **Acceso directo (Web)** | La versión web accede directamente a la base de datos SQLite del servidor |
| **Colaboración en tiempo real** | SignalR Hub para que múltiples usuarios colaboren simultáneamente |
| **Multi-idioma** | Soporte completo de internacionalización (i18n) |
| **Multi zona horaria** | Fechas almacenadas en UTC, presentadas en la zona del usuario |
| **Testable** | Tests unitarios de servicios y componentes Blazor |
| **Moderna** | .NET 10, MAUI Blazor Hybrid, Radzen, EF Core Code-First |

### 1.2 Stack Tecnológico y Arquitectura de Datos

> **Clave:** Las **Blazor Pages viven en `Disnegativos.Shared`** y se renderizan en ambos hosts. Un usuario sin la app MAUI instalada accede a las mismas páginas a través del navegador web.

```
    ┌──────────────────────────────────────────────────────────────────┐
    │                      Disnegativos.Shared (RCL)                    │
    │   Blazor Pages · Services · EF Core · DTOs · Layout · i18n       │
    │   ─────────────── Código compartido al 100% ──────────────────   │
    └───────────────┬──────────────────────────────┬───────────────────┘
                    │  se renderiza en              │  se renderiza en
                    ▼                               ▼
  ┌─────────────────────────────────┐  ┌──────────────────────────────────────┐
  │  DISPOSITIVO MAUI (Disnegativos)│  │  SERVIDOR WEB (Disnegativos.Web)     │
  │                                 │  │                                      │
  │  ┌───────────────────────────┐  │  │  ┌────────────────────────────────┐  │
  │  │ Blazor Pages (del Shared) │  │  │  │  Blazor Pages (del Shared)    │  │
  │  │ renderizadas via WebView  │  │  │  │  renderizadas via SSR + WASM  │  │
  │  └─────────┬─────────────────┘  │  │  └──────────┬─────────────────── │  │
  │            │                    │  │             │                     │  │
  │  ┌─────────┴─────────────────┐  │  │  ┌──────────┴──────────────────┐ │  │
  │  │ Services / EF Core        │  │  │  │ Services / EF Core          │ │  │
  │  └─────────┬─────────────────┘  │  │  └──────────┬──────────────────┘ │  │
  │            │                    │  │             │                     │  │
  │  ┌─────────┴─────────────────┐  │  │  ┌──────────┴──────────────────┐ │  │
  │  │  SQLite LOCAL              │  │  │  │  SQLite del SERVIDOR       │ │  │
  │  │  (réplica offline)         │  │  │  │  (fuente de verdad)        │ │  │
  │  └─────────┬─────────────────┘  │  │  └─────────────────────────────┘ │  │
  │            │                    │  │                                    │  │
  │  ┌─────────┴─────────────────┐  │  │  ┌──────────────────────────────┐ │  │
  │  │ SyncService + SyncLog     │──┼──┼─▶│  Sync API (REST)            │ │  │
  │  │ (Push/Pull al reconectar) │  │  │  │  POST /api/sync/push        │ │  │
  │  └───────────────────────────┘  │  │  │  GET  /api/sync/pull        │ │  │
  │                                 │  │  └──────────────────────────────┘ │  │
  │  ┌───────────────────────────┐  │  │  ┌──────────────────────────────┐ │  │
  │  │ SignalR Client            │◄─┼──┼──│  SignalR Hub                │ │  │
  │  │ (recibe notificaciones)   │  │  │  │  (difunde cambios)          │ │  │
  │  └───────────────────────────┘  │  │  └──────────────────────────────┘ │  │
  └─────────────────────────────────┘  └──────────────────────────────────────┘
           Funciona OFFLINE ✅                  Requiere conexión ❌
        Sync cuando hay conexión             Acceso DIRECTO a la BD
```

### 1.3 Dos Modelos de Acceso a Datos

| | MAUI (Offline-first) | Web (Directo) |
|---|---|---|
| **UI** | **Mismas Blazor Pages** del Shared, renderizadas en WebView | **Mismas Blazor Pages** del Shared, renderizadas via SSR + WASM |
| **BD** | SQLite **local** en el dispositivo | SQLite del **servidor** |
| **Lecturas** | Instantáneas (local) | Directas contra servidor |
| **Escrituras** | Locales + `SyncLog` | Directas contra servidor |
| **Sincronización** | Push/Pull vía API REST al reconectar | No necesita (accede directamente) |
| **Colaboración** | Recibe notificaciones de cambios vía SignalR Client | Recibe notificaciones de cambios vía SignalR |
| **Sin conexión** | ✅ Funciona 100% offline | ❌ Requiere conexión al servidor |

---

## 2. Estructura de la Solución

```
Disnegativos.slnx
│
├── Disnegativos/                    # Host MAUI Blazor Hybrid (Windows, Android, iOS, macOS)
│   ├── App.xaml / App.xaml.cs
│   ├── MainPage.xaml / MainPage.xaml.cs
│   ├── MauiProgram.cs               # Configuración DI, SQLite local, SignalR Client
│   ├── Components/
│   ├── Services/                    # Servicios específicos de plataforma
│   │   ├── FormFactor.cs            # Implementación IFormFactor
│   │   ├── ConnectivityService.cs   # Detección de conectividad (MAUI IConnectivity)
│   │   └── SyncService.cs           # Motor de sincronización Push/Pull
│   ├── Platforms/                   # Código específico por plataforma
│   ├── Resources/                   # Recursos nativos (fuentes, imágenes, etc.)
│   └── wwwroot/                     # Assets estáticos para Blazor Hybrid
│
├── Disnegativos.Shared/             # RCL — Razor Class Library compartida
│   ├── Models/                      # Modelos base (BaseEntity)
│   ├── Data/
│   │   ├── DisnegativosDbContext.cs  # EF Core DbContext (SQLite)
│   │   ├── Entities/                # Entidades EF Core completas
│   │   ├── Configurations/          # Fluent API
│   │   ├── Migrations/              # Migraciones EF Core
│   │   └── Repositories/            # Repositorio genérico
│   ├── DTOs/                        # DTOs
│   ├── Enums/                       # Enumeraciones (SyncStatus, etc.)
│   ├── Interfaces/                  # Interfaces comunes
│   ├── Services/
│   │   ├── Interfaces/              # IEventService, ISyncService, etc.
│   │   └── Implementations/         # EventService, TimeZoneService, etc.
│   ├── Hubs/                        # SignalR Hubs (definición compartida)
│   │   └── ICollaborationHub.cs     # Interfaz del hub para tipado fuerte
│   ├── DependencyInjection/         # ServiceCollectionExtensions
│   ├── Pages/                       # Páginas Blazor compartidas
│   ├── Layout/                      # MainLayout, NavMenu
│   ├── Resources/                   # Archivos .resx para i18n
│   └── wwwroot/                     # CSS y assets compartidos
│
├── Disnegativos.Web/                # Host ASP.NET Blazor Web (servidor)
│   ├── Program.cs                   # Configuración DI, SQLite servidor, SignalR Hub
│   ├── Hubs/                        # SignalR Hub implementation
│   │   └── CollaborationHub.cs      # Hub de colaboración en tiempo real
│   ├── Controllers/
│   │   └── SyncController.cs        # API REST para sincronización MAUI
│   ├── Components/                  # App.razor y _Imports del host web
│   ├── Services/                    # Servicios específicos Web
│   └── appsettings.json
│
└── Disnegativos.Web.Client/         # Proyecto WASM del cliente interactivo
    ├── Program.cs
    ├── Services/
    └── _Imports.razor
```

### 2.1 Responsabilidad de cada Proyecto

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `Disnegativos` | MAUI Blazor Hybrid | Host nativo. **SQLite local** en `FileSystem.AppDataDirectory`. Motor de sincronización Push/Pull. Cliente SignalR para recibir notificaciones de cambios en tiempo real |
| `Disnegativos.Shared` | Razor Class Library | **Proyecto central.** Contiene modelos, entidades, DbContext, servicios, DTOs, enums, interfaces, páginas Blazor compartidas, layout, i18n, registro DI centralizado e interfaz del SignalR Hub |
| `Disnegativos.Web` | Blazor Web App (Server) | Host web con SSR + Interactive WASM. **Accede directamente a la SQLite del servidor.** Expone el SignalR Hub y la API REST de sincronización para los dispositivos MAUI |
| `Disnegativos.Web.Client` | Blazor WebAssembly | Proyecto WASM interactivo para renderizado del lado cliente |

### 2.2 Registro de Dependencias Centralizado

```csharp
// Disnegativos.Shared/DependencyInjection/ServiceCollectionExtensions.cs
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDisnegativosSharedServices(
        this IServiceCollection services, string connectionString)
    {
        services.AddRadzenComponents();
        services.AddDbContext<DisnegativosDbContext>(options =>
            options.UseSqlite(connectionString));
        services.AddScoped<ITimeZoneService, TimeZoneService>();
        services.AddScoped<IEventService, EventService>();
        return services;
    }
}
```

Se usa en ambos hosts pero con **conexiones diferentes**:

```csharp
// ═══ Disnegativos/MauiProgram.cs (MAUI — SQLite LOCAL) ═══
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
builder.Services.AddDisnegativosSharedServices($"Data Source={dbPath}");

// ═══ Disnegativos.Web/Program.cs (Web — SQLite del SERVIDOR) ═══
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? "Data Source=disnegativos.db";
builder.Services.AddDisnegativosSharedServices(connectionString);
```

---

## 3. Patrón Container / Presentational en Blazor

Todas las páginas Blazor siguen el patrón **Container/Presentational**:

```
Disnegativos.Shared/Pages/
├── Events/
│   ├── EventsPage.razor              ← Container (orquesta datos)
│   ├── EventsPage.razor.cs           ← Code-behind del Container
│   └── Components/
│       ├── EventListView.razor        ← Presentational (muestra lista)
│       ├── EventDetailView.razor      ← Presentational (muestra detalle)
│       └── EventFormView.razor        ← Presentational (formulario)
```

### Container (Smart Component)
- Inyecta servicios vía `@inject` / `[Inject]`
- Gestiona estado y ciclo de vida
- Llama a servicios de negocio
- Pasa datos a Presentationals vía `[Parameter]`
- Maneja callbacks de los Presentationals vía `EventCallback`
- **Se suscribe al SignalR Hub para recibir actualizaciones en tiempo real**

### Presentational (Dumb Component)
- **Sin inyección de servicios** (excepto `NavigationManager` si es necesario)
- Recibe datos sólo por `[Parameter]`
- Emite eventos vía `EventCallback`
- Totalmente testeable con bUnit sin mocks de servicios
- Usa componentes Radzen para la UI

```csharp
// === CONTAINER ===
// EventsPage.razor.cs
namespace Disnegativos.Shared.Pages.Events;

public partial class EventsPage : ComponentBase, IAsyncDisposable
{
    [Inject] private IEventService EventService { get; set; } = default!;
    [Inject] private HubConnection? HubConnection { get; set; } // SignalR (nullable para MAUI offline)

    private List<EventDto> _events = [];
    private List<TeamDto> _availableTeams = [];
    private bool _isLoading = true;
    private bool _isEditing = false;
    private EventEditDto _editingEvent = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsAsync();
        _availableTeams = await EventService.GetAvailableTeamsAsync();

        // Suscribirse a cambios en tiempo real
        if (HubConnection is not null)
        {
            HubConnection.On<string, Guid>("EntityChanged", async (entityType, entityId) =>
            {
                if (entityType == nameof(Event))
                {
                    await LoadEventsAsync();
                    await InvokeAsync(StateHasChanged);
                }
            });
        }
    }

    private async Task LoadEventsAsync()
    {
        _isLoading = true;
        _events = await EventService.GetAllEventsAsync();
        _isLoading = false;
    }

    private async Task HandleSaveAsync(EventEditDto dto)
    {
        await EventService.SaveEventAsync(dto);

        // Notificar a otros usuarios del cambio
        if (HubConnection is not null)
            await HubConnection.InvokeAsync("NotifyEntityChanged", nameof(Event), dto.Id);

        await LoadEventsAsync();
    }

    private async Task HandleDeleteAsync(Guid eventId)
    {
        await EventService.DeleteEventAsync(eventId);

        // Notificar a otros usuarios del cambio
        if (HubConnection is not null)
            await HubConnection.InvokeAsync("NotifyEntityChanged", nameof(Event), eventId);

        await LoadEventsAsync();
    }

    public async ValueTask DisposeAsync()
    {
        // Cleanup de suscripciones SignalR si es necesario
    }
}

// === PRESENTATIONAL ===
// EventListView.razor
@code {
    [Parameter] public List<EventDto> Events { get; set; } = [];
    [Parameter] public bool IsLoading { get; set; }
    [Parameter] public EventCallback OnAdd { get; set; }
    [Parameter] public EventCallback<Guid> OnEdit { get; set; }
    [Parameter] public EventCallback<Guid> OnDelete { get; set; }
}
```

---

## 4. Flujo de Datos por Plataforma

### 4.1 Flujo Web (acceso directo)

```
Usuario Web → Blazor Page → Service → EF Core → SQLite del servidor
                                         ↓
                                  SignalR Hub → Notifica a TODOS los clientes conectados
```

1. El usuario realiza una operación (crear, editar, borrar)
2. El servicio escribe directamente en la SQLite del servidor
3. Tras la escritura, el componente Container invoca el SignalR Hub
4. El Hub difunde el cambio a todos los clientes conectados (Web y MAUI)
5. Los clientes suscritos recargan los datos afectados

### 4.2 Flujo MAUI Online (con conexión)

```
Usuario MAUI → Blazor Page → Service → EF Core → SQLite LOCAL
                                          ↓
                                   SyncLog (cambio pendiente)
                                          ↓
                                   SyncService (Push al servidor)
                                          ↓
                              POST /api/sync/push → Servidor aplica cambio
                                          ↓
                                   SignalR Hub → Notifica a TODOS
```

1. El usuario realiza una operación
2. El servicio escribe en la SQLite **local** del dispositivo
3. El interceptor de EF Core registra el cambio en `SyncLog`
4. El `SyncService` detecta que hay conexión y envía el cambio al servidor
5. El servidor aplica el cambio en su SQLite y notifica vía SignalR
6. Todos los demás clientes se actualizan

### 4.3 Flujo MAUI Offline (sin conexión)

```
Usuario MAUI → Blazor Page → Service → EF Core → SQLite LOCAL
                                          ↓
                                   SyncLog (cambio pendiente)
                                          ↓
                                   (Cola de espera)
                                          ↓
              ... usuario recupera conexión ...
                                          ↓
                              SyncService detecta conectividad
                                          ↓
                              Push de todos los SyncLog pendientes
                                          ↓
                              Pull de cambios del servidor
```

1. El usuario trabaja normalmente (toda la UI funciona contra datos locales)
2. Los cambios se acumulan en `SyncLog`
3. Al recuperar conexión, `SyncService` hace Push y Pull automáticamente
4. Si hay conflictos, se resuelven según la estrategia configurada
