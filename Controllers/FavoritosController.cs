using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.AspNetCore.Authorization;

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
        public async Task<IActionResult> Index(int? usuarioId)
        {
            try
            {
                List<Favorito> favoritos;

                // Si se proporciona un ID de usuario por parámetro, filtramos sus favoritos; si no, traemos todos.
                if (usuarioId.HasValue)
                {
                    favoritos = await _favoritoRepository.ObtenerPorUsuarioId(usuarioId.Value);
                }
                else
                {
                    favoritos = await _favoritoRepository.ObtenerTodos();
                }

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
            if (ModelState.IsValid)
            {
                try
                {
                    await _favoritoRepository.Crear(favorito);
                    _logger.LogInformation("Favorito creado exitosamente para el usuario ID {UsuarioId} y libro ID {LibroId}.", favorito.UsuarioId, favorito.LibroId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al intentar crear un nuevo favorito.");
                    ModelState.AddModelError("", "No se pudo guardar el favorito. Inténtelo nuevamente.");
                }
            }

            return View(favorito);
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
    }
}