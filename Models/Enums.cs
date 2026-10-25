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

    public enum EstadoLibro
    {
        DISPONIBLE,
        AGOTADO,
        INACTIVO
    }
}