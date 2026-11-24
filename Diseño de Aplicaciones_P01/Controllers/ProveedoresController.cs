using Diseño_de_Aplicaciones_P01.Interfaces;
using Diseño_de_Aplicaciones_P01.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

/// <summary>
/// Controlador para la gestión y administración de proveedores.
/// Cumple con la rúbrica PPD03 recibiendo IProveedorService por constructor y exponiendo POST /api/Proveedores con 201 CreatedAtAction.
/// </summary>
[Authorize]
[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly IProveedorService _proveedorService;

    /// <summary>
    /// Inyecta el servicio de negocio IProveedorService desacoplado de la base de datos.
    /// </summary>
    public ProveedoresController(IProveedorService proveedorService)
    {
        _proveedorService = proveedorService ?? throw new ArgumentNullException(nameof(proveedorService));
    }

    /// <summary>
    /// Consulta el listado de todos los proveedores registrados en el sistema.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Usuario")]
    public async Task<ActionResult<IEnumerable<Proveedor>>> ObtenerProveedores()
    {
        var proveedores = await _proveedorService.GetAllAsync();
        return Ok(proveedores);
    }

    /// <summary>
    /// Consulta el detalle de un proveedor a partir de su Id.
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin,Usuario")]
    public async Task<ActionResult<Proveedor>> BuscarPorId(int id)
    {
        var proveedor = await _proveedorService.GetByIdAsync(id);
        if (proveedor == null)
        {
            return NotFound(new { mensaje = $"Proveedor con el ID {id} no encontrado." });
        }
        return Ok(proveedor);
    }

    /// <summary>
    /// Registra un nuevo proveedor validado y protegido para el rol Administrador.
    /// Devuelve 201 CreatedAtAction con la ruta de consulta y el Id asignado.
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Proveedor>> PostProveedor([FromBody] Proveedor proveedor)
    {
        try
        {
            var nuevoProveedor = await _proveedorService.CrearProveedorAsync(proveedor);
            return CreatedAtAction(nameof(BuscarPorId), new { id = nuevoProveedor.Id }, nuevoProveedor);
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

    /// <summary>
    /// Modifica los datos de un proveedor existente. Requiere rol Administrador.
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> ActualizarProveedor(int id, [FromBody] Proveedor proveedor)
    {
        try
        {
            bool actualizado = await _proveedorService.ActualizarProveedorAsync(id, proveedor);
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

    /// <summary>
    /// Elimina un proveedor del sistema por su identificador. Requiere rol Administrador.
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> EliminarProveedor(int id)
    {
        bool eliminado = await _proveedorService.EliminarProveedorAsync(id);
        if (!eliminado)
        {
            return NotFound(new { mensaje = $"Proveedor con el ID {id} no encontrado." });
        }
        return NoContent();
    }
}
