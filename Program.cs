using CharacterHub.Repositories;
using CharacterHub.Services;
using CharacterHub.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var persistenceProvider = builder.Configuration["Persistence:Provider"] ?? "Json";
if (string.Equals(persistenceProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
{
    var connectionString = builder.Configuration.GetConnectionString("CharacterHub");
    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "Configure ConnectionStrings:CharacterHub when Persistence:Provider is SqlServer.");
    }

    builder.Services.AddDbContext<CharacterHubDbContext>(options =>
        options.UseSqlServer(connectionString));
    builder.Services.AddScoped<IPersonajeRepository, SqlServerPersonajeRepository>();
}
else if (string.Equals(persistenceProvider, "Json", StringComparison.OrdinalIgnoreCase))
{
    var jsonPath = builder.Configuration["Persistence:JsonFilePath"] ?? "App_Data/personajes.json";
    builder.Services.AddSingleton<IPersonajeRepository>(_ =>
        new JsonPersonajeRepository(Path.GetFullPath(jsonPath)));
}
else
{
    throw new InvalidOperationException(
        "Persistence:Provider must be either Json or SqlServer.");
}

builder.Services.AddScoped<IPersonajeService, PersonajeService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
