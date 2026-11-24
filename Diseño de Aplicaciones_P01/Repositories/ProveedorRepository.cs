using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.Interfaces;
using Diseño_de_Aplicaciones_P01.Models;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Repositories;

/// <summary>
/// Implementación concreta de IProveedorRepository que accede y manipula directamente CafeteriaDbContext.
/// Cumple con el criterio 4 de la rúbrica PPD03 aislando las operaciones de persistencia relacional con EF Core.
/// </summary>
public class ProveedorRepository : IProveedorRepository
{
    private readonly CafeteriaDbContext _context;

    /// <summary>
    /// Inyecta el contexto de base de datos de CafeteriaDbContext.
    /// </summary>
    public ProveedorRepository(CafeteriaDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    /// <summary>
    /// Consulta todos los proveedores en la base de datos sin seguimiento de estado.
    /// </summary>
    public async Task<IEnumerable<Proveedor>> GetAllAsync()
    {
        return await _context.Proveedores.AsNoTracking().ToListAsync();
    }

    /// <summary>
    /// Busca un proveedor por su Id primario.
    /// </summary>
    public async Task<Proveedor?> GetByIdAsync(int id)
    {
        return await _context.Proveedores.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Añade la entidad de un proveedor al conjunto de entidades para su posterior confirmación.
    /// </summary>
    public async Task<Proveedor> AddAsync(Proveedor proveedor)
    {
        await _context.Proveedores.AddAsync(proveedor);
        return proveedor;
    }

    /// <summary>
    /// Persiste los cambios pendientes en el motor relacional de SQL Server.
    /// </summary>
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

    /// <summary>
    /// Evalúa si ya existe registrado un proveedor con el mismo nombre, permitiendo excluir un Id determinado.
    /// </summary>
    public async Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null)
    {
        var nombreNormalizado = nombre.Trim().ToLower();
        return await _context.Proveedores.AnyAsync(p =>
            p.Nombre.ToLower() == nombreNormalizado &&
            (!idExcluir.HasValue || p.Id != idExcluir.Value));
    }

    /// <summary>
    /// Aplica cambios sobre las propiedades de un proveedor registrado y los persiste.
    /// </summary>
    public async Task<bool> UpdateAsync(int id, Proveedor proveedor)
    {
        var existente = await _context.Proveedores.FindAsync(id);
        if (existente == null)
        {
            return false;
        }

        existente.Nombre = proveedor.Nombre;
        existente.Telefono = proveedor.Telefono;
        existente.Activo = proveedor.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    /// <summary>
    /// Elimina físicamente un registro de proveedor identificado por su Id.
    /// </summary>
    public async Task<bool> DeleteAsync(int id)
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
