using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.Models;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Diseño_de_Aplicaciones_P01.Service;

public class RolService : IRolService
{
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    // Roles protegidos del sistema que no pueden ser eliminados
    private static readonly HashSet<string> RolesBaseSistema = new(StringComparer.OrdinalIgnoreCase)
    {
        "SuperAdmin",
        "Admin",
        "Usuario"
    };

    public RolService(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
    {
        _roleManager = roleManager;
        _userManager = userManager;
    }

    public async Task<List<string>> ObtenerRolesAsync()
    {
        return await _roleManager.Roles
            .AsNoTracking()
            .Select(r => r.Name!)
            .Where(r => r != null)
            .ToListAsync();
    }

    public async Task<RoleResult> CrearRolAsync(string nombreRol)
    {
        if (string.IsNullOrWhiteSpace(nombreRol))
        {
            return RoleResult.Fallido("El nombre del rol no puede estar vacío.");
        }

        string rolNormalizado = nombreRol.Trim();

        bool existe = await _roleManager.RoleExistsAsync(rolNormalizado);
        if (existe)
        {
            return RoleResult.Fallido($"El rol '{rolNormalizado}' ya existe en el sistema.");
        }

        var resultado = await _roleManager.CreateAsync(new IdentityRole(rolNormalizado));
        if (!resultado.Succeeded)
        {
            return RoleResult.Fallido("No se pudo crear el rol.", resultado.Errors.Select(e => e.Description));
        }

        return RoleResult.Ok($"Rol '{rolNormalizado}' creado exitosamente de forma dinámica.");
    }

    public async Task<RoleResult> AsignarRolAUsuarioAsync(string usuarioOEmail, string nombreRol)
    {
        if (string.IsNullOrWhiteSpace(usuarioOEmail) || string.IsNullOrWhiteSpace(nombreRol))
        {
            return RoleResult.Fallido("El usuario y el rol son obligatorios.");
        }

        string rolNormalizado = nombreRol.Trim();

        bool existeRol = await _roleManager.RoleExistsAsync(rolNormalizado);
        if (!existeRol)
        {
            return RoleResult.Fallido($"El rol '{rolNormalizado}' no existe. Primero debe crearse en el sistema.");
        }

        var user = await _userManager.FindByNameAsync(usuarioOEmail.Trim())
            ?? await _userManager.FindByEmailAsync(usuarioOEmail.Trim());

        if (user == null)
        {
            return RoleResult.Fallido($"No se encontró al usuario '{usuarioOEmail}'.");
        }

        if (await _userManager.IsInRoleAsync(user, rolNormalizado))
        {
            return RoleResult.Fallido($"El usuario '{user.UserName}' ya posee el rol '{rolNormalizado}'.");
        }

        var resultado = await _userManager.AddToRoleAsync(user, rolNormalizado);
        if (!resultado.Succeeded)
        {
            return RoleResult.Fallido("No se pudo asignar el rol al usuario.", resultado.Errors.Select(e => e.Description));
        }

        return RoleResult.Ok($"Rol '{rolNormalizado}' asignado exitosamente al usuario '{user.UserName}'.");
    }

    public async Task<IList<string>> ObtenerRolesDeUsuarioAsync(string usuarioOEmail)
    {
        if (string.IsNullOrWhiteSpace(usuarioOEmail))
        {
            return new List<string>();
        }

        var user = await _userManager.FindByNameAsync(usuarioOEmail.Trim())
            ?? await _userManager.FindByEmailAsync(usuarioOEmail.Trim());

        if (user == null)
        {
            return new List<string>();
        }

        return await _userManager.GetRolesAsync(user);
    }

    public async Task<RoleResult> EliminarRolAsync(string nombreRol)
    {
        if (string.IsNullOrWhiteSpace(nombreRol))
        {
            return RoleResult.Fallido("El nombre del rol no puede estar vacío.");
        }

        string rolNormalizado = nombreRol.Trim();

        if (RolesBaseSistema.Contains(rolNormalizado))
        {
            return RoleResult.Fallido($"No se permite eliminar el rol base del sistema '{rolNormalizado}'.");
        }

        var rol = await _roleManager.FindByNameAsync(rolNormalizado);
        if (rol == null)
        {
            return RoleResult.Fallido($"El rol '{rolNormalizado}' no existe.");
        }

        var resultado = await _roleManager.DeleteAsync(rol);
        if (!resultado.Succeeded)
        {
            return RoleResult.Fallido("No se pudo eliminar el rol.", resultado.Errors.Select(e => e.Description));
        }

        return RoleResult.Ok($"Rol '{rolNormalizado}' eliminado exitosamente.");
    }
}
