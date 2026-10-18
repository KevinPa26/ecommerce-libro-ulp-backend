using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class CarritosController : Controller
    {
        private readonly ICarritoRepository _carritoRepository;
        private readonly ILogger<CarritosController> _logger;

        public CarritosController(ICarritoRepository carritoRepository, ILogger<CarritosController> logger)
        {
            _carritoRepository = carritoRepository;
            _logger = logger;
        }

        // GET: Carritos
        public async Task<IActionResult> Index(int? usuarioId)
        {
            try
            {
                List<Carrito> carritos;

                // Si recibimos un ID de usuario por parámetro, filtramos sus carritos; si no, traemos todos.
                if (usuarioId.HasValue)
                {
                    carritos = await _carritoRepository.ObtenerPorUsuarioId(usuarioId.Value);
                }
                else
                {
                    carritos = await _carritoRepository.ObtenerTodos();
                }

                return View(carritos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de carritos.");
                return View("Error");
            }
        }

        // GET: Carritos/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var carrito = await _carritoRepository.ObtenerPorId(id);
                if (carrito == null)
                {
                    _logger.LogWarning("No se encontró el carrito con ID {CarritoId}", id);
                    return NotFound();
                }

                return View(carrito);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del carrito con ID {CarritoId}", id);
                return View("Error");
            }
        }

        // GET: Carritos/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Carritos/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Carrito carrito)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _carritoRepository.Crear(carrito);
                    _logger.LogInformation("Carrito creado exitosamente para el usuario con ID {UsuarioId}.", carrito.UsuarioId);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al intentar crear un nuevo carrito.");
                    ModelState.AddModelError("", "No se pudo guardar el carrito. Inténtelo nuevamente.");
                }
            }

            return View(carrito);
        }

        // GET: Carritos/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var carrito = await _carritoRepository.ObtenerPorId(id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Carrito carrito)
        {
            if (id != carrito.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _carritoRepository.Modificar(carrito);
                    _logger.LogInformation("Carrito con ID {CarritoId} modificado correctamente.", carrito.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al modificar el carrito con ID {CarritoId}.", carrito.Id);
                    ModelState.AddModelError("", "No se pudo actualizar el registro.");
                }
            }

            return View(carrito);
        }

        // GET: Carritos/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var carrito = await _carritoRepository.ObtenerPorId(id);
            if (carrito == null)
            {
                return NotFound();
            }

            return View(carrito);
        }

        // POST: Carritos/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var carrito = await _carritoRepository.ObtenerPorId(id);
                if (carrito != null)
                {
                    await _carritoRepository.Eliminar(carrito);
                    _logger.LogInformation("Carrito con ID {CarritoId} eliminado.", id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el carrito con ID {CarritoId}.", id);
                return View("Error");
            }
        }
    }
}