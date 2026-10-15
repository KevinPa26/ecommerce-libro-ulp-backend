
using System.ComponentModel.DataAnnotations;

namespace ecommerce_libro_ulp_backend.Models;

public class ImagenLibro
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El libro es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El libro seleccionado no es válido.")]
    public int LibroId { get; set; }

    [Required(ErrorMessage = "La ruta de la imagen es obligatoria.")]
    [StringLength(255, MinimumLength = 1,
        ErrorMessage = "La ruta no puede superar los 255 caracteres.")]
    public string Path { get; set; } = string.Empty;

    public bool EsPrincipal { get; set; } = false;
}
