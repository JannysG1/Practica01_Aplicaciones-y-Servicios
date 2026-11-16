using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[Authorize(Roles = "Admin")]
[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRolService _rolService;

    public RolesController(IRolService rolService)
    {
        _rolService = rolService;
    }

    [HttpGet]
    public async Task<ActionResult<List<string>>> ObtenerRoles()
    {
        var roles = await _rolService.ObtenerRolesAsync();
        return Ok(roles);
    }

    [HttpPost]
    public async Task<IActionResult> CrearRol([FromBody] CreateRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _rolService.CrearRolAsync(request.NombreRol);
        if (!resultado.Exitoso)
        {
            return Conflict(new { mensaje = resultado.Mensaje, errores = resultado.Errores });
        }

        return CreatedAtAction(nameof(ObtenerRoles), new { rol = request.NombreRol.Trim() }, new { mensaje = resultado.Mensaje });
    }

    [HttpPost("asignar")]
    public async Task<IActionResult> AsignarRol([FromBody] AssignRoleRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = await _rolService.AsignarRolAUsuarioAsync(request.Usuario, request.Rol);
        if (!resultado.Exitoso)
        {
            return BadRequest(new { mensaje = resultado.Mensaje, errores = resultado.Errores });
        }

        return Ok(new { mensaje = resultado.Mensaje });
    }

    [HttpGet("usuario/{usuario}")]
    public async Task<ActionResult<IList<string>>> ObtenerRolesUsuario(string usuario)
    {
        var roles = await _rolService.ObtenerRolesDeUsuarioAsync(usuario);
        return Ok(roles);
    }
}
