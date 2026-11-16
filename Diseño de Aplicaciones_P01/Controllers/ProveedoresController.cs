using Diseño_de_Aplicaciones_P01.Models;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorRepository _repo;

    public ProveedoresController(IProveedorRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Usuario")]
    public async Task<ActionResult<List<Proveedor>>> ObtenerProveedores()
    {
        var proveedores = await _repo.ObtenerTodosAsync();
        return Ok(proveedores);
    }

    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Usuario")]
    public async Task<ActionResult<Proveedor>> BuscarPorId(int id)
    {
        var proveedor = await _repo.BuscarPorIdAsync(id);
        if (proveedor == null)
        {
            return NotFound(new { mensaje = $"Proveedor con el ID {id} no encontrado." });
        }
        return Ok(proveedor);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Proveedor>> PostProveedor([FromBody] Proveedor proveedor)
    {
        try
        {
            var nuevoProveedor = await _repo.AgregarAsync(proveedor);
            return CreatedAtAction(nameof(PostProveedor), new { id = nuevoProveedor.Id }, nuevoProveedor);
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
    public async Task<ActionResult> ActualizarProveedor(int id, [FromBody] Proveedor proveedor)
    {
        try
        {
            bool actualizado = await _repo.ActualizarAsync(id, proveedor);
            if (!actualizado)
            {
                return NotFound(new { mensaje = $"Proveedor con el ID {id} no encontrado." });
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
    public async Task<ActionResult> EliminarProveedor(int id)
    {
        bool eliminado = await _repo.EliminarPorIdAsync(id);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"Proveedor con el ID {id} no encontrado." });
        }
        return NoContent();
    }
}
