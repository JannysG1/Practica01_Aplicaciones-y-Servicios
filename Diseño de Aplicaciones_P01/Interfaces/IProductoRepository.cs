using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;

namespace Diseño_de_Aplicaciones_P01.Repositories;

public interface IProductoRepository
{
    Task<List<Producto>> ObtenerTodosAsync();
    Task<Producto?> BuscarPorIdAsync(int id);
    Task<Producto?> BuscarPorNombreAsync(string nombre);
    Task<bool> ExisteNombreAsync(string nombre, int? idExcluir = null);
    Task<Producto> AgregarAsync(Producto producto);
    Task<Producto> CrearAsync(ProductoCreateDTO dto);
    Task<bool> ActualizarAsync(Producto producto);
    Task<bool> ActualizarAsync(int id, ProductoUpdateDTO dto);
    Task<bool> EliminarPorIdAsync(int id);
}
