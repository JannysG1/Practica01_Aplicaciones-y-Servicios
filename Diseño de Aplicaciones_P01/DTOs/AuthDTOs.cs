namespace Diseño_de_Aplicaciones_P01.DTOs;

public record LoginRequest(string Usuario, string Password);

public record RegisterRequest(string Usuario, string Email, string Password, string NombreCompleto);

public class AuthResult
{
    public bool Exitoso { get; set; }
    public string? MensajeError { get; set; }
    public bool EstaBloqueado { get; set; }
    public string? Token { get; set; }
    public DateTime? Expira { get; set; }
    public string? Usuario { get; set; }
    public IList<string>? Roles { get; set; }

    public static AuthResult Ok(string token, DateTime expira, string usuario, IList<string> roles) =>
        new() { Exitoso = true, Token = token, Expira = expira, Usuario = usuario, Roles = roles };

    public static AuthResult Fallido(string mensaje, bool estaBloqueado = false) =>
        new() { Exitoso = false, MensajeError = mensaje, EstaBloqueado = estaBloqueado };
}

public class RegisterResult
{
    public bool Exitoso { get; set; }
    public string? Usuario { get; set; }
    public IEnumerable<string>? Errores { get; set; }

    public static RegisterResult Ok(string usuario) =>
        new() { Exitoso = true, Usuario = usuario };

    public static RegisterResult Fallido(IEnumerable<string> errores) =>
        new() { Exitoso = false, Errores = errores };
}
