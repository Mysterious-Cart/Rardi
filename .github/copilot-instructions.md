# GitHub Copilot Instructions for Rardi_V1

## 1. Project Overview

This is a .NET 8 Server-Side Blazor application (CHKS) with:
- EF Core for MySQL data access (context: `Data/Rardi/Rardi_Context.cs`).
- ASP.NET Core Identity on MySQL (`ApplicationIdentityDbContext`).
- MudBlazor UI components.
- SignalR hub (`InventoryNotificationHub`) for real-time inventory updates.
- REST endpoints under `Application/Controllers/Security` for authentication and user management.

## 2. Key Components & Structure

- **Program.cs**: Central DI setup, middleware (Auth, CORS, HeaderPropagation), SignalR mapping.
- **Data/Rardi/Rardi_Context.cs**: DbContext factory registration and model definitions via migrations.
- **Migrations/**: EF Core migrations with partial snapshot classes; keep naming and order.
- **Application/Services/**: Application services and hubs (e.g., `CartControlService`, `SecurityService`, `StockLogsTrackingService`, `InventoryNotificationHub`). Register new services here and add to DI in Program.cs.
- **Application/Filters/**: Global authorization filter pattern.
- **Application/Controllers/Security/**: REST endpoints for authentication and user management.
- **Domain/Entities/**: Domain records and mapping helpers organized by area (`Customer`, `Product`, `Transaction`, etc.).
- **Domain/Enums/**: Domain enum types used by entities and data models.
- **Infrastructure/Repository/**: External integrations (e.g., `VehicleAPI`).
- **Pages/**: Blazor components for UI, organized by area.
- **wwwroot/**: Static assets (JS, CSS) for Blazor components.
- **Data/Rardi/Models/**: EF Core data models organized by area (`Customer`, `Product`, `Transaction`, etc.).

## 3. Developer Workflows

### Build & Run
```powershell
cd f:\Projects\Rardi_V1\src
# Build
dotnet build CHKS.csproj -c Debug
# Run
dotnet run --project CHKS.csproj
# Watch
dotnet watch --project CHKS.csproj
```

### Docker
```powershell
# From workspace root
docker build -t Rardi -f Dockerfile .;
docker run -p 8080:8080 -p 8081:8081 Rardi
```

### Database Migrations
```powershell
# Add a new migration
dotnet ef migrations add <Name> --project CHKS.csproj --startup-project CHKS
# Apply to database
dotnet ef database update --project CHKS.csproj --startup-project CHKS
```

### Configuration Overrides
- Connection strings in `appsettings.json` under **ConnectionStrings:development**.
- Use `appsettings.Development.json` for local secrets.
- CORS policy named **AllowAll** is enabled globally in Program.cs.

## 4. Conventions & Patterns

- **Namespace Layout**:
	- `CHKS.Application.Services`
	- `CHKS.Application.Controllers`
	- `CHKS.Application.Filters`
	- `CHKS.Domain.Entities`
	- `CHKS.Domain.Mappers`
	- `CHKS.Domain.Enums`
- **Folder/Namespace Alignment**: New files should align to the layered folder and namespace conventions above.
- **Service Naming**: Classes suffixed `Service` or `ControlService`, registered as scoped by default.
- **HTTP Client**: Named client `CHKS` with cookie header propagation (`AddHeaderPropagation`).
- **Context Factory**: Use `AddDbContextFactory<Rardi_Context>` for EF Core contexts in scoped/background scenarios.
- **SignalR**: Hub endpoint configured at `/inventorylogs`.
- **Identity**: `ApplicationUser` and `ApplicationRole` under Models; default token providers enabled.
- **Authorization**: Apply `[Authorize]` on Blazor pages and controllers; fallback uses `_Host.cshtml`.

## 5. Integration Points

- **SecurityService**: Handles login/logout flows; uses `ApplicationAuthenticationStateProvider` for Blazor auth state.
- **HeaderPropagation**: Ensures cookies are forwarded to API calls in Blazor.

---