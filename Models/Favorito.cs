
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models;

public class Favorito
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

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime? FechaAgregado { get; set; }
}
