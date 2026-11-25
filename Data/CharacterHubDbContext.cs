using CharacterHub.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterHub.Data;

public sealed class CharacterHubDbContext(DbContextOptions<CharacterHubDbContext> options)
    : DbContext(options)
{
    public DbSet<Personaje> Personajes => Set<Personaje>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Personaje>(entity =>
        {
            entity.ToTable("Personajes");
            entity.HasKey(personaje => personaje.Id);
            entity.Property(personaje => personaje.Nombre).HasMaxLength(120).IsRequired();
            entity.Property(personaje => personaje.Videojuego).HasMaxLength(120).IsRequired();
            entity.Property(personaje => personaje.Clase).HasMaxLength(80).IsRequired();
            entity.Property(personaje => personaje.ImagenUrl).HasMaxLength(2048).IsRequired();
        });
    }
}
