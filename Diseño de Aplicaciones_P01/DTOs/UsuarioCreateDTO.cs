using System.ComponentModel.DataAnnotations;

namespace Diseño_de_Aplicaciones_P01.DTOs;

public class UsuarioCreateDTO
{
    [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string NombreUsuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "El correo electrónico es obligatorio.")]
    [EmailAddress(ErrorMessage = "El correo electrónico no tiene un formato válido.")]
    [StringLength(150, ErrorMessage = "El correo no puede tener más de 150 caracteres.")]
    public string CorreoElectronicousuario { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria.")]
    [StringLength(255, MinimumLength = 4, ErrorMessage = "La contraseña debe tener al menos 4 caracteres.")]
    public string Contrasenausuario { get; set; } = string.Empty;

    public bool Activo { get; set; } = true;
}
