using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class ProductosController : ControllerBase
{
    private readonly IProductoRepository _repo;

    public ProductosController(IProductoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public async Task<ActionResult<List<Producto>>> ObtenerTodos()
    {
        var productos = await _repo.ObtenerTodosAsync();
        return Ok(productos);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Producto>> ObtenerPorId(int id)
    {
        var producto = await _repo.BuscarPorIdAsync(id);
        if (producto == null)
        {
            return NotFound(new { mensaje = "Producto no encontrado" });
        }
        return Ok(producto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Agregar([FromBody] ProductoCreateDTO dto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var producto = await _repo.CrearAsync(dto);
            return CreatedAtAction(nameof(ObtenerPorId), new { id = producto.Id }, producto);
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
    public async Task<ActionResult> Actualizar(int id, [FromBody] ProductoUpdateDTO dto)
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
                return NotFound(new { mensaje = "Producto no encontrado para actualizar" });
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
    public async Task<ActionResult> Eliminar(int id)
    {
        bool eliminado = await _repo.EliminarPorIdAsync(id);
        if (!eliminado)
        {
            return NotFound(new { mensaje = "Producto no encontrado para eliminar" });
        }

        return NoContent();
    }
}
