
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

    /* 
     * A Usuario y Libro los rellena EF.
     * Por defecto se basa en UsuarioId y LibroId.
     * El virtual hace que EF haga lazy load. Tecnicamente no nos
     * haria falta por que planeamos incluir los datos de forma
     * explicita con el .Include() en FavoritoRepository.
     */
    public virtual Usuario Usuario { get; set; } = null!;

    [Required(ErrorMessage = "El libro es obligatorio.")]
    [Range(1, int.MaxValue,
        ErrorMessage = "El libro seleccionado no es válido.")]
    public int LibroId { get; set; }

    public virtual Libro Libro { get; set; } = null!;

    [Column(TypeName = "date")]
    [DataType(DataType.Date)]
    public DateTime? FechaAgregado { get; set; }
}
