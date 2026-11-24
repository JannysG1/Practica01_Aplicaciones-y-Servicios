using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;

namespace Diseño_de_Aplicaciones_P01.Repositories;

public interface IUsuarioRepository
{
    Task<List<Usuario>> ObtenerTodosAsync();
    Task<Usuario?> BuscarPorIdAsync(int id);
    Task<Usuario?> BuscarPorCorreoAsync(string correo);
    Task<bool> ExisteCorreoAsync(string correo, int? idExcluir = null);
    Task<Usuario> AgregarAsync(Usuario nuevoUsuario);
    Task<Usuario> CrearAsync(UsuarioCreateDTO dto);
    Task<bool> ActualizarAsync(Usuario usuario);
    Task<bool> ActualizarAsync(int id, UsuarioUpdateDTO dto);
    Task<bool> EliminarPorIdAsync(int id);
}
