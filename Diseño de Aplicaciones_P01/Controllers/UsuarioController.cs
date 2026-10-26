using Microsoft.AspNetCore.Mvc;
using Diseño_de_Aplicaciones_P01.recursos; 
using Diseño_de_Aplicaciones_P01.model;

namespace MiPrimeraApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly repos_usuario _repositorio;

        public UsuariosController(repos_usuario repos)
        {
            _repositorio = repos;
        }

        [HttpGet]
        public ICollection<Usuario> ObtenerUsuarios()
        {
            return _repositorio.ObtenerUsuarios();
        }
        [HttpGet("{id}")]
        public ActionResult<Usuario> BuscarPorId(int id)
        {
            var usuario = _repositorio.BuscarPorId(id);

            if (usuario == null)
            {
                return NotFound($"Usuario con el ID {id} no encontrado."); 
            }
            return Ok(usuario); 
        }

        [HttpPost]
        public ActionResult AgregarUsuario([FromBody] Usuario nuevoUsuario)
        {
            _repositorio.AgregarUsuario(nuevoUsuario);
            return Ok(nuevoUsuario);
        }

        [HttpDelete("{id}")]
        public ActionResult EliminarPorId(int id)
        {
            bool eliminado = _repositorio.EliminarPorId(id);
            if (!eliminado)
            {
                return NotFound($"No se puede eliminar: Usuario con ID {id} no encontrado.");
            }
            return NoContent();
        }

    }
}