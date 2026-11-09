using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;
using Diseño_de_Aplicaciones_P01.recursos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioRepository _repositorio;

    public UsuariosController(IUsuarioRepository repositorio)
    {
        _repositorio = repositorio;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Usuario")]
    public ActionResult<ICollection<Usuario>> ObtenerUsuarios()
    {
        var usuarios = _repositorio.ObtenerUsuarios();
        return Ok(usuarios);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Usuario")]
    public ActionResult<Usuario> BuscarPorId(int id)
    {
        var usuario = _repositorio.BuscarPorId(id);

        if (usuario == null)
        {
            return NotFound(new { mensaje = $"Usuario con el ID {id} no encontrado." });
        }
        return Ok(usuario);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public ActionResult AgregarUsuario([FromBody] UsuarioCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (_repositorio.ExisteCorreo(dto.CorreoElectronicousuario))
        {
            return Conflict(new { mensaje = $"Ya existe un usuario registrado con el correo '{dto.CorreoElectronicousuario}'." });
        }

        var nuevoUsuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario.Trim(),
            CorreoElectronicousuario = dto.CorreoElectronicousuario.Trim().ToLower(),
            Contrasenausuario = dto.Contrasenausuario,
            Activo = dto.Activo
        };

        _repositorio.AgregarUsuario(nuevoUsuario);

        return CreatedAtAction(nameof(BuscarPorId), new { id = nuevoUsuario.IdUsuario }, nuevoUsuario);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public ActionResult ActualizarUsuario(int id, [FromBody] UsuarioUpdateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var existente = _repositorio.BuscarPorId(id);
        if (existente == null)
        {
            return NotFound(new { mensaje = $"No se puede actualizar: Usuario con ID {id} no encontrado." });
        }

        if (_repositorio.ExisteCorreo(dto.CorreoElectronicousuario, id))
        {
            return Conflict(new { mensaje = $"El correo '{dto.CorreoElectronicousuario}' ya está en uso por otro usuario." });
        }

        var usuarioActualizar = new Usuario
        {
            IdUsuario = id,
            NombreUsuario = dto.NombreUsuario.Trim(),
            CorreoElectronicousuario = dto.CorreoElectronicousuario.Trim().ToLower(),
            Contrasenausuario = string.IsNullOrWhiteSpace(dto.Contrasenausuario) ? existente.Contrasenausuario : dto.Contrasenausuario,
            Activo = dto.Activo
        };

        bool actualizado = _repositorio.ActualizarUsuario(usuarioActualizar);
        if (!actualizado)
        {
            return StatusCode(500, new { mensaje = "Ocurrió un error al actualizar el usuario." });
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public ActionResult EliminarPorId(int id)
    {
        bool eliminado = _repositorio.EliminarPorId(id);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"No se puede eliminar: Usuario con ID {id} no encontrado." });
        }
        return NoContent();
    }
}
