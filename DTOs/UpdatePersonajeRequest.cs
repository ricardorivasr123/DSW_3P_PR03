using System.ComponentModel.DataAnnotations;
using CharacterHub.Validation;

namespace CharacterHub.DTOs;

public sealed class UpdatePersonajeRequest
{
    [NotBlank]
    [StringLength(120)]
    public required string Nombre { get; init; }

    [NotBlank]
    [StringLength(120)]
    public required string Videojuego { get; init; }

    [NotBlank]
    [StringLength(80)]
    public required string Clase { get; init; }

    [Range(1, 100)]
    public int Nivel { get; init; }

    [Range(1, int.MaxValue)]
    public int Poder { get; init; }

    public bool Disponible { get; init; }

    [Required]
    [StringLength(2048)]
    [ImageHttpUrl]
    public required string ImagenUrl { get; init; }
}
