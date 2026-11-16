using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repo;

    public UsuariosController(IUsuarioRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Usuario")]
    public async Task<ActionResult<List<Usuario>>> ObtenerUsuarios()
    {
        var usuarios = await _repo.ObtenerTodosAsync();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Usuario")]
    public async Task<ActionResult<Usuario>> BuscarPorId(int id)
    {
        var usuario = await _repo.BuscarPorIdAsync(id);

        if (usuario == null)
        {
            return NotFound(new { mensaje = $"Usuario con el ID {id} no encontrado." });
        }
        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> AgregarUsuario([FromBody] UsuarioCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var nuevoUsuario = await _repo.CrearAsync(dto);
            return CreatedAtAction(nameof(BuscarPorId), new { id = nuevoUsuario.IdUsuario }, nuevoUsuario);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ActualizarUsuario(int id, [FromBody] UsuarioUpdateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            bool actualizado = await _repo.ActualizarAsync(id, dto);
            if (!actualizado)
            {
                return NotFound(new { mensaje = $"No se puede actualizar: Usuario con ID {id} no encontrado." });
            }

            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { mensaje = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return Conflict(new { mensaje = ex.Message });
        }
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> EliminarPorId(int id)
    {
        bool eliminado = await _repo.EliminarPorIdAsync(id);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"No se puede eliminar: Usuario con ID {id} no encontrado." });
        }
        return NoContent();
    }
}
