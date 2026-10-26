using Diseño_de_Aplicaciones_P01.model;

namespace Diseño_de_Aplicaciones_P01.recursos
{
    public class UsuarioRepository : IUsuarioRepository
    {
        public List<Usuario> Usuarios { get; set; }

        public UsuarioRepository()
        {
            Usuarios = new List<Usuario>
            {
                new Usuario { IdUsuario = 1, NombreUsuario = "Ana Torres", CorreoElectronicousuario = "ana@correo.com", Contrasenausuario = "ana123", Activo = true },
                new Usuario { IdUsuario = 2, NombreUsuario = "Marco Aurelio", CorreoElectronicousuario = "marco@correo.com", Contrasenausuario = "marco123", Activo = true },
                new Usuario { IdUsuario = 3, NombreUsuario = "Maria Antonieta", CorreoElectronicousuario = "maria@correo.com", Contrasenausuario = "maria123", Activo = true }
            };
        }

        public ICollection<Usuario> ObtenerUsuarios() => Usuarios;

        public Usuario BuscarPorId(int id) => Usuarios.FirstOrDefault(u => u.IdUsuario == id);

        public void AgregarUsuario(Usuario nuevoUsuario)
        {
            nuevoUsuario.IdUsuario = Usuarios.Any() ? Usuarios.Max(u => u.IdUsuario) + 1 : 1;
            Usuarios.Add(nuevoUsuario);
        }

        public bool EliminarPorId(int id)
        {
            var usuario = BuscarPorId(id);
            if (usuario != null)
            {
                Usuarios.Remove(usuario);
                return true;
            }
            return false;
        }
    }
}