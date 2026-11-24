using System.ComponentModel.DataAnnotations;

namespace Diseño_de_Aplicaciones_P01.DTOs;

/// <summary>
/// Parámetros de entrada para la generación de la tabla de potencias matemáticas.
/// </summary>
public record TablaPotenciasRequest(
    [Range(-1000.0, 1000.0, ErrorMessage = "La base debe situarse entre -1000 y 1000.")]
    double Base = 2.0,

    [Range(-50, 50, ErrorMessage = "El exponente inicial debe situarse entre -50 y 50.")]
    int ExponenteInicial = 0,

    [Range(-50, 50, ErrorMessage = "El exponente final debe situarse entre -50 y 50.")]
    int ExponenteFinal = 10
);

/// <summary>
/// Representa una fila individual en la tabla de potencias calculadas.
/// </summary>
public record FilaPotenciaDto(
    int Exponente,
    double Base,
    double Resultado,
    string Expresion
);

/// <summary>
/// Estructura de salida completa con los resultados tabulares de potencias y su representación textual.
/// </summary>
public class TablaPotenciasResponse
{
    public string Titulo { get; set; } = string.Empty;
    public double Base { get; set; }
    public int ExponenteInicial { get; set; }
    public int ExponenteFinal { get; set; }
    public int TotalFilas { get; set; }
    public List<FilaPotenciaDto> Filas { get; set; } = new();
    public string TablaFormatoTexto { get; set; } = string.Empty;
}

/// <summary>
/// Parámetros de entrada para la generación de la tabla de funciones trigonométricas.
/// </summary>
public record TablaTrigonometriaRequest(
    [Range(-3600.0, 3600.0, ErrorMessage = "El ángulo inicial debe situarse en el rango de -3600° a 3600°.")]
    double AnguloInicial = 0.0,

    [Range(-3600.0, 3600.0, ErrorMessage = "El ángulo final debe situarse en el rango de -3600° a 3600°.")]
    double AnguloFinal = 360.0,

    [Range(0.1, 180.0, ErrorMessage = "El paso angular debe ser mayor a 0.1° y menor o igual a 180°.")]
    double Paso = 15.0,

    [Range(0, 10, ErrorMessage = "La precisión decimal debe situarse entre 0 y 10 dígitos.")]
    int PrecisionDecimales = 4
);

/// <summary>
/// Representa una fila individual en la tabla trigonométrica con los valores angulares y sus funciones asociadas.
/// </summary>
public record FilaTrigonometricaDto(
    double AnguloGrados,
    double AnguloRadianes,
    double Seno,
    double Coseno,
    double? Tangente,
    string? Observacion
);

/// <summary>
/// Estructura de salida completa con los resultados tabulares de funciones trigonométricas y su representación textual.
/// </summary>
public class TablaTrigonometriaResponse
{
    public string Titulo { get; set; } = string.Empty;
    public double AnguloInicialGrados { get; set; }
    public double AnguloFinalGrados { get; set; }
    public double PasoGrados { get; set; }
    public int TotalFilas { get; set; }
    public List<FilaTrigonometricaDto> Filas { get; set; } = new();
    public string TablaFormatoTexto { get; set; } = string.Empty;
}
