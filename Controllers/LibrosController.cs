using Microsoft.AspNetCore.Mvc;
using ecommerce_libro_ulp_backend.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class LibrosController : Controller
    {
        private readonly ILibroRepository _libroRepository;
        private readonly IImagenLibroRepository _imagenLibroRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly IFavoritoRepository _favoritoRepository;
        private readonly ILogger<LibrosController> _logger;

        public LibrosController(ILibroRepository libroRepository, IImagenLibroRepository imagenLibroRepository, IFavoritoRepository favoritoRepository, IWebHostEnvironment webHostEnvironment, ILogger<LibrosController> logger)
        {
            _libroRepository = libroRepository;
            _imagenLibroRepository = imagenLibroRepository;
            _webHostEnvironment = webHostEnvironment;
            _favoritoRepository = favoritoRepository;
            _logger = logger;
        }

        // GET: Libros
        public async Task<IActionResult> Index(string? titulo, string? autor, string? isbn, int? estado, int page = 1)
        {
            int pageSizeReal = 8; // Múltiplo de 4 para que encaje perfecto en la grilla de tarjetas

            if (!User.IsInRole("ADMIN"))
            {
                estado = 0; 
            }

            var resultado = await _libroRepository.ObtenerTodos(
                titulo, autor, isbn, estado, page, pageSizeReal
            );

            int totalPages = (int)Math.Ceiling(resultado.TotalCount / (double)pageSizeReal);

            ViewBag.Titulo = titulo;
            ViewBag.Autor = autor;
            ViewBag.Isbn = isbn;
            ViewBag.Estado = estado;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages == 0 ? 1 : totalPages;
            ViewBag.TotalRegistros = resultado.TotalCount;

            return View(resultado.Items);
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

                bool esFavorito = false;

                if (User.Identity!.IsAuthenticated)
                {
                    var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    if (int.TryParse(userIdClaim, out int usuarioId))
                    {
                        esFavorito = await _favoritoRepository.isFavorito(usuarioId, libro.Id);
                    }
                }

                ViewBag.EsFavorito = esFavorito;

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
            var estaAutenticado = User.Identity?.IsAuthenticated;
            var tipoAutenticacion = User.Identity?.AuthenticationType;
            var usuarioIdString = User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );
            
            if (int.TryParse(usuarioIdString, out int vendedorIdLogueado))
            {
                libro.VendedorId = vendedorIdLogueado;
                libro.UsuarioCreador = vendedorIdLogueado;
                libro.Estado = EstadoLibro.DISPONIBLE;
            }
            else
            {
                // Si no hay sesión válida, redirigir al login o devolver error de autenticación
                return Challenge(); 
            }

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
                    for (int i = 0; i < archivosFotos!.Count; i++) // ! -> ModelState.IsValid daria falso si archivosFotos fuese nulo, ver paso (2)
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
        public async Task<IActionResult> Editar(int id, Libro libroConCambios, List<IFormFile> archivosFotos, List<int> imagenesAEliminar)
        {
            if (id != libroConCambios.Id)
            {
                return NotFound();
            }

            // 1. Obtener el ID del usuario logueado actual desde los claims (por seguridad)
            var usuarioIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (!int.TryParse(usuarioIdString, out int vendedorIdLogueado))
            {
                return Challenge();
            }

            // 2. RECUPERAR EL LIBRO ORIGINAL DE LA BASE DE DATOS
            // Esto es vital para asegurar que ISBN, Título y Autor NO se alteren maliciosamente
            var libroOriginal = await _libroRepository.ObtenerPorId(id); // Asegúrate de incluir las imágenes en esta consulta
            if (libroOriginal == null)
            {
                return NotFound();
            }

            if (!User.IsInRole("ADMIN")) return Forbid();

            // 3. OBTENER LAS IMÁGENES ACTUALES QUE QUEDAN EN LA BASE DE DATOS
            var imagenesActualesBD = libroOriginal.Imagenes;
            
            // Calcular cuántas imágenes quedarán tras las eliminaciones solicitadas
            int cantidadDespuesDeEliminar = imagenesActualesBD.Count - (imagenesAEliminar?.Count ?? 0);
            int cantidadTotalFinal = cantidadDespuesDeEliminar + (archivosFotos?.Count ?? 0);

            // Validar regla de negocio: Debe tener entre 1 y 4 imágenes en total
            if (cantidadTotalFinal < 1 || cantidadTotalFinal > 4)
            {
                ModelState.AddModelError("", "El libro debe tener obligatoriamente entre 1 y 4 fotografías en total.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // 4. ACTUALIZAR SOLAMENTE LOS CAMPOS PERMITIDOS EN EL OBJETO ORIGINAL
                    libroOriginal.Precio = libroConCambios.Precio;
                    libroOriginal.Stock = libroConCambios.Stock;
                    libroOriginal.Estado = libroConCambios.Estado;
                    libroOriginal.DescripcionVendedor = libroConCambios.DescripcionVendedor;
                    libroOriginal.DescripcionLibro = libroConCambios.DescripcionLibro;

                    // Guardar cambios principales en el repositorio
                    await _libroRepository.Modificar(libroOriginal);

                    // 5. PROCESAR ELIMINACIÓN DE IMÁGENES SELECCIONADAS
                    if (imagenesAEliminar != null && imagenesAEliminar.Any())
                    {
                        foreach (var imgId in imagenesAEliminar)
                        {
                            var imgAEliminar = await _imagenLibroRepository.ObtenerPorId(imgId);
                            if (imgAEliminar != null && imgAEliminar.LibroId == id)
                            {
                                // Opcional: Borrar el archivo físico del disco si usas almacenamiento local
                                BorrarArchivoFisico(imgAEliminar.Path);

                                await _imagenLibroRepository.Eliminar(imgAEliminar);
                            }
                        }
                    }

                    // 6. PROCESAR Y AGREGAR LAS NUEVAS FOTOGRAFÍAS (SI LAS HAY)
                    if (archivosFotos != null && archivosFotos.Any())
                    {
                        // Volvemos a consultar cuántas imágenes activas quedaron tras el borrado
                        var imagenesRestantes = await _imagenLibroRepository.ObtenerPorLibro(id);
                        bool yaTienePrincipal = imagenesRestantes.Any(img => img.EsPrincipal);

                        for (int i = 0; i < archivosFotos.Count; i++)
                        {
                            var archivo = archivosFotos[i];
                            if (archivo != null && archivo.Length > 0)
                            {
                                var rutaRelativa = await GuardarArchivoLibroAsync(archivo, id);

                                var nuevaImagen = new ImagenLibro
                                {
                                    LibroId = id,
                                    Path = rutaRelativa,
                                    // Si ya no quedaban imágenes principales tras los borrados, la primera nueva pasa a ser principal
                                    EsPrincipal = !yaTienePrincipal && (i == 0)
                                };

                                await _imagenLibroRepository.Crear(nuevaImagen);
                            }
                        }
                    }

                    _logger.LogInformation("Libro con ID {Id} actualizado exitosamente.", id);
                    TempData["SuccessMessage"] = "¡Cambios guardados con éxito!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error al actualizar el libro con ID {Id}.", id);
                    ModelState.AddModelError("", "Ocurrió un error inesperado al intentar guardar los cambios.");
                }
            }

            // Si falla el ModelState, reasignamos el modelo original completo a la vista para no perder datos de renderizado
            return View(libroOriginal);
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

        private void BorrarArchivoFisico(string rutaRelativa)
        {
            if (string.IsNullOrEmpty(rutaRelativa)) return;

            // Convertimos la ruta web relativa (ej: /uploads/libros/5/abc.jpg) a ruta física del servidor
            string rutaFisica = Path.Combine(_webHostEnvironment.WebRootPath, rutaRelativa.TrimStart('/'));

            if (System.IO.File.Exists(rutaFisica))
            {
                try
                {
                    System.IO.File.Delete(rutaFisica);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "No se pudo eliminar el archivo físico en la ruta: {Ruta}", rutaFisica);
                }
            }
        }
    }
}