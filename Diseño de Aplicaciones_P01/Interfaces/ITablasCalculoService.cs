using Diseño_de_Aplicaciones_P01.DTOs;

namespace Diseño_de_Aplicaciones_P01.Repositories;

/// <summary>
/// Contrato de servicio para el cómputo y formateo de tablas matemáticas y trigonométricas.
/// Aplica el principio de segregación de interfaces (ISP) delimitando operaciones de cálculo numérico.
/// </summary>
public interface ITablasCalculoService
{
    /// <summary>
    /// Genera la tabla de potencias sucesivas a partir de la base y el rango de exponentes definidos.
    /// </summary>
    /// <param name="request">Configuración de base y rango exponencial.</param>
    /// <returns>Estructura tabular con los valores de las potencias y su formato de texto.</returns>
    TablaPotenciasResponse GenerarTablaPotencias(TablaPotenciasRequest? request = null);

    /// <summary>
    /// Genera la tabla de funciones trigonométricas (seno y coseno) para una secuencia angular.
    /// </summary>
    /// <param name="request">Configuración del barrido angular en grados y precisión requerida.</param>
    /// <returns>Estructura tabular con ángulos en grados, radianes y valores trigonométricos evaluados.</returns>
    TablaTrigonometriaResponse GenerarTablaTrigonometria(TablaTrigonometriaRequest? request = null);

    /// <summary>
    /// Produce la representación textual en formato ASCII alineado para la tabla de potencias.
    /// </summary>
    string ObtenerTablaPotenciasTexto(TablaPotenciasRequest? request = null);

    /// <summary>
    /// Produce la representación textual en formato ASCII alineado para la tabla de ángulos, senos y cosenos.
    /// </summary>
    string ObtenerTablaTrigonometriaTexto(TablaTrigonometriaRequest? request = null);
}
