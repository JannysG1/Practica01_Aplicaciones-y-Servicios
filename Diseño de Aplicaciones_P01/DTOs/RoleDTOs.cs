using System.ComponentModel.DataAnnotations;

namespace Diseño_de_Aplicaciones_P01.DTOs;

public record CreateRoleRequest(
    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "El nombre del rol debe tener entre 2 y 50 caracteres.")]
    string NombreRol
);

public record AssignRoleRequest(
    [Required(ErrorMessage = "El usuario o correo es obligatorio.")]
    string Usuario,

    [Required(ErrorMessage = "El nombre del rol es obligatorio.")]
    string Rol
);

public class RoleResult
{
    public bool Exitoso { get; set; }
    public string? Mensaje { get; set; }
    public IEnumerable<string>? Errores { get; set; }

    public static RoleResult Ok(string mensaje) =>
        new() { Exitoso = true, Mensaje = mensaje };

    public static RoleResult Fallido(string mensaje, IEnumerable<string>? errores = null) =>
        new() { Exitoso = false, Mensaje = mensaje, Errores = errores };
}
