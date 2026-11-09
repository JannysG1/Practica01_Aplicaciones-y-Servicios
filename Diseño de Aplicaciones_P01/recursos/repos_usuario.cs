using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.model;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.recursos;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public ICollection<Usuario> ObtenerUsuarios() =>
        _context.Usuarios.AsNoTracking().ToList();

    public Usuario? BuscarPorId(int id) =>
        _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.IdUsuario == id);

    public Usuario? BuscarPorCorreo(string correo) =>
        _context.Usuarios.AsNoTracking().FirstOrDefault(u => u.CorreoElectronicousuario.ToLower() == correo.Trim().ToLower());

    public bool ExisteCorreo(string correo, int? idExcluir = null)
    {
        var normalizado = correo.Trim().ToLower();
        return _context.Usuarios.Any(u =>
            u.CorreoElectronicousuario.ToLower() == normalizado &&
            (!idExcluir.HasValue || u.IdUsuario != idExcluir.Value));
    }

    public void AgregarUsuario(Usuario nuevoUsuario)
    {
        _context.Usuarios.Add(nuevoUsuario);
        _context.SaveChanges();
    }

    public bool ActualizarUsuario(Usuario usuario)
    {
        var existente = _context.Usuarios.FirstOrDefault(u => u.IdUsuario == usuario.IdUsuario);
        if (existente == null)
        {
            return false;
        }

        existente.NombreUsuario = usuario.NombreUsuario;
        existente.CorreoElectronicousuario = usuario.CorreoElectronicousuario;
        if (!string.IsNullOrWhiteSpace(usuario.Contrasenausuario))
        {
            existente.Contrasenausuario = usuario.Contrasenausuario;
        }
        existente.Activo = usuario.Activo;

        _context.SaveChanges();
        return true;
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
