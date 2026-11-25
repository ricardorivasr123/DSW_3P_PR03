# CharacterHub

API REST para administrar personajes de videojuegos con MVC, Service y Repository. Requiere .NET 10 SDK.

La persistencia inicial utiliza `App_Data/personajes.json`. El archivo se crea al registrar el primer personaje y conserva los datos después de reiniciar la aplicación.

Ejecuta `dotnet run` y abre `/swagger` para probar los endpoints.

## SQL Server (segunda etapa)

Configura la cadena de conexión y selecciona el proveedor sin modificar Controller, Service ni `IPersonajeRepository`:

```powershell
$env:Persistence__Provider = "SqlServer"
$env:ConnectionStrings__CharacterHub = "Server=.\SQLEXPRESS;Database=CharacterHubDb;Trusted_Connection=True;TrustServerCertificate=True"
dotnet ef database update
dotnet run
```

La migración inicial se encuentra en `Data/Migrations`. Para volver al JSON, establece `Persistence__Provider=Json`.

## Autenticación JWT

La clave de firma y las credenciales de demostración se configuran externamente; no las guardes en el repositorio. En PowerShell:

```powershell
$jwtBytes = New-Object byte[] 32
$rng = [Security.Cryptography.RandomNumberGenerator]::Create()
$rng.GetBytes($jwtBytes)
$env:Jwt__Secret = [Convert]::ToBase64String($jwtBytes)
$rng.Dispose()
$env:DemoUser__Username = "demo"
$env:DemoUser__Password = "elige-una-clave-local"
dotnet run
```

Inicia sesión con `POST /api/auth/login`, copia `accessToken` y utiliza el esquema Bearer en Swagger. Todas las rutas `/api/personajes` requieren un token válido. El vencimiento se configura con `Jwt__ExpirationMinutes` (1 a 1440; predeterminado 60).

## CORS

Configura el origen exacto del cliente de evaluación indicado por el docente:

```powershell
$env:Cors__AllowedOrigins__0 = "http://localhost:3000"
```

La API permite los métodos y encabezados necesarios. Sin orígenes configurados, no habilita solicitudes CORS externas.
