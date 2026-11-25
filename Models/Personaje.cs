namespace CharacterHub.Models;

public sealed class Personaje
{
    public int Id { get; set; }
    public required string Nombre { get; set; }
    public required string Videojuego { get; set; }
    public required string Clase { get; set; }
    public int Nivel { get; set; }
    public int Poder { get; set; }
    public bool Disponible { get; set; }
    public required string ImagenUrl { get; set; }
}
