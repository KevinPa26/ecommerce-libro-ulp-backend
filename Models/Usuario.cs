using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ecommerce_libro_ulp_backend.Models
{
    public enum RolUsuario
    {
        ADMIN,
        USUARIO
    }

    public enum EstadoUsuario
    {
        ACTIVO,
        INACTIVO,
        SUSPENDIDO
    }

    public class Usuario
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100,MinimumLength = 2, ErrorMessage = "El nombre debe tener entre 2 y 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;


        [StringLength(255, ErrorMessage = "El avatar no puede superar los 255 caracteres.")]
        public string Avatar { get; set; } = string.Empty;

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public RolUsuario Rol { get; set; } = RolUsuario.USUARIO;

        [Required(ErrorMessage = "El email es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del email no es válido.")]
        [StringLength(254)]
        public string Email { get; set; } = string.Empty;

        // Este campo almacena el hash de la contraseña.
        // No debe mostrarse directamente en las vistas.
        // ver si esto lo trae
        [StringLength(255)]
        public string PasswordHash { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        public EstadoUsuario Activo { get; set; } = EstadoUsuario.ACTIVO;

        public DateTime FechaRegistro { get; set; }

        public int? UsuarioCreador { get; set; }

        public int? UsuarioDelete { get; set; }
        public string RolNombre => Rol.ToString();

    }
}

