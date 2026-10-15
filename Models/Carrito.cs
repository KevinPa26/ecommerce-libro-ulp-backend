
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models;

public class Carrito
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El usuario es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El usuario seleccionado no es válido.")]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "El libro es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El libro seleccionado no es válido.")]
    public int LibroId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "La cantidad debe ser como mínimo 1.")]
    public int Cantidad { get; set; } = 1;

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime? FechaAgregado { get; set; }
}
