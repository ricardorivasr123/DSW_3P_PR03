namespace CharacterHub.DTOs;

public sealed record PersonajeResponse(
    int Id,
    string Nombre,
    string Videojuego,
    string Clase,
    int Nivel,
    int Poder,
    bool Disponible,
    string ImagenUrl);
