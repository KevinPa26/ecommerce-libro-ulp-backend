
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models;

public class Compra
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El comprador es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El comprador seleccionado no es válido.")]
    public int UsuarioId { get; set; }

    [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime FechaCompra { get; set; } = DateTime.Today;
}
