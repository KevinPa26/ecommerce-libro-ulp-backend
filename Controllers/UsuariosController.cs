using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;
using System.Diagnostics;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<UsuariosController> _logger;

        // 1. Constantes/Configuraciones a nivel de Controlador para la imagen
        private const long TAMANIO_MAXIMO_AVATAR_BYTES = 2 * 1024 * 1024; // 2 MB
        private static readonly string[] EXTENSIONES_PERMITIDAS_AVATAR = { ".jpg", ".jpeg", ".png", ".webp" };
        private const string CARPETA_SUBIDA_AVATAR = "uploads/avatars";

        public UsuariosController(IUsuarioRepository usuarioRepository, IWebHostEnvironment webHostEnvironment, ILogger<UsuariosController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index(
            string? busqueda,
            RolUsuario? rol,
            EstadoUsuario? estado,
            int page = 1)
        {
            try
            {
                const int pageSize = 5;

                page = Math.Max(1, page);

                var resultado = await _usuarioRepository.ObtenerPaginados(
                    busqueda,
                    rol,
                    estado,
                    page,
                    pageSize
                );

                int totalPages = (int)Math.Ceiling(
                    resultado.TotalCount / (double)pageSize
                );

                totalPages = Math.Max(1, totalPages);

                // Si solicitan una página fuera de rango, usamos la última válida.
                if (page > totalPages)
                {
                    page = totalPages;

                    resultado = await _usuarioRepository.ObtenerPaginados(
                        busqueda,
                        rol,
                        estado,
                        page,
                        pageSize
                    );
                }

                ViewBag.Busqueda = busqueda;
                ViewBag.Rol = rol;
                ViewBag.Estado = estado;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = totalPages;
                ViewBag.TotalRegistros = resultado.TotalCount;

                return View(resultado.Items);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener usuarios paginados.");

                var errorModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
                };

                return View("Error", errorModel);
            }
        }

        // GET: Usuarios/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerPorId(id);
                if (usuario == null)
                {
                    _logger.LogWarning("Intento de ver detalles de un usuario inexistente con ID: {UsuarioId}", id);
                    return NotFound();
                }

                return View(usuario);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del usuario con ID: {UsuarioId}", id);
                return View("Error");
            }
        }

        // GET: Usuarios/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Usuarios/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Usuario usuario, IFormFile? archivoAvatar)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // PASO 1: Validar si el email ya existe en la base de datos
                    var usuarioExistente = await _usuarioRepository.ObtenerPorEmail(usuario.Email);
                    if (usuarioExistente != null)
                    {
                        ModelState.AddModelError("Email", "Ya existe un usuario registrado con este correo electrónico.");
                        return View(usuario);
                    }

                    // PASO 2: Hashear contraseña y Crear el usuario primero
                    if (string.IsNullOrEmpty(usuario.Password))
                    {
                        ModelState.AddModelError("Password", "La contraseña es obligatoria.");
                        return View(usuario);
                    }

                    usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Password);

                    // Insertar usuario en la base de datos (aquí se genera su Id)
                    await _usuarioRepository.Crear(usuario);

                    // PASO 3: Si se subió avatar, procesar/validar la imagen usando la función auxiliar
                    if (archivoAvatar != null && archivoAvatar.Length > 0)
                    {
                        var (esValido, mensajeError, rutaRelativa) = await ProcesarYGuardarAvatarAsync(archivoAvatar, usuario.Email);

                        if (!esValido)
                        {
                            // Si la imagen falla en validaciones (peso/formato), notificamos el error.
                            // Nota: El usuario ya quedó registrado, pero se requiere arreglar el avatar.
                            ModelState.AddModelError("Avatar", mensajeError);
                            return View(usuario);
                        }

                        // PASO 4: Si fue exitosa la subida, actualizar el usuario con la ruta de la imagen
                        usuario.Avatar = rutaRelativa;
                        await _usuarioRepository.Modificar(usuario);
                    }

                    _logger.LogInformation("Usuario {Email} creado exitosamente.", usuario.Email);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al procesar la creación del usuario.");
                    ModelState.AddModelError("", "Ocurrió un error inesperado durante el registro.");
                }
            }

            return View(usuario);
        }

        // GET: Usuarios/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Usuario usuario, IFormFile? archivoAvatar)
        {
            if (id != usuario.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. Obtener el usuario existente desde la base de datos
                    var usuarioDb = await _usuarioRepository.ObtenerPorId(id);
                    if (usuarioDb == null) return NotFound();

                    // 2. Validar que el email no pertenezca a OTRO usuario diferente
                    var usuarioConMismoEmail = await _usuarioRepository.ObtenerPorEmail(usuario.Email);
                    if (usuarioConMismoEmail != null && usuarioConMismoEmail.Id != id)
                    {
                        ModelState.AddModelError("Email", "El correo electrónico ya está registrado por otro usuario.");
                        return View(usuario);
                    }

                    // 3. Manejo de Contraseña:
                    // Si escribió una nueva contraseña, la hasheamos.
                    // Si la dejó en blanco, conservamos el PasswordHash previo.
                    if (!string.IsNullOrEmpty(usuario.Password))
                    {
                        usuario.PasswordHash = BCrypt.Net.BCrypt.HashPassword(usuario.Password);
                    }
                    else
                    {
                        usuario.PasswordHash = usuarioDb.PasswordHash;
                    }

                    // 4. Manejo del Avatar:
                    if (archivoAvatar != null && archivoAvatar.Length > 0)
                    {
                        // Pasamos el archivo y el email actual al método auxiliar
                        var (esValido, mensajeError, rutaRelativa) = await ProcesarYGuardarAvatarAsync(archivoAvatar, usuario.Email);
                        if (!esValido)
                        {
                            ModelState.AddModelError("Avatar", mensajeError);
                            return View(usuario);
                        }

                        usuario.Avatar = rutaRelativa;
                    }
                    else
                    {
                        // Si no subió una nueva imagen, conservamos la que ya tenía
                        usuario.Avatar = usuarioDb.Avatar;
                    }

                    // Preservamos la fecha original de registro
                    usuario.FechaRegistro = usuarioDb.FechaRegistro;

                    // 5. Guardar los cambios actualizados
                    await _usuarioRepository.Modificar(usuario);
                    _logger.LogInformation("Usuario con ID {Id} actualizado correctamente.", id);

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el usuario con ID {Id}", id);
                    ModelState.AddModelError("", "Ocurrió un error inesperado al actualizar el usuario.");
                }
            }

            return View(usuario);
        }

        // GET: Usuarios/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var usuario = await _usuarioRepository.ObtenerPorId(id);
            if (usuario == null)
            {
                return NotFound();
            }

            return View(usuario);
        }

        // POST: Usuarios/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var usuario = await _usuarioRepository.ObtenerPorId(id);
                if (usuario != null)
                {
                    await _usuarioRepository.Eliminar(usuario);
                    _logger.LogInformation("Usuario con ID {UsuarioId} eliminado.", id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar eliminar el usuario con ID {UsuarioId}.", id);
                return View("Error");
            }
        }

        // GET: Usuarios/Avatar/5
        [HttpGet("Usuarios/Avatar/{id}")]
        public async Task<IActionResult> Avatar(int id)
        {
            // Llamamos a la función dedicada de nuestro repositorio para obtener la ruta
            string? rutaAvatar = await _usuarioRepository.ObtenerAvatarPorId(id);

            // Si existe la ruta en la base de datos, devolvemos esa ruta en formato JSON
            if (!string.IsNullOrEmpty(rutaAvatar))
            {
                return Ok(new { ruta = rutaAvatar });
            }

            // Si no tiene avatar asignado, retornamos una ruta por defecto genérica
            return Ok(new { ruta = "/images/default-avatar.svg" });
        }

        // =========================================================================
        // MÉTODO AUXILIAR REUTILIZABLE PARA VALIDAR Y GUARDAR EL AVATAR
        // =========================================================================
        private async Task<(bool esValido, string mensajeError, string rutaRelativa)> ProcesarYGuardarAvatarAsync(IFormFile archivo, string emailUsuario)
        {
            // 1. Validar Tamaño máximo (2 MB)
            if (archivo.Length > TAMANIO_MAXIMO_AVATAR_BYTES)
            {
                return (false, $"El tamaño de la imagen no debe superar los {TAMANIO_MAXIMO_AVATAR_BYTES / (1024 * 1024)} MB.", string.Empty);
            }

            // 2. Validar Extensión
            var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            if (!EXTENSIONES_PERMITIDAS_AVATAR.Contains(extension))
            {
                return (false, $"Formato no permitido. Solo se aceptan: {string.Join(", ", EXTENSIONES_PERMITIDAS_AVATAR)}", string.Empty);
            }

            // 3. Crear el nombre del archivo basado en el email sanitizado
            string emailSanitizado = emailUsuario.Replace("@", "_at_").Replace(".", "_");
            string nombreArchivo = $"{emailSanitizado}{extension}";

            string carpetaDestino = Path.Combine(_webHostEnvironment.WebRootPath, CARPETA_SUBIDA_AVATAR);

            if (!Directory.Exists(carpetaDestino))
            {
                Directory.CreateDirectory(carpetaDestino);
            }

            string rutaCompleta = Path.Combine(carpetaDestino, nombreArchivo);

            // 4. Guardar archivo físicamente en disco
            using (var stream = new FileStream(rutaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Retornar éxito con la ruta relativa para guardar en BD
            return (true, string.Empty, $"/{CARPETA_SUBIDA_AVATAR}/{nombreArchivo}");
        }
    }
}