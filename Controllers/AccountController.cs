using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Text;
using ecommerce_libro_ulp_backend.DTOs.Auth;
using ecommerce_libro_ulp_backend.Models;
using ecommerce_libro_ulp_backend.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace ecommerce_libro_ulp_backend.Controllers;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly IUsuarioRepository _usuarioRepository;
    private readonly IConfiguration _configuration;
    private readonly IWebHostEnvironment _environment;

    private const long MaxAvatarBytes = 2 * 1024 * 1024;

    private static readonly HashSet<string> ExtensionesPermitidas =
        new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

    public AccountController(
        IUsuarioRepository usuarioRepository,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _usuarioRepository = usuarioRepository;
        _configuration = configuration;
        _environment = environment;
    }

    // POST: api/account/register
    [AllowAnonymous]
    [HttpPost("register")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> Registrar(
        [FromForm] RegistroRequest request,
        [FromQuery] string? redirectUrl = null)
    {
        var email = request.Email.Trim();

        if (await _usuarioRepository.ObtenerPorEmail(email) is not null)
        {
            return Conflict(new
            {
                mensaje = "Ya existe una cuenta activa con ese email."
            });
        }

        string? rutaAvatar = null;
        Usuario usuario;

        try
        {
            if (request.Avatar is not null)
            {
                var resultadoAvatar = await GuardarAvatar(request.Avatar);

                if (!resultadoAvatar.Exito)
                {
                    return BadRequest(new
                    {
                        mensaje = resultadoAvatar.Error
                    });
                }

                rutaAvatar = resultadoAvatar.Ruta;
            }

            usuario = new Usuario
            {
                Nombre = request.Nombre.Trim(),
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(
                    request.Password),
                Rol = RolUsuario.USUARIO,
                Estado = EstadoUsuario.ACTIVO,
                Avatar = rutaAvatar ?? string.Empty,
                FechaRegistro = DateTime.UtcNow
            };

            await _usuarioRepository.Crear(usuario);
        }
        catch (DbUpdateException)
        {
            if (rutaAvatar is not null)
            {
                EliminarArchivoAvatar(rutaAvatar);
            }

            return Conflict(new
            {
                mensaje = "No se pudo registrar la cuenta. Verificá si el email ya está registrado."
            });
        }
        catch
        {
            if (rutaAvatar is not null)
            {
                EliminarArchivoAvatar(rutaAvatar);
            }

            throw;
        }

        // Generamos el JWT para mantener compatibilidad con los clientes API.
        var respuesta = CrearRespuestaAutenticacion(usuario);

        // Creamos la cookie de autenticación para la aplicación MVC.
        var claims = CrearClaims(usuario);

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identidad);

        var propiedades = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            AllowRefresh = true
        };

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            propiedades
        );

        // Solo aceptamos destinos locales para evitar redirecciones externas.
        if (!string.IsNullOrWhiteSpace(redirectUrl) &&
            Url.IsLocalUrl(redirectUrl))
        {
            return Ok(new
            {
                respuesta,
                redirectUrl
            });
        }

        return Ok(respuesta);
    }


    // POST: api/account/login
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request,
        [FromQuery] string? redirectUrl = null)
    {
        var email = request.Email.Trim();

        var usuario = await _usuarioRepository.ObtenerPorEmail(email);

        // Mensaje genérico para no revelar si un email está registrado.
        if (usuario is null ||
            !BCrypt.Net.BCrypt.Verify(
                request.Password,
                usuario.PasswordHash))
        {
            return Unauthorized(new
            {
                mensaje = "Email o contraseña incorrectos."
            });
        }

        // Generamos el JWT como hasta ahora.
        var respuesta = CrearRespuestaAutenticacion(usuario);

        // Creamos la identidad para la cookie con las mismas claims.
        var claims = CrearClaims(usuario);

        var identidad = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        var principal = new ClaimsPrincipal(identidad);

        var propiedades = new AuthenticationProperties
        {
            IsPersistent = true,
            ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8),
            AllowRefresh = true
        };

        // El navegador recibirá la cookie de autenticación.
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            propiedades
        );

        // Si se proporcionó un destino local válido, lo devolvemos.
        if (!string.IsNullOrWhiteSpace(redirectUrl) &&
            Url.IsLocalUrl(redirectUrl))
        {
            return Ok(new
            {
                respuesta,
                redirectUrl
            });
        }

        // Conservamos el comportamiento actual para clientes que usan JWT.
        return Ok(respuesta);
    }


    private List<Claim> CrearClaims(Usuario usuario)
    {
        var claims = new List<Claim>
        {
            new Claim(
                ClaimTypes.NameIdentifier,
                usuario.Id.ToString()),

            new Claim(
                ClaimTypes.Name,
                usuario.Nombre),

            new Claim(
                ClaimTypes.Email,
                usuario.Email),

            new Claim(
                ClaimTypes.Role,
                usuario.Rol.ToString())
        };

        if (!string.IsNullOrWhiteSpace(usuario.Avatar))
        {
            claims.Add(new Claim("avatar", usuario.Avatar));
        }

        return claims;
    }

    // GET: api/account/me
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> ObtenerPerfil()
    {
        var idClaim = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (!int.TryParse(idClaim, out var usuarioId))
        {
            return Unauthorized(new
            {
                mensaje = "El token no contiene una identidad válida."
            });
        }

        var usuario = await _usuarioRepository.ObtenerPorId(usuarioId);

        if (usuario is null ||
            usuario.Estado != EstadoUsuario.ACTIVO)
        {
            return Unauthorized(new
            {
                mensaje = "La cuenta no está disponible."
            });
        }

        return Ok(new UsuarioAuthResponse
        {
            Id = usuario.Id,
            Nombre = usuario.Nombre,
            Email = usuario.Email,
            Rol = usuario.Rol.ToString(),
            Avatar = usuario.Avatar
        });
    }


    // PUT: api/account/avatar
    [Authorize]
    [HttpPut("avatar")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(3 * 1024 * 1024)]
    public async Task<IActionResult> ActualizarAvatar(
        [FromForm] IFormFile? avatar)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!int.TryParse(idClaim, out var usuarioId))
        {
            return Unauthorized(new
            {
                mensaje = "No se pudo identificar al usuario."
            });
        }

        if (avatar is null)
        {
            return BadRequest(new
            {
                mensaje = "Seleccioná una imagen."
            });
        }

        var usuario = await _usuarioRepository.ObtenerPorId(usuarioId);

        if (usuario is null || usuario.Estado != EstadoUsuario.ACTIVO)
        {
            return Unauthorized(new
            {
                mensaje = "La cuenta no está disponible."
            });
        }

        var resultado = await GuardarAvatar(avatar);

        if (!resultado.Exito)
        {
            return BadRequest(new
            {
                mensaje = resultado.Error
            });
        }

        var avatarAnterior = usuario.Avatar;

        try
        {
            usuario.Avatar = resultado.Ruta!;

            await _usuarioRepository.Modificar(usuario);

            // Eliminamos el avatar anterior únicamente después
            // de guardar correctamente la nueva ruta.
            if (!string.IsNullOrWhiteSpace(avatarAnterior) &&
                avatarAnterior.StartsWith(
                    "/uploads/avatars/",
                    StringComparison.OrdinalIgnoreCase))
            {
                EliminarArchivoAvatar(avatarAnterior);
            }

            return Ok(new
            {
                mensaje = "Avatar actualizado correctamente.",
                avatar = usuario.Avatar
            });
        }
        catch
        {
            // Si falla la actualización, limpiamos el archivo nuevo.
            EliminarArchivoAvatar(resultado.Ruta!);
            throw;
        }
    }


    private AuthResponse CrearRespuestaAutenticacion(Usuario usuario)
    {
        var secret = _configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException(
                "Falta configurar Jwt:Secret.");

        var issuer = _configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException(
                "Falta configurar Jwt:Issuer.");

        var audience = _configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException(
                "Falta configurar Jwt:Audience.");

        var expiracionMinutos = int.TryParse(
            _configuration["Jwt:ExpiresMinutes"],
            out var minutos)
                ? minutos
                : 60;

        if (expiracionMinutos <= 0)
        {
            expiracionMinutos = 60;
        }

        var ahora = DateTime.UtcNow;
        var expiraEn = ahora.AddMinutes(expiracionMinutos);

        var claims = CrearClaims(usuario);

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(secret));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: ahora,
            expires: expiraEn,
            signingCredentials: credentials);

        var tokenGenerado = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return new AuthResponse
        {
            Token = tokenGenerado,
            ExpiraEn = expiraEn,
            Usuario = new UsuarioAuthResponse
            {
                Id = usuario.Id,
                Nombre = usuario.Nombre,
                Email = usuario.Email,
                Rol = usuario.Rol.ToString(),
                Avatar = usuario.Avatar
            }
        };
    }

    private async Task<(bool Exito, string? Ruta, string? Error)>
        GuardarAvatar(IFormFile archivo)
    {
        if (archivo.Length <= 0)
        {
            return (false, null, "El archivo está vacío.");
        }

        if (archivo.Length > MaxAvatarBytes)
        {
            return (false, null, "El avatar no puede superar los 2 MB.");
        }

        var extension = Path.GetExtension(archivo.FileName);

        if (!ExtensionesPermitidas.Contains(extension))
        {
            return (
                false,
                null,
                "El avatar debe ser JPG, JPEG, PNG o WEBP.");
        }

        var tiposPermitidos = new Dictionary<string, string>(
            StringComparer.OrdinalIgnoreCase)
        {
            [".jpg"] = "image/jpeg",
            [".jpeg"] = "image/jpeg",
            [".png"] = "image/png",
            [".webp"] = "image/webp"
        };

        if (!tiposPermitidos.TryGetValue(extension, out var tipoEsperado) ||
            !string.Equals(
                archivo.ContentType,
                tipoEsperado,
                StringComparison.OrdinalIgnoreCase))
        {
            return (false, null, "El tipo de archivo no es válido.");
        }

        var carpeta = Path.Combine(
            _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot"),
            "uploads",
            "avatars");

        Directory.CreateDirectory(carpeta);

        // Nunca usamos el nombre original del archivo en el servidor.
        var nombreArchivo = $"{Guid.NewGuid():N}{extension.ToLowerInvariant()}";
        var rutaFisica = Path.Combine(carpeta, nombreArchivo);

        await using (var stream = new FileStream(
            rutaFisica,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
        {
            await archivo.CopyToAsync(stream);
        }

        return (
            true,
            $"/uploads/avatars/{nombreArchivo}",
            null);
    }

    private void EliminarArchivoAvatar(string rutaRelativa)
    {
        try
        {
            var rutaBase = _environment.WebRootPath
                ?? Path.Combine(_environment.ContentRootPath, "wwwroot");

            var nombreArchivo = Path.GetFileName(rutaRelativa);
            var rutaFisica = Path.Combine(
                rutaBase,
                "uploads",
                "avatars",
                nombreArchivo);

            if (System.IO.File.Exists(rutaFisica))
            {
                System.IO.File.Delete(rutaFisica);
            }
        }
        catch
        {
            // No ocultamos el resultado de la operación principal
            // por un error al limpiar un archivo temporal.
        }
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(
            CookieAuthenticationDefaults.AuthenticationScheme
        );

        return Ok(new { mensaje = "Sesión cerrada correctamente." });
    }
}