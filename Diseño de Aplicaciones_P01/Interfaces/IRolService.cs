using Diseño_de_Aplicaciones_P01.DTOs;

namespace Diseño_de_Aplicaciones_P01.Repositories;

public interface IRolService
{
    Task<List<string>> ObtenerRolesAsync();
    Task<RoleResult> CrearRolAsync(string nombreRol);
    Task<RoleResult> AsignarRolAUsuarioAsync(string usuarioOEmail, string nombreRol);
    Task<IList<string>> ObtenerRolesDeUsuarioAsync(string usuarioOEmail);
    Task<RoleResult> EliminarRolAsync(string nombreRol);
}
