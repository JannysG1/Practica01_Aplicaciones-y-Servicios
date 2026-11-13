using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProveedoresController : ControllerBase
{
    private readonly CafeteriaDbContext _context;

    public ProveedoresController(CafeteriaDbContext context)
    {
        _context = context;
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Proveedor>> PostProveedor([FromBody] Proveedor proveedor)
    {
        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostProveedor), new { id = proveedor.Id }, proveedor);
    }
}
