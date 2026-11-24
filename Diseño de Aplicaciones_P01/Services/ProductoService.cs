using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Service;

public class ProductoService : IProductoRepository
{
    private readonly CafeteriaDbContext _context;

    public ProductoService(CafeteriaDbContext context)
    {
        _context = context;
    }

    public async Task<List<Producto>> ObtenerTodosAsync()
    {
        return await _context.Productos.AsNoTracking().ToListAsync();
    }

    public async Task<Producto?> BuscarPorIdAsync(int id)
    {
        return await _context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Producto?> BuscarPorNombreAsync(string nombre)
    {
        var normalizado = nombre.Trim().ToLower();
        return await _context.Productos
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Nombre.ToLower() == normalizado);
    }

    public async Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null)
    {
        var normalizado = nombre.Trim().ToLower();
        return await _context.Productos.AnyAsync(p =>
            p.Nombre.ToLower() == normalizado &&
            (!idExcluir.HasValue || p.Id != idExcluir.Value));
    }

    public async Task<Producto> AgregarAsync(Producto producto)
    {
        if (producto == null)
        {
            throw new ArgumentNullException(nameof(producto));
        }

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        }

        if (producto.Precio <= 0)
        {
            throw new ArgumentException("El precio del producto debe ser mayor a 0.");
        }

        producto.Nombre = producto.Nombre.Trim();
        if (await ExisteNombreAsync(producto.Nombre))
        {
            throw new InvalidOperationException($"Ya existe un producto con el nombre '{producto.Nombre}'.");
        }

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task<Producto> CrearAsync(ProductoCreateDTO dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        }

        if (dto.Precio <= 0)
        {
            throw new ArgumentException("El precio del producto debe ser mayor a 0.");
        }

        string nombreLimpio = dto.Nombre.Trim();
        if (await ExisteNombreAsync(nombreLimpio))
        {
            throw new InvalidOperationException($"Ya existe un producto registrado con el nombre '{nombreLimpio}'.");
        }

        var producto = new Producto
        {
            Nombre = nombreLimpio,
            Precio = dto.Precio,
            Activo = dto.Activo
        };

        _context.Productos.Add(producto);
        await _context.SaveChangesAsync();
        return producto;
    }

    public async Task<bool> ActualizarAsync(Producto producto)
    {
        if (producto == null)
        {
            throw new ArgumentNullException(nameof(producto));
        }

        if (string.IsNullOrWhiteSpace(producto.Nombre))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        }

        if (producto.Precio <= 0)
        {
            throw new ArgumentException("El precio del producto debe ser mayor a 0.");
        }

        var existente = await _context.Productos.FindAsync(producto.Id);
        if (existente == null)
        {
            return false;
        }

        string nombreLimpio = producto.Nombre.Trim();
        if (await ExisteNombreAsync(nombreLimpio, producto.Id))
        {
            throw new InvalidOperationException($"Ya existe otro producto con el nombre '{nombreLimpio}'.");
        }

        existente.Nombre = nombreLimpio;
        existente.Precio = producto.Precio;
        existente.Activo = producto.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActualizarAsync(int id, ProductoUpdateDTO dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.Nombre))
        {
            throw new ArgumentException("El nombre del producto no puede estar vacío.");
        }

        if (dto.Precio <= 0)
        {
            throw new ArgumentException("El precio del producto debe ser mayor a 0.");
        }

        var existente = await _context.Productos.FindAsync(id);
        if (existente == null)
        {
            return false;
        }

        string nombreLimpio = dto.Nombre.Trim();
        if (await ExisteNombreAsync(nombreLimpio, id))
        {
            throw new InvalidOperationException($"Ya existe otro producto con el nombre '{nombreLimpio}'.");
        }

        existente.Nombre = nombreLimpio;
        existente.Precio = dto.Precio;
        existente.Activo = dto.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarPorIdAsync(int id)
    {
        var existente = await _context.Productos.FindAsync(id);
        if (existente == null)
        {
            return false;
        }

        _context.Productos.Remove(existente);
        await _context.SaveChangesAsync();
        return true;
    }
}
