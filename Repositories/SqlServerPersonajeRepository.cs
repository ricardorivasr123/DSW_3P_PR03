using CharacterHub.Data;
using CharacterHub.Models;
using Microsoft.EntityFrameworkCore;

namespace CharacterHub.Repositories;

public sealed class SqlServerPersonajeRepository(CharacterHubDbContext dbContext)
    : IPersonajeRepository
{
    public async Task<IReadOnlyList<Personaje>> GetAllAsync(CancellationToken cancellationToken) =>
        await dbContext.Personajes
            .AsNoTracking()
            .OrderBy(personaje => personaje.Id)
            .ToListAsync(cancellationToken);

    public Task<Personaje?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        dbContext.Personajes.FirstOrDefaultAsync(
            personaje => personaje.Id == id,
            cancellationToken);

    public async Task<IReadOnlyList<Personaje>> GetByVideojuegoAsync(
        string nombre,
        CancellationToken cancellationToken) =>
        await dbContext.Personajes
            .AsNoTracking()
            .Where(personaje => personaje.Videojuego == nombre)
            .OrderBy(personaje => personaje.Id)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Personaje>> GetByMinimumLevelAsync(
        int nivel,
        CancellationToken cancellationToken) =>
        await dbContext.Personajes
            .AsNoTracking()
            .Where(personaje => personaje.Nivel >= nivel)
            .OrderBy(personaje => personaje.Id)
            .ToListAsync(cancellationToken);

    public async Task<Personaje> AddAsync(
        Personaje personaje,
        CancellationToken cancellationToken)
    {
        dbContext.Personajes.Add(personaje);
        await dbContext.SaveChangesAsync(cancellationToken);
        return personaje;
    }

    public async Task<Personaje> UpdateAsync(
        Personaje personaje,
        CancellationToken cancellationToken)
    {
        dbContext.Personajes.Update(personaje);
        await dbContext.SaveChangesAsync(cancellationToken);
        return personaje;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        var personaje = await dbContext.Personajes
            .FirstOrDefaultAsync(item => item.Id == id, cancellationToken);
        if (personaje is null)
        {
            return false;
        }

        dbContext.Personajes.Remove(personaje);
        await dbContext.SaveChangesAsync(cancellationToken);
        return true;
    }
}
