using Diseño_de_Aplicaciones_P01.Data;
using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.model;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Service;

public class UsuarioService : IUsuarioRepository
{
    private readonly ApplicationDbContext _context;

    public UsuarioService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Usuario>> ObtenerTodosAsync()
    {
        return await _context.Usuarios.AsNoTracking().ToListAsync();
    }

    public async Task<Usuario?> BuscarPorIdAsync(int id)
    {
        return await _context.Usuarios.AsNoTracking().FirstOrDefaultAsync(u => u.IdUsuario == id);
    }

    public async Task<Usuario?> BuscarPorCorreoAsync(string correo)
    {
        var normalizado = correo.Trim().ToLower();
        return await _context.Usuarios
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CorreoElectronicousuario.ToLower() == normalizado);
    }

    public async Task<bool> ExisteCorreoAsync(string correo, int? idExcluir = null)
    {
        var normalizado = correo.Trim().ToLower();
        return await _context.Usuarios.AnyAsync(u =>
            u.CorreoElectronicousuario.ToLower() == normalizado &&
            (!idExcluir.HasValue || u.IdUsuario != idExcluir.Value));
    }

    public async Task<Usuario> AgregarAsync(Usuario nuevoUsuario)
    {
        if (nuevoUsuario == null)
        {
            throw new ArgumentNullException(nameof(nuevoUsuario));
        }

        string correoNormalizado = nuevoUsuario.CorreoElectronicousuario.Trim().ToLower();
        if (await ExisteCorreoAsync(correoNormalizado))
        {
            throw new InvalidOperationException($"Ya existe un usuario registrado con el correo '{nuevoUsuario.CorreoElectronicousuario}'.");
        }

        nuevoUsuario.NombreUsuario = nuevoUsuario.NombreUsuario.Trim();
        nuevoUsuario.CorreoElectronicousuario = correoNormalizado;

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();
        return nuevoUsuario;
    }

    public async Task<Usuario> CrearAsync(UsuarioCreateDTO dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        if (string.IsNullOrWhiteSpace(dto.NombreUsuario) || dto.NombreUsuario.Trim().Length < 2)
        {
            throw new ArgumentException("El nombre de usuario debe tener al menos 2 caracteres.");
        }

        if (string.IsNullOrWhiteSpace(dto.CorreoElectronicousuario))
        {
            throw new ArgumentException("El correo electrónico es obligatorio.");
        }

        string correoNormalizado = dto.CorreoElectronicousuario.Trim().ToLower();
        if (await ExisteCorreoAsync(correoNormalizado))
        {
            throw new InvalidOperationException($"Ya existe un usuario registrado con el correo '{dto.CorreoElectronicousuario}'.");
        }

        if (string.IsNullOrWhiteSpace(dto.Contrasenausuario) || dto.Contrasenausuario.Length < 4)
        {
            throw new ArgumentException("La contraseña debe tener al menos 4 caracteres.");
        }

        var nuevoUsuario = new Usuario
        {
            NombreUsuario = dto.NombreUsuario.Trim(),
            CorreoElectronicousuario = correoNormalizado,
            Contrasenausuario = dto.Contrasenausuario,
            Activo = dto.Activo
        };

        _context.Usuarios.Add(nuevoUsuario);
        await _context.SaveChangesAsync();
        return nuevoUsuario;
    }

    public async Task<bool> ActualizarAsync(Usuario usuario)
    {
        if (usuario == null)
        {
            throw new ArgumentNullException(nameof(usuario));
        }

        var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == usuario.IdUsuario);
        if (existente == null)
        {
            return false;
        }

        string correoNormalizado = usuario.CorreoElectronicousuario.Trim().ToLower();
        if (await ExisteCorreoAsync(correoNormalizado, usuario.IdUsuario))
        {
            throw new InvalidOperationException($"El correo '{usuario.CorreoElectronicousuario}' ya está en uso por otro usuario.");
        }

        existente.NombreUsuario = usuario.NombreUsuario.Trim();
        existente.CorreoElectronicousuario = correoNormalizado;
        if (!string.IsNullOrWhiteSpace(usuario.Contrasenausuario))
        {
            existente.Contrasenausuario = usuario.Contrasenausuario;
        }
        existente.Activo = usuario.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ActualizarAsync(int id, UsuarioUpdateDTO dto)
    {
        if (dto == null)
        {
            throw new ArgumentNullException(nameof(dto));
        }

        var existente = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
        if (existente == null)
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(dto.NombreUsuario) || dto.NombreUsuario.Trim().Length < 2)
        {
            throw new ArgumentException("El nombre de usuario debe tener al menos 2 caracteres.");
        }

        string correoNormalizado = dto.CorreoElectronicousuario.Trim().ToLower();
        if (await ExisteCorreoAsync(correoNormalizado, id))
        {
            throw new InvalidOperationException($"El correo '{dto.CorreoElectronicousuario}' ya está en uso por otro usuario.");
        }

        existente.NombreUsuario = dto.NombreUsuario.Trim();
        existente.CorreoElectronicousuario = correoNormalizado;
        if (!string.IsNullOrWhiteSpace(dto.Contrasenausuario))
        {
            existente.Contrasenausuario = dto.Contrasenausuario;
        }
        existente.Activo = dto.Activo;

        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> EliminarPorIdAsync(int id)
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.IdUsuario == id);
        if (usuario == null)
        {
            return false;
        }

        _context.Usuarios.Remove(usuario);
        await _context.SaveChangesAsync();
        return true;
    }
}
