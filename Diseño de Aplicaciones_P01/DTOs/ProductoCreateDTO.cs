using System.ComponentModel.DataAnnotations;

namespace Diseño_de_Aplicaciones_P01.DTOs;

public class ProductoCreateDTO
{
    [Required(ErrorMessage = "El nombre del producto es obligatorio.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
    public string Nombre { get; set; } = string.Empty;

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, 10000.00, ErrorMessage = "El precio debe ser mayor que 0.")]
    public decimal Precio { get; set; }

    public bool Activo { get; set; } = true;
}
