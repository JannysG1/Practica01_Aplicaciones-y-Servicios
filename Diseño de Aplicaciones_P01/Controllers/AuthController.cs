using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Diseño_de_Aplicaciones_P01.recursos;

namespace Diseño_de_Aplicaciones_P01.Controllers;

public record LoginRequest(string Usuario, string Password);

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly IUsuarioRepository _usuarioRepo;

    public AuthController(IConfiguration config, IUsuarioRepository usuarioRepo)
    {
        _config = config;
        _usuarioRepo = usuarioRepo;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        string? rol = null;
        string nombreIdentidad = request.Usuario;

        // 1. Usuarios fijos para pruebas rápidas
        if (request.Usuario.Equals("admin", StringComparison.OrdinalIgnoreCase) && request.Password == "1234")
        {
            rol = "Admin";
        }
        else if ((request.Usuario.Equals("cliente", StringComparison.OrdinalIgnoreCase) || request.Usuario.Equals("usuario", StringComparison.OrdinalIgnoreCase)) && request.Password == "1234")
        {
            rol = "Usuario";
        }
        else
        {
            // 2. Autenticación con usuarios registrados en la base de datos
            var usuarioDb = _usuarioRepo.BuscarPorCorreo(request.Usuario);
            if (usuarioDb != null && usuarioDb.Activo && usuarioDb.Contrasenausuario == request.Password)
            {
                nombreIdentidad = usuarioDb.NombreUsuario;
                rol = (usuarioDb.NombreUsuario.Contains("Admin", StringComparison.OrdinalIgnoreCase) || 
                       usuarioDb.CorreoElectronicousuario.StartsWith("admin", StringComparison.OrdinalIgnoreCase))
                    ? "Admin"
                    : "Usuario";
            }
        }

        if (rol == null)
        {
            return Unauthorized(new { mensaje = "Usuario o contraseña incorrectos" });
        }

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, nombreIdentidad),
            new Claim(ClaimTypes.Role, rol),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            usuario = nombreIdentidad,
            rol = rol
        });
    }
}
