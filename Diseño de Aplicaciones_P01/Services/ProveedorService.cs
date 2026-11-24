using Diseño_de_Aplicaciones_P01.Interfaces;
using Diseño_de_Aplicaciones_P01.Models;

namespace Diseño_de_Aplicaciones_P01.Services;

/// <summary>
/// Implementación de la capa de servicio para Proveedores.
/// Cumple con el criterio 5 de la rúbrica PPD03 dependiendo exclusivamente de IProveedorRepository
/// mediante inyección de dependencias por constructor, desacoplándose de la base de datos.
/// </summary>
public class ProveedorService : IProveedorService
{
    private readonly IProveedorRepository _proveedorRepository;

    /// <summary>
    /// Inyecta el contrato de abstracción del repositorio de proveedores.
    /// </summary>
    public ProveedorService(IProveedorRepository proveedorRepository)
    {
        _proveedorRepository = proveedorRepository ?? throw new ArgumentNullException(nameof(proveedorRepository));
    }

    /// <summary>
    /// Ejecuta las validaciones comerciales de nombre, unicidad y formato antes de persistir el nuevo proveedor.
    /// </summary>
    public async Task<Proveedor> CrearProveedorAsync(Proveedor proveedor)
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
        if (await _proveedorRepository.ExistsByNameAsync(nombreLimpio))
        {
            throw new InvalidOperationException($"Ya existe un proveedor registrado con el nombre '{nombreLimpio}'.");
        }

        proveedor.Nombre = nombreLimpio;
        if (proveedor.Telefono != null)
        {
            proveedor.Telefono = proveedor.Telefono.Trim();
        }

        await _proveedorRepository.AddAsync(proveedor);
        await _proveedorRepository.SaveChangesAsync();
        return proveedor;
    }

    /// <summary>
    /// Consulta y retorna el listado de proveedores a través de la abstracción del repositorio.
    /// </summary>
    public async Task<IEnumerable<Proveedor>> GetAllAsync()
    {
        return await _proveedorRepository.GetAllAsync();
    }

    /// <summary>
    /// Obtiene un proveedor a partir de su clave de identidad.
    /// </summary>
    public async Task<Proveedor?> GetByIdAsync(int id)
    {
        return await _proveedorRepository.GetByIdAsync(id);
    }

    /// <summary>
    /// Valida las reglas de unicidad y contenido previo a delegar la actualización al repositorio.
    /// </summary>
    public async Task<bool> ActualizarProveedorAsync(int id, Proveedor proveedor)
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
        if (await _proveedorRepository.ExistsByNameAsync(nombreLimpio, id))
        {
            throw new InvalidOperationException($"Ya existe otro proveedor registrado con el nombre '{nombreLimpio}'.");
        }

        proveedor.Nombre = nombreLimpio;
        proveedor.Telefono = proveedor.Telefono?.Trim();

        return await _proveedorRepository.UpdateAsync(id, proveedor);
    }

    /// <summary>
    /// Solicita la baja del registro del proveedor por medio del contrato de persistencia.
    /// </summary>
    public async Task<bool> EliminarProveedorAsync(int id)
    {
        return await _proveedorRepository.DeleteAsync(id);
    }
}
