using CharacterHub.DTOs;
using CharacterHub.Services;
using Microsoft.AspNetCore.Mvc;

namespace CharacterHub.Controllers;

[ApiController]
[Route("api/personajes")]
[Produces("application/json")]
public sealed class PersonajesController(IPersonajeService service) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<PersonajeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonajeResponse>>> GetAll(
        CancellationToken cancellationToken) =>
        Ok(await service.GetAllAsync(cancellationToken));

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PersonajeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonajeResponse>> GetById(
        int id,
        CancellationToken cancellationToken)
    {
        var personaje = await service.GetByIdAsync(id, cancellationToken);
        return personaje is null ? NotFound() : Ok(personaje);
    }

    [HttpGet("videojuego/{nombre}")]
    [ProducesResponseType(typeof(IReadOnlyList<PersonajeResponse>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<PersonajeResponse>>> GetByVideojuego(
        string nombre,
        CancellationToken cancellationToken) =>
        Ok(await service.GetByVideojuegoAsync(nombre, cancellationToken));

    [HttpGet("nivel/{nivel:int}")]
    [ProducesResponseType(typeof(IReadOnlyList<PersonajeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IReadOnlyList<PersonajeResponse>>> GetByMinimumLevel(
        int nivel,
        CancellationToken cancellationToken)
    {
        if (nivel is < 1 or > 100)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Nivel inválido",
                Detail = "El nivel debe estar entre 1 y 100.",
                Status = StatusCodes.Status400BadRequest
            });
        }

        return Ok(await service.GetByMinimumLevelAsync(nivel, cancellationToken));
    }

    [HttpPost]
    [ProducesResponseType(typeof(PersonajeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PersonajeResponse>> Create(
        CreatePersonajeRequest request,
        CancellationToken cancellationToken)
    {
        var personaje = await service.CreateAsync(request, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = personaje.Id }, personaje);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(PersonajeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PersonajeResponse>> Update(
        int id,
        UpdatePersonajeRequest request,
        CancellationToken cancellationToken)
    {
        var personaje = await service.UpdateAsync(id, request, cancellationToken);
        return personaje is null ? NotFound() : Ok(personaje);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var deleted = await service.DeleteAsync(id, cancellationToken);
        return deleted ? NoContent() : NotFound();
    }
}
