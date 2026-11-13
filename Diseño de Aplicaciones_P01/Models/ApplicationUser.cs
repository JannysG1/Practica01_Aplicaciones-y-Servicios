using Microsoft.AspNetCore.Identity;

namespace Diseño_de_Aplicaciones_P01.Models;

public class ApplicationUser : IdentityUser
{
    public string NombreCompleto { get; set; } = string.Empty;
}
