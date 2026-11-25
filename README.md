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
