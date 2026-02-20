# Disnegativos — Documento de Diseño y Arquitectura

## 1. Visión General

**Disnegativos** es una aplicación multiplataforma de análisis deportivo que permite a los usuarios registrar, analizar y generar informes sobre eventos deportivos. La aplicación se ejecuta en **Windows, Android, iOS y macOS** mediante .NET 10 MAUI Blazor Hybrid, y también ofrece una versión **web** accesible desde el navegador.

### 1.1 Objetivos Clave

| Objetivo | Descripción |
|---|---|
| **Multiplataforma** | Windows, Android, iOS, macOS + Web |
| **Offline-first** | Trabajo en desconectado con SQLite local y sincronización al reconectar |
| **Multi-idioma** | Soporte completo de internacionalización (i18n) |
| **Multi zona horaria** | Fechas almacenadas en UTC, presentadas en la zona del usuario |
| **Testable** | Tests unitarios de servicios y componentes Blazor |
| **Moderna** | .NET 10, MAUI Blazor Hybrid, Radzen, EF Core Code-First |

### 1.2 Stack Tecnológico

```
┌─────────────────────────────────────────────────────┐
│                    PRESENTACIÓN                      │
│  Blazor Components (Radzen) — Container/Presentational│
├─────────────────────────────────────────────────────┤
│                  LÓGICA DE NEGOCIO                   │
│         Services · Validators · Mappers              │
├─────────────────────────────────────────────────────┤
│                  ACCESO A DATOS                      │
│      EF Core (Code-First) + SQLite                   │
├─────────────────────────────────────────────────────┤
│               SINCRONIZACIÓN                         │
│     Change Tracking · Conflict Resolution · Queue    │
├─────────────────────────────────────────────────────┤
│                  PLATAFORMA                          │
│  .NET 10 MAUI Blazor Hybrid  │  ASP.NET Blazor Web  │
└─────────────────────────────────────────────────────┘
```

---

## 2. Estructura de la Solución

```
Disnegativos.slnx
│
├── Disnegativos/                    # Host MAUI Blazor Hybrid (Windows, Android, iOS, macOS)
│   ├── App.xaml / App.xaml.cs
│   ├── MainPage.xaml / MainPage.xaml.cs
│   ├── MauiProgram.cs               # Configuración DI y servicios para MAUI
│   ├── Components/
│   ├── Services/                    # Servicios específicos de plataforma (FormFactor, etc.)
│   ├── Platforms/                   # Código específico por plataforma
│   ├── Resources/                   # Recursos nativos (fuentes, imágenes, etc.)
│   └── wwwroot/                     # Assets estáticos para Blazor Hybrid
│
├── Disnegativos.Shared/             # RCL — Razor Class Library compartida
│   ├── Models/                      # Modelos base (BaseEntity)
│   ├── Data/
│   │   ├── DisnegativosDbContext.cs  # EF Core DbContext (SQLite)
│   │   ├── Entities/                # Entidades EF Core (Tenant, Customer, Event, Team, etc.)
│   │   ├── Configurations/          # Fluent API (BaseEntityConfiguration)
│   │   └── Repositories/            # Repositorio genérico
│   ├── DTOs/                        # DTOs (EventDto, EventEditDto, TeamDto, etc.)
│   ├── Enums/                       # Enumeraciones (SyncStatus, etc.)
│   ├── Interfaces/                  # Interfaces comunes (IRepository, ITimeZoneService)
│   ├── Services/
│   │   ├── Interfaces/              # Interfaces de servicios (IEventService)
│   │   └── Implementations/         # Implementaciones (EventService, TimeZoneService)
│   ├── DependencyInjection/         # ServiceCollectionExtensions (registro centralizado DI)
│   ├── Pages/                       # Páginas Blazor compartidas
│   │   └── Events/                  # Página de Eventos (Container + Presentational)
│   ├── Layout/                      # MainLayout, NavMenu
│   ├── Resources/                   # Archivos .resx para i18n (SharedResources)
│   ├── Routes.razor
│   ├── _Imports.razor
│   └── wwwroot/                     # CSS y assets compartidos
│
├── Disnegativos.Web/                # Host ASP.NET Blazor Web (servidor)
│   ├── Program.cs                   # Configuración DI y middleware para Web
│   ├── Components/                  # App.razor y _Imports del host web
│   ├── Services/                    # Servicios específicos Web (FormFactor)
│   └── appsettings.json
│
└── Disnegativos.Web.Client/         # Proyecto WASM del cliente interactivo
    ├── Program.cs                   # Registro de servicios del lado WASM
    ├── Services/                    # Servicios cliente (FormFactor WASM)
    └── _Imports.razor
```

### 2.1 Responsabilidad de cada Proyecto

| Proyecto | Tipo | Responsabilidad |
|---|---|---|
| `Disnegativos` | MAUI Blazor Hybrid | Host nativo para Windows/Android/iOS/macOS. Configura DI con `MauiProgram.cs`, SQLite local en `FileSystem.AppDataDirectory` |
| `Disnegativos.Shared` | Razor Class Library | **Proyecto central.** Contiene modelos, entidades, DbContext, servicios, DTOs, enums, interfaces, páginas Blazor compartidas, layout, i18n y registro DI centralizado |
| `Disnegativos.Web` | Blazor Web App (Server) | Host web con SSR + Interactive WebAssembly. Configura DI con `Program.cs` |
| `Disnegativos.Web.Client` | Blazor WebAssembly | Proyecto WASM interactivo para el renderizado del lado cliente |

### 2.2 Registro de Dependencias Centralizado

El método de extensión `AddDisnegativosSharedServices` en `Disnegativos.Shared.DependencyInjection` registra todos los servicios compartidos:

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

Se usa tanto en **MAUI** como en **Web**:

```csharp
// Disnegativos/MauiProgram.cs
var dbPath = Path.Combine(FileSystem.AppDataDirectory, "disnegativos.db");
builder.Services.AddDisnegativosSharedServices($"Data Source={dbPath}");

// Disnegativos.Web/Program.cs
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

public partial class EventsPage : ComponentBase
{
    [Inject] private IEventService EventService { get; set; } = default!;

    private List<EventDto> _events = [];
    private List<TeamDto> _availableTeams = [];
    private bool _isLoading = true;
    private bool _isEditing = false;
    private EventEditDto _editingEvent = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadEventsAsync();
        _availableTeams = await EventService.GetAvailableTeamsAsync();
    }

    private async Task LoadEventsAsync()
    {
        _isLoading = true;
        _events = await EventService.GetAllEventsAsync();
        _isLoading = false;
    }

    private async Task HandleDeleteAsync(Guid eventId)
    {
        await EventService.DeleteEventAsync(eventId);
        await LoadEventsAsync();
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
