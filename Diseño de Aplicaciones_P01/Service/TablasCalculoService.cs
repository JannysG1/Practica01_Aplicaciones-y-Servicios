/*
# **********************************************************
# Deber/Ejercicio 01
# Tu Nombre va aquí: Jannys Garrido
# Fecha de realización: 24/11/2026
# Fecha de entrega: 24/11/2026
# Resultados:
#  * El código permita generar una tabla con potencias y una tabla con ángulos y los
#    valores de las funciones seno y coseno
# Conclusiones:
#  * La abstracción mediante la interfaz ITablasCalculoService desacopla el cálculo matemático
#    de las capas de transporte y presentación, cumpliendo rigurosamente con SRP y DIP.
#  * El tratamiento numérico con normalización de residuos infinitesimales en múltiplos de 90°
#    garantiza exactitud física y consistencia matemática frente al estándar IEEE 754.
#  * La generación dual (estructurada en objetos DTO y renderizada en texto ASCII tabular)
#    optimiza tanto el consumo de clientes API como la legibilidad en consolas y reportes.
# Recomendaciones:
#  * Restringir los rangos de entrada en los DTOs para impedir anomalías por desbordamiento numérico
#    o degradación en tiempos de respuesta por tamaños excesivos de paso angular.
#  * Enfocar la documentación y comentarios del código en la intención del negocio y la semántica
#    del dominio, omitiendo descripciones mecánicas de sintaxis para preservar el principio Clean Code.
#  * Centralizar el cálculo de funciones trigonométricas en métodos puros e inmutables para facilitar
#    su prueba unitaria y futura extensión a otras funciones matemáticas.
# Bibliografía:
#  * Martin, R. C. (2008). Clean Code: A Handbook of Agile Software Craftsmanship. Prentice Hall.
#  * Microsoft Learn. Documentación de .NET: Operaciones matemáticas y diseño de APIs REST en C#.
#  * IEEE Standard for Floating-Point Arithmetic (IEEE 754-2019).
# **********************************************************
*/

using System.Text;
using Diseño_de_Aplicaciones_P01.DTOs;
using Diseño_de_Aplicaciones_P01.Repositories;

namespace Diseño_de_Aplicaciones_P01.Service;

/// <summary>
/// Provee la lógica de negocio para la generación, evaluación y formateo de tablas matemáticas.
/// Diseñado bajo el principio de responsabilidad única (SRP), delegando la presentación a DTOs.
/// </summary>
public class TablasCalculoService : ITablasCalculoService
{
    private const double GradosARadianes = Math.PI / 180.0;
    private const double ToleranciaCero = 1e-12;

    /// <summary>
    /// Construye la tabla de potencias sucesivas dentro del rango exponencial configurado.
    /// </summary>
    public TablaPotenciasResponse GenerarTablaPotencias(TablaPotenciasRequest? request = null)
    {
        var parametros = request ?? new TablaPotenciasRequest();
        var (inicio, fin) = NormalizarLimites(parametros.ExponenteInicial, parametros.ExponenteFinal);

        var filas = new List<FilaPotenciaDto>();
        for (int exponente = inicio; exponente <= fin; exponente++)
        {
            double resultado = Math.Pow(parametros.Base, exponente);
            filas.Add(new FilaPotenciaDto(
                Exponente: exponente,
                Base: parametros.Base,
                Resultado: Math.Round(resultado, 6),
                Expresion: $"{parametros.Base} ^ {exponente} = {FormatearNumero(resultado)}"
            ));
        }

        var respuesta = new TablaPotenciasResponse
        {
            Titulo = $"Tabla de Potencias para base {parametros.Base} (Exponentes: {inicio} a {fin})",
            Base = parametros.Base,
            ExponenteInicial = inicio,
            ExponenteFinal = fin,
            TotalFilas = filas.Count,
            Filas = filas
        };

        respuesta.TablaFormatoTexto = RenderizarTablaPotenciasAscii(respuesta);
        return respuesta;
    }

    /// <summary>
    /// Construye la tabla de funciones trigonométricas evaluando el barrido angular especificado.
    /// </summary>
    public TablaTrigonometriaResponse GenerarTablaTrigonometria(TablaTrigonometriaRequest? request = null)
    {
        var parametros = request ?? new TablaTrigonometriaRequest();
        var (inicio, fin) = NormalizarLimites(parametros.AnguloInicial, parametros.AnguloFinal);
        double paso = Math.Max(0.1, parametros.Paso);
        int decimales = Math.Clamp(parametros.PrecisionDecimales, 0, 10);

        var filas = new List<FilaTrigonometricaDto>();
        for (double angulo = inicio; angulo <= fin + (paso / 100.0); angulo += paso)
        {
            double anguloNormalizado = Math.Round(angulo, 2);
            double radianes = anguloNormalizado * GradosARadianes;

            double seno = EvaluarSeno(anguloNormalizado, radianes, decimales);
            double coseno = EvaluarCoseno(anguloNormalizado, radianes, decimales);
            (double? tangente, string observacion) = EvaluarTangente(anguloNormalizado, seno, coseno, decimales);

            filas.Add(new FilaTrigonometricaDto(
                AnguloGrados: anguloNormalizado,
                AnguloRadianes: Math.Round(radianes, decimales),
                Seno: seno,
                Coseno: coseno,
                Tangente: tangente,
                Observacion: observacion
            ));
        }

        var respuesta = new TablaTrigonometriaResponse
        {
            Titulo = $"Tabla Trigonométrica de Ángulos, Seno y Coseno ({inicio}° a {fin}°, paso {paso}°)",
            AnguloInicialGrados = inicio,
            AnguloFinalGrados = fin,
            PasoGrados = paso,
            TotalFilas = filas.Count,
            Filas = filas
        };

        respuesta.TablaFormatoTexto = RenderizarTablaTrigonometriaAscii(respuesta);
        return respuesta;
    }

    /// <summary>
    /// Genera exclusivamente la vista tabular en texto plano para tablas de potencias.
    /// </summary>
    public string ObtenerTablaPotenciasTexto(TablaPotenciasRequest? request = null)
    {
        return GenerarTablaPotencias(request).TablaFormatoTexto;
    }

    /// <summary>
    /// Genera exclusivamente la vista tabular en texto plano para funciones trigonométricas.
    /// </summary>
    public string ObtenerTablaTrigonometriaTexto(TablaTrigonometriaRequest? request = null)
    {
        return GenerarTablaTrigonometria(request).TablaFormatoTexto;
    }

    /// <summary>
    /// Evalúa la función seno suprimiendo residuos de coma flotante en múltiplos enteros de 180 grados.
    /// </summary>
    private static double EvaluarSeno(double anguloGrados, double radianes, int decimales)
    {
        if (EsMultiploExacto(anguloGrados, 180.0))
        {
            return 0.0;
        }

        double valor = Math.Sin(radianes);
        return Math.Abs(valor) < ToleranciaCero ? 0.0 : Math.Round(valor, decimales);
    }

    /// <summary>
    /// Evalúa la función coseno suprimiendo residuos de coma flotante en múltiplos impares de 90 grados.
    /// </summary>
    private static double EvaluarCoseno(double anguloGrados, double radianes, int decimales)
    {
        if (EsMultiploImpar(anguloGrados, 90.0))
        {
            return 0.0;
        }

        double valor = Math.Cos(radianes);
        return Math.Abs(valor) < ToleranciaCero ? 0.0 : Math.Round(valor, decimales);
    }

    /// <summary>
    /// Evalúa la tangente considerando la asíntota vertical en valores con coseno nulo.
    /// </summary>
    private static (double? Tangente, string Observacion) EvaluarTangente(double anguloGrados, double seno, double coseno, int decimales)
    {
        if (Math.Abs(coseno) < ToleranciaCero)
        {
            return (null, "Indeterminado (asíntota)");
        }

        if (Math.Abs(seno) < ToleranciaCero)
        {
            return (0.0, "Cero exacto");
        }

        double tangente = Math.Round(seno / coseno, decimales);
        return (tangente, "Definido");
    }

    /// <summary>
    /// Determina si un ángulo coincide con múltiplos exactos de un intervalo angular de referencia.
    /// </summary>
    private static bool EsMultiploExacto(double valor, double divisor)
    {
        double cociente = Math.Abs(valor / divisor);
        return Math.Abs(cociente - Math.Round(cociente)) < ToleranciaCero;
    }

    /// <summary>
    /// Determina si un ángulo corresponde a una perpendicular con asíntota respecto al eje cartesiano.
    /// </summary>
    private static bool EsMultiploImpar(double valor, double divisor)
    {
        double cociente = Math.Abs(valor / divisor);
        long redondeado = (long)Math.Round(cociente);
        return Math.Abs(cociente - redondeado) < ToleranciaCero && (redondeado % 2 != 0);
    }

    /// <summary>
    /// Ordena consistentemente los límites numéricos inicial y final.
    /// </summary>
    private static (int Inicio, int Fin) NormalizarLimites(int a, int b) =>
        a <= b ? (a, b) : (b, a);

    /// <summary>
    /// Ordena consistentemente los límites angulares inicial y final.
    /// </summary>
    private static (double Inicio, double Fin) NormalizarLimites(double a, double b) =>
        a <= b ? (a, b) : (b, a);

    /// <summary>
    /// Provee formato numérico legible para valores enteros y decimales.
    /// </summary>
    private static string FormatearNumero(double valor) =>
        valor % 1 == 0 ? valor.ToString("F0") : valor.ToString("F4");

    /// <summary>
    /// Renderiza la cuadrícula ASCII para visualización uniforme de la tabla de potencias.
    /// </summary>
    private static string RenderizarTablaPotenciasAscii(TablaPotenciasResponse datos)
    {
        var sb = new StringBuilder();
        sb.AppendLine("+------------+------------+--------------------+------------------------------+");
        sb.AppendLine("| Exponente  |    Base    |     Resultado      | Expresión Matemática         |");
        sb.AppendLine("+------------+------------+--------------------+------------------------------+");

        foreach (var fila in datos.Filas)
        {
            sb.AppendLine($"| {fila.Exponente,10} | {fila.Base,10:F2} | {fila.Resultado,18:N4} | {fila.Expresion,-28} |");
        }

        sb.AppendLine("+------------+------------+--------------------+------------------------------+");
        return sb.ToString();
    }

    /// <summary>
    /// Renderiza la cuadrícula ASCII para visualización uniforme de la tabla de funciones trigonométricas.
    /// </summary>
    private static string RenderizarTablaTrigonometriaAscii(TablaTrigonometriaResponse datos)
    {
        var sb = new StringBuilder();
        sb.AppendLine("+-------------+-------------+------------+------------+------------+--------------------------+");
        sb.AppendLine("| Ángulo (°)  | Ángulo(rad) |    Seno    |   Coseno   |  Tangente  | Observación              |");
        sb.AppendLine("+-------------+-------------+------------+------------+------------+--------------------------+");

        foreach (var fila in datos.Filas)
        {
            string tangenteTexto = fila.Tangente.HasValue ? fila.Tangente.Value.ToString("F4") : "N/D";
            sb.AppendLine($"| {fila.AnguloGrados,11:F1} | {fila.AnguloRadianes,11:F4} | {fila.Seno,10:F4} | {fila.Coseno,10:F4} | {tangenteTexto,10} | {fila.Observacion,-24} |");
        }

        sb.AppendLine("+-------------+-------------+------------+------------+------------+--------------------------+");
        return sb.ToString();
    }
}
