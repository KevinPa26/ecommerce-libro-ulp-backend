using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ILibroRepository _libroRepository;
        private readonly IImagenLibroRepository _imagenLibroRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ILibroRepository libroRepository, IImagenLibroRepository imagenLibroRepository, IWebHostEnvironment webHostEnvironment, ILogger<LibrosController> logger)
        {
            _libroRepository = libroRepository;
            _imagenLibroRepository = imagenLibroRepository;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<IActionResult> Crear(Libro libro, List<IFormFile> archivosFotos)
        {
            // 1. Obtener el ID del usuario logueado actual desde los claims
            // var usuarioIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            // if (int.TryParse(usuarioIdString, out int vendedorIdLogueado))
            // {
                libro.VendedorId = 1;
                libro.UsuarioCreador = 1;
                libro.Estado = EstadoLibro.DISPONIBLE;
            // }
            // else
            // {
            //     // Si no hay sesión válida, redirigir al login o devolver error de autenticación
            //     return Challenge(); 
            // }

            // Limpiamos la validación del ModelState para VendedorId y UsuarioCreador ya que vienen del sistema
            // ModelState.Remove(nameof(libro.VendedorId));
            // ModelState.Remove(nameof(libro.UsuarioCreador));

            // 2. Validar la cantidad de fotos (Mínimo 1, Máximo 4)
            if (archivosFotos == null || archivosFotos.Count < 1 || archivosFotos.Count > 4)
            {
                ModelState.AddModelError("Fotos", "Debes subir obligatoriamente entre 1 y 4 fotografías del libro.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 3. Guardar el libro primero en el repositorio para que se genere su ID
                    await _libroRepository.Crear(libro); 
                    // Nota: Asegúrate de que tu método CrearAsync en el repositorio asigne el ID al objeto `libro` (o guarde los cambios y devuelva el objeto con el ID poblado).

                    // 4. Procesar y guardar las imágenes asociadas
                    for (int i = 0; i < archivosFotos.Count; i++)
                    {
                        var archivo = archivosFotos[i];
                        if (archivo != null && archivo.Length > 0)
                        {
                            // Llamamos a tu método auxiliar para guardar el archivo en disco (ej: wwwroot/uploads/libros/)
                            // y que te devuelva la ruta relativa (ej: "/uploads/libros/1/foto_abc.jpg")
                            var rutaRelativa = await GuardarArchivoLibroAsync(archivo, libro.Id);

                            var imagenLibro = new ImagenLibro
                            {
                                LibroId = libro.Id,
                                Path = rutaRelativa,
                                // LA MAGIA AQUí: La primera imagen (índice 0) es la principal
                                EsPrincipal = (i == 0) 
                            };

                            // Guardamos la imagen en la base de datos a través de su respectivo repositorio o contexto
                            await _imagenLibroRepository.Crear(imagenLibro);
                        }
                    }

                    _logger.LogInformation("Libro con ID {Id} creado exitosamente con {Count} imágenes.", libro.Id, archivosFotos.Count);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al crear el libro y sus imágenes asociadas.");
                    ModelState.AddModelError("", "Ocurrió un error inesperado al intentar registrar el libro.");
                }
            }

            // Si el ModelState falla o hay excepciones, recargamos la vista con el modelo actual
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

        private async Task<string> GuardarArchivoLibroAsync(IFormFile archivo, int libroId)
        {
            // Definimos la carpeta destino organizada por ID de libro dentro de wwwroot/uploads/libros
            string carpetaUploads = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", "libros", libroId.ToString());

            if (!Directory.Exists(carpetaUploads))
            {
                Directory.CreateDirectory(carpetaUploads);
            }

            // Generamos un nombre único para evitar colisiones
            string extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();
            string nombreUnico = $"{Guid.NewGuid()}{extension}";
            string rutaFisicaCompleta = Path.Combine(carpetaUploads, nombreUnico);

            using (var stream = new FileStream(rutaFisicaCompleta, FileMode.Create))
            {
                await archivo.CopyToAsync(stream);
            }

            // Retornamos la ruta relativa para guardarla en la base de datos
            return $"/uploads/libros/{libroId}/{nombreUnico}";
        }
    }
}