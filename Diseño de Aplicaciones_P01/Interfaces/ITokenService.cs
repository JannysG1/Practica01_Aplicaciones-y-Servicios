using Diseño_de_Aplicaciones_P01.Models;

namespace Diseño_de_Aplicaciones_P01.Repositories;

public interface ITokenService
{
    (string Token, DateTime Expira) GenerarToken(ApplicationUser usuario, IList<string> roles);
}
