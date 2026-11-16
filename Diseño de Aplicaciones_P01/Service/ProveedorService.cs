using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.Models;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Service;

public class ProveedorService : IProveedorRepository
{
    private readonly CafeteriaDbContext _context;

    public ProveedorService(CafeteriaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Proveedor>> ObtenerTodosAsync()
    {
        return await _context.Proveedores.AsNoTracking().ToListAsync();
    }

    public async Task<Proveedor?> BuscarPorIdAsync(int id)
    {
        return await _context.Proveedores.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
    {
        var normalizado = nombre.Trim().ToLower();
        return await _context.Proveedores.AnyAsync(p =>
            p.Nombre.ToLower() == normalizado &&
            (!idExcluir.HasValue || p.Id != idExcluir.Value));
    }

    public async Task<Proveedor> AgregarAsync(Proveedor proveedor)
    {
        if (proveedor == null)
        {
            throw new ArgumentNullException(nameof(proveedor));
        }

        if (string.IsNullOrWhiteSpace(proveedor.Nombre) || proveedor.Nombre.Trim().Length < 2)
        {
            throw new ArgumentException("El nombre del proveedor debe tener al menos 2 caracteres.");
        }

        string nombreLimpio = proveedor.Nombre.Trim();
        if (await ExisteNombreAsync(nombreLimpio))
        {
            throw new InvalidOperationException($"Ya existe un proveedor registrado con el nombre '{nombreLimpio}'.");
        }

        proveedor.Nombre = nombreLimpio;
        if (proveedor.Telefono != null)
        {
            proveedor.Telefono = proveedor.Telefono.Trim();
        }

        _context.Proveedores.Add(proveedor);
        await _context.SaveChangesAsync();
        return proveedor;
    }

    public async Task<bool> ActualizarAsync(int id, Proveedor proveedor)
    {
        if (proveedor == null)
        {
            throw new ArgumentNullException(nameof(proveedor));
        }

        var existente = await _context.Proveedores.FindAsync(id);
        if (existente == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(proveedor.Nombre) || proveedor.Nombre.Trim().Length < 2)
        {
            throw new ArgumentException("El nombre del proveedor debe tener al menos 2 caracteres.");
        }

        string nombreLimpio = proveedor.Nombre.Trim();
        if (await ExisteNombreAsync(nombreLimpio, id))
        {
            throw new InvalidOperationException($"Ya existe otro proveedor registrado con el nombre '{nombreLimpio}'.");
        }

        existente.Nombre = nombreLimpio;
        existente.Telefono = proveedor.Telefono?.Trim();
        existente.Activo = proveedor.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarPorIdAsync(int id)
    {
        var existente = await _context.Proveedores.FindAsync(id);
        if (existente == null)
        {
            return false;
        }

        _context.Proveedores.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }
}
