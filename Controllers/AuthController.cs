using CharacterHub.DTOs;
using CharacterHub.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CharacterHub.Controllers;

[ApiController]
[AllowAnonymous]
[Route("api/auth")]
[Produces("application/json")]
public sealed class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<LoginResponse> Login(LoginRequest request)
    {
        var result = authService.Login(request.Username, request.Password);
        return result is null ? Unauthorized() : Ok(result);
    }
}
