using Diseño_de_Aplicaciones_P01.DTOs;

namespace Diseño_de_Aplicaciones_P01.Repositories;

public interface IAuthService
{
    Task<AuthResult> LoginAsync(LoginRequest request);
    Task<RegisterResult> RegisterAsync(RegisterRequest request);
}
