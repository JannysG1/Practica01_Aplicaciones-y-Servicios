using Diseño_de_Aplicaciones_P01.Models;

namespace Diseño_de_Aplicaciones_P01.Repositories;

public interface IProveedorRepository
{
    Task<List<Proveedor>> ObtenerTodosAsync();
    Task<Proveedor?> BuscarPorIdAsync(int id);
    Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
    Task<Proveedor> AgregarAsync(Proveedor proveedor);
    Task<bool> ActualizarAsync(int id, Proveedor proveedor);
    Task<bool> EliminarPorIdAsync(int id);
}
