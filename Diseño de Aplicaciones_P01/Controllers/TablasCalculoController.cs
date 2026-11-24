using System.Net.Mime;
using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Diseño_de_Aplicaciones_P01.Controllers;

/// <summary>
/// Controlador HTTP para la exposición de endpoints de cálculo y visualización de tablas matemáticas.
/// Aplica el principio de inversión de dependencias (DIP) delegando la computación a ITablasCalculoService.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("api/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class TablasCalculoController : ControllerBase
{
    private readonly ITablasCalculoService _tablasCalculoService;

    /// <summary>
    /// Inicializa una nueva instancia del controlador inyectando la abstracción del servicio de cálculo.
    /// </summary>
    public TablasCalculoController(ITablasCalculoService tablasCalculoService)
    {
        _tablasCalculoService = tablasCalculoService;
    }

    /// <summary>
    /// Obtiene la tabla de potencias sucesivas a partir de parámetros en cadena de consulta.
    /// </summary>
    [HttpGet("potencias")]
    [ProducesResponseType(typeof(TablaPotenciasResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TablaPotenciasResponse> ObtenerTablaPotencias(
        [FromQuery] double baseNumero = 2.0,
        [FromQuery] int inicio = 0,
        [FromQuery] int fin = 10)
    {
        var request = new TablaPotenciasRequest(baseNumero, inicio, fin);
        if (!TryValidateModel(request))
        {
            return BadRequest(ModelState);
        }

        var resultado = _tablasCalculoService.GenerarTablaPotencias(request);
        return Ok(resultado);
    }

    /// <summary>
    /// Genera la tabla de potencias procesando especificaciones enviadas en el cuerpo de la petición.
    /// </summary>
    [HttpPost("potencias")]
    [ProducesResponseType(typeof(TablaPotenciasResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TablaPotenciasResponse> GenerarTablaPotencias([FromBody] TablaPotenciasRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = _tablasCalculoService.GenerarTablaPotencias(request);
        return Ok(resultado);
    }

    /// <summary>
    /// Retorna la representación visual en texto plano (ASCII tabular) de la tabla de potencias.
    /// </summary>
    [HttpGet("potencias/texto")]
    [Produces(MediaTypeNames.Text.Plain)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult ObtenerTablaPotenciasTexto(
        [FromQuery] double baseNumero = 2.0,
        [FromQuery] int inicio = 0,
        [FromQuery] int fin = 10)
    {
        var request = new TablaPotenciasRequest(baseNumero, inicio, fin);
        string textoAscii = _tablasCalculoService.ObtenerTablaPotenciasTexto(request);
        return Content(textoAscii, MediaTypeNames.Text.Plain);
    }

    /// <summary>
    /// Obtiene la tabla de ángulos y funciones seno y coseno mediante parámetros de consulta.
    /// </summary>
    [HttpGet("trigonometria")]
    [ProducesResponseType(typeof(TablaTrigonometriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TablaTrigonometriaResponse> ObtenerTablaTrigonometria(
        [FromQuery] double inicio = 0.0,
        [FromQuery] double fin = 360.0,
        [FromQuery] double paso = 15.0,
        [FromQuery] int decimales = 4)
    {
        var request = new TablaTrigonometriaRequest(inicio, fin, paso, decimales);
        if (!TryValidateModel(request))
        {
            return BadRequest(ModelState);
        }

        var resultado = _tablasCalculoService.GenerarTablaTrigonometria(request);
        return Ok(resultado);
    }

    /// <summary>
    /// Genera la tabla de funciones trigonométricas a partir de especificaciones en el cuerpo de la petición.
    /// </summary>
    [HttpPost("trigonometria")]
    [ProducesResponseType(typeof(TablaTrigonometriaResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TablaTrigonometriaResponse> GenerarTablaTrigonometria([FromBody] TablaTrigonometriaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var resultado = _tablasCalculoService.GenerarTablaTrigonometria(request);
        return Ok(resultado);
    }

    /// <summary>
    /// Retorna la representación visual en texto plano (ASCII tabular) de la tabla trigonométrica.
    /// </summary>
    [HttpGet("trigonometria/texto")]
    [Produces(MediaTypeNames.Text.Plain)]
    [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
    public IActionResult ObtenerTablaTrigonometriaTexto(
        [FromQuery] double inicio = 0.0,
        [FromQuery] double fin = 360.0,
        [FromQuery] double paso = 15.0,
        [FromQuery] int decimales = 4)
    {
        var request = new TablaTrigonometriaRequest(inicio, fin, paso, decimales);
        string textoAscii = _tablasCalculoService.ObtenerTablaTrigonometriaTexto(request);
        return Content(textoAscii, MediaTypeNames.Text.Plain);
    }
}
