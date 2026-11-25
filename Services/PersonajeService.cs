using CharacterHub.DTOs;
using CharacterHub.Models;
using CharacterHub.Repositories;

namespace CharacterHub.Services;

public sealed class PersonajeService(IPersonajeRepository repository) : IPersonajeService
{
    public async Task<IReadOnlyList<PersonajeResponse>> GetAllAsync(
        CancellationToken cancellationToken) =>
        (await repository.GetAllAsync(cancellationToken)).Select(ToResponse).ToArray();

    public async Task<PersonajeResponse?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        var personaje = await repository.GetByIdAsync(id, cancellationToken);
        return personaje is null ? null : ToResponse(personaje);
    }

    public async Task<IReadOnlyList<PersonajeResponse>> GetByVideojuegoAsync(
        string nombre,
        CancellationToken cancellationToken) =>
        (await repository.GetByVideojuegoAsync(nombre, cancellationToken))
        .Select(ToResponse)
        .ToArray();

    public async Task<IReadOnlyList<PersonajeResponse>> GetByMinimumLevelAsync(
        int nivel,
        CancellationToken cancellationToken) =>
        (await repository.GetByMinimumLevelAsync(nivel, cancellationToken))
        .Select(ToResponse)
        .ToArray();

    public async Task<PersonajeResponse> CreateAsync(
        CreatePersonajeRequest request,
        CancellationToken cancellationToken)
    {
        var personaje = new Personaje
        {
            Nombre = request.Nombre.Trim(),
            Videojuego = request.Videojuego.Trim(),
            Clase = request.Clase.Trim(),
            Nivel = request.Nivel,
            Poder = request.Poder,
            Disponible = true,
            ImagenUrl = request.ImagenUrl.Trim()
        };

        return ToResponse(await repository.AddAsync(personaje, cancellationToken));
    }

    public async Task<PersonajeResponse?> UpdateAsync(
        int id,
        UpdatePersonajeRequest request,
        CancellationToken cancellationToken)
    {
        var personaje = await repository.GetByIdAsync(id, cancellationToken);
        if (personaje is null)
        {
            return null;
        }

        personaje.Nombre = request.Nombre.Trim();
        personaje.Videojuego = request.Videojuego.Trim();
        personaje.Clase = request.Clase.Trim();
        personaje.Nivel = request.Nivel;
        personaje.Poder = request.Poder;
        personaje.Disponible = request.Disponible;
        personaje.ImagenUrl = request.ImagenUrl.Trim();

        return ToResponse(await repository.UpdateAsync(personaje, cancellationToken));
    }

    public Task<bool> DeleteAsync(int id, CancellationToken cancellationToken) =>
        repository.DeleteAsync(id, cancellationToken);

    private static PersonajeResponse ToResponse(Personaje personaje) =>
        new(
            personaje.Id,
            personaje.Nombre,
            personaje.Videojuego,
            personaje.Clase,
            personaje.Nivel,
            personaje.Poder,
            personaje.Disponible,
            personaje.ImagenUrl);
}
