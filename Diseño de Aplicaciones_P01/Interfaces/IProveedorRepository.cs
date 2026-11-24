using Diseño_de_Aplicaciones_P01.Models;

namespace Diseño_de_Aplicaciones_P01.Interfaces;

/// <summary>
/// Contrato de repositorio para el acceso a datos y persistencia directa de Proveedores en base de datos.
/// Cumple con los requerimientos de la Guía ITID803_02 (PPD03): GetAllAsync, GetByIdAsync, AddAsync y SaveChangesAsync.
/// </summary>
public interface IProveedorRepository
{
    /// <summary>
    /// Consulta y recupera todos los proveedores registrados en la base de datos.
    /// </summary>
    Task<IEnumerable<Proveedor>> GetAllAsync();

    /// <summary>
    /// Busca un proveedor específico por su identificador primario.
    /// </summary>
    Task<Proveedor?> GetByIdAsync(int id);

    /// <summary>
    /// Registra la entidad de un proveedor en el contexto de persistencia.
    /// </summary>
    Task<Proveedor> AddAsync(Proveedor proveedor);

    /// <summary>
    /// Confirma y persiste los cambios pendientes en el motor de base de datos.
    /// </summary>
    Task SaveChangesAsync();

    /// <summary>
    /// Verifica la existencia de un nombre de proveedor para garantizar reglas de unicidad.
    /// </summary>
    Task<bool> ExistsByNameAsync(string nombre, int? idExcluir = null);

    /// <summary>
    /// Modifica los atributos persistidos de un proveedor existente.
    /// </summary>
    Task<bool> UpdateAsync(int id, Proveedor proveedor);

    /// <summary>
    /// Remueve un registro de proveedor de la persistencia relacional.
    /// </summary>
    Task<bool> DeleteAsync(int id);
}
