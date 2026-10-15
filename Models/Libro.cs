
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models;

public enum EstadoLibro
{
    DISPONIBLE,
    AGOTADO,
    INACTIVO
}

public class Libro
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El vendedor es obligatorio.")]
    [Range(1, int.MaxValue, ErrorMessage = "El vendedor seleccionado no es válido.")]
    public int VendedorId { get; set; }

    [StringLength(13, MinimumLength = 13,
        ErrorMessage = "El ISBN-13 debe contener exactamente 13 caracteres.")]
    [RegularExpression(@"^\d{13}$",
        ErrorMessage = "El ISBN-13 debe contener únicamente números.")]
    public string? Isbn13 { get; set; }

    [Required(ErrorMessage = "El título es obligatorio.")]
    [StringLength(150, MinimumLength = 1,
        ErrorMessage = "El título debe tener entre 1 y 150 caracteres.")]
    public string Titulo { get; set; } = string.Empty;

    [Required(ErrorMessage = "El autor es obligatorio.")]
    [StringLength(100, MinimumLength = 2,
        ErrorMessage = "El autor debe tener entre 2 y 100 caracteres.")]
    public string Autor { get; set; } = string.Empty;

    [StringLength(65535,
        ErrorMessage = "La descripción del vendedor es demasiado extensa.")]
    public string? DescripcionVendedor { get; set; }

    [StringLength(65535,
        ErrorMessage = "La descripción del libro es demasiado extensa.")]
    public string? DescripcionLibro { get; set; }

    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0.01", "99999999.99",
        ErrorMessage = "El precio debe ser mayor a cero y no superar 99.999.999,99.")]
    public decimal Precio { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "El stock no puede ser negativo.")]
    public int Stock { get; set; } = 0;

    [Required(ErrorMessage = "El estado es obligatorio.")]
    public EstadoLibro Estado { get; set; } = EstadoLibro.DISPONIBLE;

    public int? UsuarioCreador { get; set; }

    public int? UsuarioDelete { get; set; }
}
