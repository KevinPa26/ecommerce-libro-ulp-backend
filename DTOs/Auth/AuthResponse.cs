namespace ecommerce_libro_ulp_backend.DTOs.Auth;

public class AuthResponse
{
    public string Token { get; set; } = string.Empty;

    public DateTime ExpiraEn { get; set; }

    public UsuarioAuthResponse Usuario { get; set; } = new();
}

public class UsuarioAuthResponse
{
    public int Id { get; set; }

    public string Nombre { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Rol { get; set; } = string.Empty;

    public string? Avatar { get; set; }
}