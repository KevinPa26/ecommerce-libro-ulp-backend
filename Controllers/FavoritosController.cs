using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ecommerce_libro_ulp_backend.Controllers
{
    [Authorize(Roles = "USUARIO")]
    public class FavoritosController : Controller
    {
        private readonly IFavoritoRepository _favoritoRepository;
        private readonly ILogger<FavoritosController> _logger;

        public FavoritosController(IFavoritoRepository favoritoRepository, ILogger<FavoritosController> logger)
        {
            _favoritoRepository = favoritoRepository;
            _logger = logger;
        }

        // GET: Favoritos
        public async Task<IActionResult> Index()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var usuarioId))
            {
                return Unauthorized();
            }
            
            try
            {
                List<Favorito> favoritos = await _favoritoRepository.ObtenerPorUsuarioId(usuarioId);
                return View(favoritos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de favoritos.");
                return View("Error");
            }
        }

        // GET: Favoritos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var favorito = await _favoritoRepository.ObtenerPorId(id);
                if (favorito == null)
                {
                    _logger.LogWarning("No se encontró el favorito con ID {FavoritoId}", id);
                    return NotFound();
                }

                return View(favorito);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del favorito con ID {FavoritoId}", id);
                return View("Error");
            }
        }

        // GET: Favoritos/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Favoritos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Favorito favorito)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Datos inválidos para agregar a favoritos." });
            }

            try
            {
                Favorito? existe = await _favoritoRepository.ObtenerPorId(favorito.UsuarioId, favorito.LibroId);
                if (existe != null)
                {
                    return BadRequest(new { success = false, message = "El libro ya está en tus favoritos." });
                }

                await _favoritoRepository.Crear(favorito);
                _logger.LogInformation("Favorito creado exitosamente para el usuario ID {UsuarioId} y libro ID {LibroId}.", favorito.UsuarioId, favorito.LibroId);
                
                return Ok(new { success = true, message = "Agregado a tus favoritos." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al intentar crear un nuevo favorito.");
                return StatusCode(500, new { success = false, message = "No se pudo guardar el favorito. Inténtelo nuevamente." });
            }
        }

        // GET: Favoritos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var favorito = await _favoritoRepository.ObtenerPorId(id);
            if (favorito == null)
            {
                return NotFound();
            }

            return View(favorito);
        }

        // POST: Favoritos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Favorito favorito)
        {
            if (id != favorito.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _favoritoRepository.Modificar(favorito);
                    _logger.LogInformation("Favorito con ID {FavoritoId} modificado correctamente.", favorito.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al modificar el favorito con ID {FavoritoId}.", favorito.Id);
                    ModelState.AddModelError("", "No se pudo actualizar el registro.");
                }
            }

            return View(favorito);
        }

        // GET: Favoritos/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var favorito = await _favoritoRepository.ObtenerPorId(id);
            if (favorito == null)
            {
                return NotFound();
            }

            return View(favorito);
        }

        /*
        // POST: Favoritos/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var favorito = await _favoritoRepository.ObtenerPorId(id);
                if (favorito != null)
                {
                    await _favoritoRepository.Eliminar(favorito);
                    _logger.LogInformation("Favorito con ID {FavoritoId} eliminado.", id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el favorito con ID {FavoritoId}.", id);
                return View("Error");
            }
        }
        */

        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(Favorito favorito)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { success = false, message = "Datos inválidos para eliminar el favorito." });
            }

            try
            {
                Favorito? bdFavorito = await _favoritoRepository.ObtenerPorId(favorito.UsuarioId, favorito.LibroId);
                if (bdFavorito != null)
                {
                    await _favoritoRepository.Eliminar(bdFavorito);
                    _logger.LogInformation("Favorito con ID {FavoritoId} eliminado.", bdFavorito.Id);
                }

                return Ok(new { success = true, message = "Eliminado de tus favoritos." });                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el favorito con IDs {UsuarioId}, {LibroId}.", favorito.UsuarioId, favorito.LibroId);
                return StatusCode(500, new { success = false, message = "Error interno del servidor." });
            }
        }
    }
}