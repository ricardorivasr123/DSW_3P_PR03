using System.Text;
using CharacterHub.Repositories;
using CharacterHub.Services;
using CharacterHub.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

var jwtSecret = builder.Configuration["Jwt:Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
{
    throw new InvalidOperationException(
        "Configure Jwt:Secret with at least 32 UTF-8 bytes using an environment variable or user secrets.");
}

if (string.IsNullOrWhiteSpace(builder.Configuration["DemoUser:Username"]) ||
    string.IsNullOrWhiteSpace(builder.Configuration["DemoUser:Password"]))
{
    throw new InvalidOperationException(
        "Configure DemoUser:Username and DemoUser:Password using environment variables or user secrets.");
}

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "CharacterHub API",
        Version = "v1",
        Description = "API REST para administrar personajes de videojuegos."
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa el token JWT obtenido en /api/auth/login."
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", document),
            new List<string>()
        }
    });
});

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

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "CharacterHub",
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "CharacterHubClient",
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromSeconds(30)
        };
    });
builder.Services.AddAuthorization();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options =>
{
    options.AddPolicy("EvaluationClient", policy =>
    {
        if (allowedOrigins.Length > 0)
        {
            policy.WithOrigins(allowedOrigins)
                .AllowAnyHeader()
                .AllowAnyMethod();
        }
    });
});

builder.Services.AddScoped<IPersonajeService, PersonajeService>();
builder.Services.AddScoped<IAuthService, AuthService>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("EvaluationClient");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();
