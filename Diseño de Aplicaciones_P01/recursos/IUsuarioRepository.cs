using Diseño_de_Aplicaciones_P01.model;

namespace Diseño_de_Aplicaciones_P01.recursos;

public interface IUsuarioRepository
{
    ICollection<Usuario> ObtenerUsuarios();
    Usuario? BuscarPorId(int id);
    Usuario? BuscarPorCorreo(string correo);
    bool ExisteCorreo(string correo, int? idExcluir = null);
    void AgregarUsuario(Usuario nuevoUsuario);
    bool ActualizarUsuario(Usuario usuario);
    bool EliminarPorId(int id);
}
