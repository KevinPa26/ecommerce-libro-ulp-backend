using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class UsuariosController : Controller
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly ILogger<UsuariosController> _logger;

        public UsuariosController(IUsuarioRepository usuarioRepository, ILogger<UsuariosController> logger)
        {
            _usuarioRepository = usuarioRepository;
            _logger = logger;
        }

        // GET: Usuarios
        public async Task<IActionResult> Index()
        {
            try
            {
                var usuarios = await _usuarioRepository.ObtenerTodos();
                return View(usuarios);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de usuarios.");
                return View("Error");
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
        public async Task<IActionResult> Crear(Usuario usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _usuarioRepository.Crear(usuario);
                    _logger.LogInformation("Usuario {Email} creado exitosamente.", usuario.Email);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al intentar crear un usuario con email {Email}.", usuario.Email);
                    ModelState.AddModelError("", "No se pudo guardar el usuario. Inténtelo nuevamente.");
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
        public async Task<IActionResult> Editar(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _usuarioRepository.Modificar(usuario);
                    _logger.LogInformation("Usuario con ID {UsuarioId} actualizado correctamente.", usuario.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el usuario con ID {UsuarioId}.", usuario.Id);
                    ModelState.AddModelError("", "No se pudo actualizar el registro.");
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
    }
}