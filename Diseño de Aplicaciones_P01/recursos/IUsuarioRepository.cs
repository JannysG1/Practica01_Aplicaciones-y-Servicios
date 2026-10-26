using Diseño_de_Aplicaciones_P01.model;

namespace Diseño_de_Aplicaciones_P01.recursos
{
    public interface IUsuarioRepository
    {
        ICollection<Usuario> ObtenerUsuarios();
        Usuario BuscarPorId(int id);
        void AgregarUsuario(Usuario nuevoUsuario);
        bool EliminarPorId(int id);
    }
}