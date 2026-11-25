using CharacterHub.DTOs;

namespace CharacterHub.Services;

public interface IPersonajeService
{
    Task<IReadOnlyList<PersonajeResponse>> GetAllAsync(CancellationToken cancellationToken);
    Task<PersonajeResponse?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IReadOnlyList<PersonajeResponse>> GetByVideojuegoAsync(
        string nombre,
        CancellationToken cancellationToken);
    Task<IReadOnlyList<PersonajeResponse>> GetByMinimumLevelAsync(
        int nivel,
        CancellationToken cancellationToken);
    Task<PersonajeResponse> CreateAsync(
        CreatePersonajeRequest request,
        CancellationToken cancellationToken);
    Task<PersonajeResponse?> UpdateAsync(
        int id,
        UpdatePersonajeRequest request,
        CancellationToken cancellationToken);
    Task<bool> DeleteAsync(int id, CancellationToken cancellationToken);
}
