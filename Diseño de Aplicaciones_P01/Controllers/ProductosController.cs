using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Diseño_de_Aplicaciones_P01.model;
using Diseño_de_Aplicaciones_P01.recursos;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[Authorize] // todo el controlador pide token
[ApiController]
[Route("api/[controller]")]
public class ProductosController : ControllerBase
{
    private readonly ProductoRepository _repo;

    public ProductosController(ProductoRepository repo)
    {
        _repo = repo;
    }

    [HttpGet]
    public IActionResult ObtenerTodos()
    {
        return Ok(_repo.ObtenerTodos());
    }

    [HttpGet("{id}")]
    public IActionResult ObtenerPorId(int id)
    {
        var producto = _repo.BuscarPorId(id);
        if (producto == null)
            return NotFound(new { mensaje = "Producto no encontrado" });

        return Ok(producto);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public IActionResult Crear([FromBody] Producto nuevoProducto)
    {
        var creado = _repo.Agregar(nuevoProducto);
        return CreatedAtAction(nameof(ObtenerPorId), new { id = creado.Id }, creado);
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")] // token + rol Admin
    public IActionResult Eliminar(int id)
    {
        var eliminado = _repo.EliminarPorId(id);
        if (!eliminado)
            return NotFound(new { mensaje = "Producto no encontrado" });

        return NoContent();
    }
}
