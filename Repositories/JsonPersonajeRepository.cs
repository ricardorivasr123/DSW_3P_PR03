using System.Text.Json;
using CharacterHub.Models;

namespace CharacterHub.Repositories;

public sealed class JsonPersonajeRepository(string filePath) : IPersonajeRepository
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true
    };

    private readonly SemaphoreSlim _gate = new(1, 1);

    public async Task<IReadOnlyList<Personaje>> GetAllAsync(CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return (await ReadAsync(cancellationToken))
                .OrderBy(personaje => personaje.Id)
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Personaje?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return (await ReadAsync(cancellationToken)).FirstOrDefault(personaje => personaje.Id == id);
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<Personaje>> GetByVideojuegoAsync(
        string nombre,
        CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return (await ReadAsync(cancellationToken))
                .Where(personaje => string.Equals(
                    personaje.Videojuego,
                    nombre,
                    StringComparison.OrdinalIgnoreCase))
                .OrderBy(personaje => personaje.Id)
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<IReadOnlyList<Personaje>> GetByMinimumLevelAsync(
        int nivel,
        CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            return (await ReadAsync(cancellationToken))
                .Where(personaje => personaje.Nivel >= nivel)
                .OrderBy(personaje => personaje.Id)
                .ToArray();
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Personaje> AddAsync(Personaje personaje, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var personajes = await ReadAsync(cancellationToken);
            personaje.Id = personajes.Count == 0 ? 1 : personajes.Max(item => item.Id) + 1;
            personajes.Add(personaje);
            await WriteAsync(personajes, cancellationToken);
            return personaje;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<Personaje> UpdateAsync(
        Personaje personaje,
        CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var personajes = await ReadAsync(cancellationToken);
            var index = personajes.FindIndex(item => item.Id == personaje.Id);
            if (index < 0)
            {
                throw new InvalidOperationException($"Personaje {personaje.Id} no existe.");
            }

            personajes[index] = personaje;
            await WriteAsync(personajes, cancellationToken);
            return personaje;
        }
        finally
        {
            _gate.Release();
        }
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await _gate.WaitAsync(cancellationToken);
        try
        {
            var personajes = await ReadAsync(cancellationToken);
            var removed = personajes.RemoveAll(personaje => personaje.Id == id) > 0;
            if (removed)
            {
                await WriteAsync(personajes, cancellationToken);
            }

            return removed;
        }
        finally
        {
            _gate.Release();
        }
    }

    private async Task<List<Personaje>> ReadAsync(CancellationToken cancellationToken)
    {
        if (!File.Exists(filePath))
        {
            return [];
        }

        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<List<Personaje>>(
                   stream,
                   JsonOptions,
                   cancellationToken)
               ?? [];
    }

    private async Task WriteAsync(
        List<Personaje> personajes,
        CancellationToken cancellationToken)
    {
        var directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryPath = $"{filePath}.{Guid.NewGuid():N}.tmp";
        try
        {
            await using (var stream = File.Create(temporaryPath))
            {
                await JsonSerializer.SerializeAsync(stream, personajes, JsonOptions, cancellationToken);
            }

            File.Move(temporaryPath, filePath, true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }
}
