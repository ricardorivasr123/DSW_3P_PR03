using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CharacterHub.Data;

public sealed class CharacterHubDbContextFactory
    : IDesignTimeDbContextFactory<CharacterHubDbContext>
{
    public CharacterHubDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("CharacterHub")
            ?? throw new InvalidOperationException(
                "Configure ConnectionStrings:CharacterHub before creating or applying migrations.");

        var options = new DbContextOptionsBuilder<CharacterHubDbContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new CharacterHubDbContext(options);
    }
}
