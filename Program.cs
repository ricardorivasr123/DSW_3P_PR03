using CharacterHub.Repositories;
using CharacterHub.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var jsonPath = builder.Configuration["Persistence:JsonFilePath"] ?? "App_Data/personajes.json";
builder.Services.AddSingleton<IPersonajeRepository>(_ =>
    new JsonPersonajeRepository(Path.GetFullPath(jsonPath)));

builder.Services.AddScoped<IPersonajeService, PersonajeService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.MapControllers();
app.Run();
