using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var resultado = await _authService.LoginAsync(request);

        if (!resultado.Exitoso)
        {
            return Unauthorized(new { mensaje = resultado.MensajeError });
        }

        return Ok(new
        {
            token = resultado.Token,
            expira = resultado.Expira,
            usuario = resultado.Usuario,
            roles = resultado.Roles
        });
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var resultado = await _authService.RegisterAsync(request);

        if (!resultado.Exitoso)
        {
            return BadRequest(new { errores = resultado.Errores });
        }

        return Ok(new
        {
            mensaje = "Usuario registrado correctamente",
            usuario = resultado.Usuario
        });
    }
}
