using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.model;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.recursos
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly ApplicationDbContext _context;

        public UsuarioRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public ICollection<Usuario> ObtenerUsuarios() => _context.Usuarios.AsNoTracking().ToList();

        public Usuario? BuscarPorId(int id) => _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.IdUsuario == id);

        public void AgregarUsuario(Usuario nuevoUsuario)
        {
            _context.Usuarios.Add(nuevoUsuario);
            _context.SaveChanges();
        }

        public bool EliminarPorId(int id)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}