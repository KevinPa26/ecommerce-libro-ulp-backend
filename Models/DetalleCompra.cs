
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models;

public class DetalleCompra
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "La compra es obligatoria.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "La compra seleccionada no es válida.")]
    public int CompraId { get; set; }

    [Required(ErrorMessage = "El libro es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El libro seleccionado no es válido.")]
    public int LibroId { get; set; }

    [Range(1, int.MaxValue,
        ErrorMessage = "La cantidad debe ser como mínimo 1.")]
    public int Cantidad { get; set; }

    [Required(ErrorMessage = "El precio unitario es obligatorio.")]
    [Column(TypeName = "decimal(10,2)")]
    [Range(typeof(decimal), "0.01", "99999999.99",
        ErrorMessage = "El precio unitario debe ser mayor a cero.")]
    public decimal PrecioUnitario { get; set; }

    [Range(0, int.MaxValue,
        ErrorMessage = "Los puntos no pueden ser negativos.")]
    public int Puntos { get; set; } = 0;
}
