using System.ComponentModel.DataAnnotations;

namespace CharacterHub.DTOs;

public sealed class LoginRequest
{
    [Required]
    public required string Username { get; init; }

    [Required]
    public required string Password { get; init; }
}
