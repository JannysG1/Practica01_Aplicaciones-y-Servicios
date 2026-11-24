/*
# **********************************************************
# Deber/Ejercicio: PPD03 – Arquitectura en Capas, Interfaces y Persistencia
# Tu Nombre va aquí: Jannys Garrido
# Fecha de realización: 24/11/2026
# Fecha de entrega: 24/11/2026
# Resultados:
#  * El código implementa una arquitectura en capas desacoplada con interfaces:
#    - Carpeta Interfaces/ para los contratos IProveedorRepository e IProveedorService.
#    - Carpeta Repositories/ para ProveedorRepository usando CafeteriaDbContext directamente.
#    - Carpeta Services/ para ProveedorService dependiendo de IProveedorRepository por constructor.
#    - ProveedoresController refactorizado inyectando IProveedorService y retornando 201 CreatedAtAction.
#    - Registro de dependencias en Program.cs y persistencia demostrada en SQL Server (dbo.Proveedores).
# Conclusiones:
#  * La separación en capas con interfaces desacopla totalmente la persistencia relacional de la lógica de negocio y transporte HTTP.
#  * La inyección por constructor de IProveedorRepository en ProveedorService facilita el mantenimiento y la realización de pruebas unitarias con dobles de prueba (mocks).
#  * El cumplimiento de los principios SOLID (SRP, ISP, DIP) garantiza escalabilidad, modularidad y robustez en la API REST.
# Recomendaciones:
#  * Mantener la inyección de dependencias a través de contratos e interfaces para evitar acoplamiento directo a implementaciones concretas.
#  * Aplicar validaciones de negocio en la capa de servicios antes de interactuar con la base de datos para prevenir inconsistencias.
#  * Documentar el propósito y diseño arquitectónico en función del qué hace y no de la mecánica sintáctica del lenguaje.
# Bibliografía:
#  * Martin, R. C. (2008). Clean Code: A Handbook of Agile Software Craftsmanship. Prentice Hall.
#  * Microsoft Learn. Arquitectura de capas e inyección de dependencias en ASP.NET Core y Entity Framework Core.
#  * Evans, E. (2003). Domain-Driven Design: Tackling Complexity in the Heart of Software. Addison-Wesley.
# **********************************************************
*/

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
