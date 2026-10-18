using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ILibroRepository _libroRepository;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ILibroRepository libroRepository, ILogger<LibrosController> logger)
        {
            _libroRepository = libroRepository;
            _logger = logger;
        }

        // GET: Libros
        public async Task<IActionResult> Index(string buscarPor, string termino)
        {
            try
            {
                List<Libro> libros;

                // Aprovechamos los filtros de tu repositorio si el usuario realiza una búsqueda
                if (!string.IsNullOrEmpty(termino))
                {
                    libros = buscarPor switch
                    {
                        "Autor" => await _libroRepository.ObtenerPorAutor(termino),
                        "ISBN" => await _libroRepository.ObtenerPorISBN(termino),
                        _ => await _libroRepository.ObtenerPorNombre(termino), // Por defecto busca por título/nombre
                    };
                }
                else
                {
                    libros = await _libroRepository.ObtenerTodos();
                }

                return View(libros);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener el listado de libros.");
                return View("Error");
            }
        }

        // GET: Libros/Details/5
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var libro = await _libroRepository.ObtenerPorId(id);
                if (libro == null)
                {
                    _logger.LogWarning("No se encontró el libro con ID {LibroId}", id);
                    return NotFound();
                }

                return View(libro);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener los detalles del libro con ID {LibroId}", id);
                return View("Error");
            }
        }

        // GET: Libros/Crear
        public IActionResult Crear()
        {
            return View();
        }

        // POST: Libros/Crear
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(Libro libro)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _libroRepository.Crear(libro);
                    _logger.LogInformation("Libro '{Titulo}' creado exitosamente.", libro.Titulo);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al intentar crear un nuevo libro.");
                    ModelState.AddModelError("", "No se pudo guardar el libro. Inténtelo nuevamente.");
                }
            }

            return View(libro);
        }

        // GET: Libros/Editar/5
        public async Task<IActionResult> Editar(int id)
        {
            var libro = await _libroRepository.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(int id, Libro libro)
        {
            if (id != libro.Id)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _libroRepository.Modificar(libro);
                    _logger.LogInformation("Libro con ID {LibroId} modificado correctamente.", libro.Id);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al modificar el libro con ID {LibroId}.", libro.Id);
                    ModelState.AddModelError("", "No se pudo actualizar el registro.");
                }
            }

            return View(libro);
        }

        // GET: Libros/Eliminar/5
        public async Task<IActionResult> Eliminar(int id)
        {
            var libro = await _libroRepository.ObtenerPorId(id);
            if (libro == null)
            {
                return NotFound();
            }

            return View(libro);
        }

        // POST: Libros/Eliminar/5
        [HttpPost, ActionName("Eliminar")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarConfirmado(int id)
        {
            try
            {
                var libro = await _libroRepository.ObtenerPorId(id);
                if (libro != null)
                {
                    await _libroRepository.Eliminar(libro);
                    _logger.LogInformation("Libro con ID {LibroId} eliminado.", id);
                }

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar el libro con ID {LibroId}.", id);
                return View("Error");
            }
        }
    }
}