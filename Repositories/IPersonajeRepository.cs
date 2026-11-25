using CharacterHub.Models;

namespace CharacterHub.Repositories;

public interface IPersonajeRepository
{
    Task<IReadOnlyList<Personaje>> GetAllAsync(CancellationToken cancellationToken);
    Task<Personaje?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<Personaje>> GetByVideojuegoAsync(
        string nombre,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<Personaje>> GetByMinimumLevelAsync(
        int nivel,
        CancellationToken cancellationToken);
    Task<Personaje> AddAsync(Personaje personaje, CancellationToken cancellationToken);
    Task<Personaje> UpdateAsync(Personaje personaje, CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
