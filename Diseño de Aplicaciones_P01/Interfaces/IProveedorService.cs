using Diseño_de_Aplicaciones_P01.Models;

namespace Diseño_de_Aplicaciones_P01.Interfaces;

/// <summary>
/// Contrato de la capa de servicio para la gestión de reglas de negocio y validación de Proveedores.
/// Cumple con los requerimientos de la Guía ITID803_02 (PPD03): CrearProveedorAsync y operaciones de consulta.
/// </summary>
public interface IProveedorService
{
    /// <summary>
    /// Aplica validaciones de negocio, unicidad de nombre y crea un nuevo proveedor.
    /// </summary>
    Task<Proveedor> CrearProveedorAsync(Proveedor proveedor);

    /// <summary>
    /// Obtiene la colección completa de proveedores disponibles.
    /// </summary>
    Task<IEnumerable<Proveedor>> GetAllAsync();

    /// <summary>
    /// Obtiene un proveedor a partir de su identificador primario.
    /// </summary>
    Task<Proveedor?> GetByIdAsync(int id);

    /// <summary>
    /// Valida y actualiza los datos comerciales de un proveedor registrado.
    /// </summary>
    Task<bool> ActualizarProveedorAsync(int id, Proveedor proveedor);

    /// <summary>
    /// Gestiona la baja o eliminación controlada de un proveedor.
    /// </summary>
    Task<bool> EliminarProveedorAsync(int id);
}
