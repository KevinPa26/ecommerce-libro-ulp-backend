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
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Listar(
            string? titulo,
            string? autor,
            string? isbn,
            int? estado,
            int page = 1)
        {
            try
            {
                const int pageSize = 8;

                if (page < 1)
                    page = 1;

                
                // Obtener libros filtrados y paginados.
                var resultado = await _libroRepository.ObtenerTodos(
                    titulo,
                    autor,
                    isbn,
                    estado,
                    page,
                    pageSize
                );

                // Convertimos las entidades a datos para Vue.
                var libros = resultado.Items.Select(libro =>
                {
                    var imagenPrincipal = libro.Imagenes?
                        .FirstOrDefault(img => img.EsPrincipal)
                        ?? libro.Imagenes?.FirstOrDefault();

                    return new
                    {
                        id = libro.Id,
                        titulo = libro.Titulo,
                        autor = libro.Autor,
                        isbn13 = libro.Isbn13,
                        precio = libro.Precio,
                        stock = libro.Stock,
                        estado = (int)libro.Estado,

                        imagen = imagenPrincipal?.Path ?? "/images/book.svg"
                    };
                }).ToList();

                int totalPages = (int)Math.Ceiling(
                    (double)resultado.TotalCount / pageSize
                );

                return Ok(new
                {
                    success = true,
                    items = libros,
                    totalCount = resultado.TotalCount,
                    currentPage = page,
                    totalPages,
                    pageSize,
                    esAdmin = User.IsInRole("ADMIN")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar libros.");

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al cargar el catálogo."
                });
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
        public async Task<IActionResult> Crear(
            Libro libro,
            List<IFormFile>? archivosFotos)
        {
            try
            {
                // 1. Obtener el usuario autenticado.
                var usuarioIdClaim = User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier
                )?.Value;

                if (!int.TryParse(usuarioIdClaim, out int usuarioId))
                {
                    return Unauthorized(new
                    {
                        success = false,
                        message = "Debes iniciar sesión para registrar un libro."
                    });
                }

                // 2. Asignar datos que controla el backend.
                libro.VendedorId = usuarioId;
                libro.UsuarioCreador = usuarioId;
                libro.Estado = EstadoLibro.DISPONIBLE;

                // 3. Validar cantidad de fotografías.
                archivosFotos = (archivosFotos ?? new List<IFormFile>())
                    .Where(a => a != null && a.Length > 0)
                    .ToList();

                // Los archivos se validan manualmente, no mediante ModelState.
                foreach (var key in ModelState.Keys
                    .Where(k => k.StartsWith("archivosFotos",
                        StringComparison.OrdinalIgnoreCase))
                    .ToList())
                {
                    ModelState.Remove(key);
                }

                if (archivosFotos.Count < 1)
                {
                    ModelState.AddModelError(
                        "Fotos",
                        "Debes seleccionar al menos una fotografía."
                    );
                }
                else if (archivosFotos.Count > 4)
                {
                    ModelState.AddModelError(
                        "Fotos",
                        "Solo puedes seleccionar un máximo de 4 fotografías."
                    );
                }

                // 4. Validar el modelo en el servidor.
                TryValidateModel(libro);

                if (!ModelState.IsValid)
                {
                    var errores = ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors)
                        .Select(e => !string.IsNullOrWhiteSpace(e.ErrorMessage)
                            ? e.ErrorMessage
                            : "Uno de los datos ingresados no es válido.")
                        .Distinct()
                        .ToList();

                    return BadRequest(new
                    {
                        success = false,
                        message = string.Join(" ", errores),
                        errors = errores
                    });
                }

                // 5. Registrar el libro.
                await _libroRepository.Crear(libro);

                // 6. Guardar fotografías.
                for (int i = 0; i < archivosFotos.Count; i++)
                {
                    var archivo = archivosFotos[i];

                    var ruta = await GuardarArchivoLibroAsync(
                        archivo,
                        libro.Id
                    );

                    var imagen = new ImagenLibro
                    {
                        LibroId = libro.Id,
                        Path = ruta,
                        EsPrincipal = i == 0
                    };

                    await _imagenLibroRepository.Crear(imagen);
                }

                _logger.LogInformation(
                    "Libro {LibroId} registrado correctamente por el usuario {UsuarioId}.",
                    libro.Id,
                    usuarioId
                );

                // 7. Respuesta JSON.
                return Ok(new
                {
                    success = true,
                    message = "El libro se registró correctamente.",
                    libroId = libro.Id
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al registrar el libro."
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al registrar el libro. Inténtalo nuevamente."
                });
            }
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
        public async Task<IActionResult> Editar(
            int id,
            Libro libroConCambios,
            List<IFormFile>? archivosFotos,
            List<string>? imagenesAEliminar)
        {
            // 1. Validar coincidencia de IDs.
            if (id != libroConCambios.Id)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "El identificador del libro no es válido."
                });
            }

            // 2. Obtener el usuario logueado.
            var usuarioIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(usuarioIdString, out int vendedorIdLogueado))
            {
                return Unauthorized(new
                {
                    success = false,
                    message = "Debes iniciar sesión para realizar esta operación."
                });
            }

            // 3. Validar permisos.
            if (!User.IsInRole("ADMIN"))
            {
                return StatusCode(403, new
                {
                    success = false,
                    message = "No tienes permisos para editar este libro."
                });
            }

            // 4. Recuperar el libro original con sus imágenes.
            var libroOriginal = await _libroRepository.ObtenerPorId(id);

            if (libroOriginal == null)
            {
                return NotFound(new
                {
                    success = false,
                    message = "No se encontró el libro solicitado."
                });
            }
            
            // 5. Obtener imágenes actuales.
            var imagenesActualesBD = libroOriginal.Imagenes
                ?? new List<ImagenLibro>();

            // Normalizar archivos: descartar entradas vacías.
            archivosFotos = (archivosFotos ?? new List<IFormFile>())
                .Where(a => a != null && a.Length > 0)
                .ToList();

            // Convertir los IDs recibidos, ignorando valores vacíos o inválidos.
            var idsSolicitados = (imagenesAEliminar ?? new List<string>())
                .Where(valor => !string.IsNullOrWhiteSpace(valor))
                .Select(valor => int.TryParse(valor, out int imgId) ? imgId : 0)
                .Where(imgId => imgId > 0)
                .Distinct()
                .ToList();

            // Quitar errores de conversión de estos parámetros.
            // Su contenido ya se normalizó y se valida manualmente.
            foreach (var key in ModelState.Keys
                .Where(k =>
                    k.StartsWith("archivosFotos", StringComparison.OrdinalIgnoreCase) ||
                    k.StartsWith("imagenesAEliminar", StringComparison.OrdinalIgnoreCase))
                .ToList())
            {
                ModelState.Remove(key);
            }

            // Solo se pueden eliminar imágenes pertenecientes al libro.
            var idsAEliminarValidos = imagenesActualesBD
                .Where(img => idsSolicitados.Contains(img.Id))
                .Select(img => img.Id)
                .Distinct()
                .ToList();

            // Calcular el total final.
            int cantidadDespuesDeEliminar =
                imagenesActualesBD.Count - idsAEliminarValidos.Count;

            int cantidadTotalFinal =
                cantidadDespuesDeEliminar + archivosFotos.Count;

            // Validar regla de negocio.
            if (cantidadTotalFinal < 1 || cantidadTotalFinal > 4)
            {
                ModelState.AddModelError(
                    "Fotos",
                    "El libro debe tener obligatoriamente entre 1 y 4 fotografías en total."
                );
            }


            // 7. Validar modelo.
            if (!ModelState.IsValid)
            {
                var errores = ModelState
                    .Where(e => e.Value?.Errors.Count > 0)
                    .ToDictionary(
                        e => e.Key,
                        e => e.Value!.Errors.Select(error =>
                            !string.IsNullOrWhiteSpace(error.ErrorMessage)
                                ? error.ErrorMessage
                                : "El valor ingresado no es válido."
                        ).ToArray()
                    );

                return BadRequest(new
                {
                    success = false,
                    message = string.Join(" ",
                        errores.SelectMany(e => e.Value)),
                    errors = errores
                });
            }

            try
            {
                // 8. Actualizar solamente los campos permitidos.
                libroOriginal.Precio = libroConCambios.Precio;
                libroOriginal.Stock = libroConCambios.Stock;
                libroOriginal.Estado = libroConCambios.Estado;
                libroOriginal.DescripcionVendedor =
                    libroConCambios.DescripcionVendedor;
                libroOriginal.DescripcionLibro =
                    libroConCambios.DescripcionLibro;

                await _libroRepository.Modificar(libroOriginal);

                // 9. Eliminar imágenes seleccionadas.
                foreach (var imgId in idsAEliminarValidos)
                {
                    var imgAEliminar =
                        await _imagenLibroRepository.ObtenerPorId(imgId);

                    if (imgAEliminar != null && imgAEliminar.LibroId == id)
                    {
                        BorrarArchivoFisico(imgAEliminar.Path);

                        await _imagenLibroRepository.Eliminar(imgAEliminar);
                    }
                }

                // 10. Agregar nuevas fotografías.
                if (archivosFotos.Any())
                {
                    var imagenesRestantes =
                        await _imagenLibroRepository.ObtenerPorLibro(id);

                    bool yaTienePrincipal =
                        imagenesRestantes.Any(img => img.EsPrincipal);

                    for (int i = 0; i < archivosFotos.Count; i++)
                    {
                        var archivo = archivosFotos[i];

                        if (archivo != null && archivo.Length > 0)
                        {
                            var rutaRelativa =
                                await GuardarArchivoLibroAsync(archivo, id);

                            var nuevaImagen = new ImagenLibro
                            {
                                LibroId = id,
                                Path = rutaRelativa,
                                EsPrincipal = !yaTienePrincipal && i == 0
                            };

                            await _imagenLibroRepository.Crear(nuevaImagen);

                            // Si esta imagen pasó a ser principal,
                            // las siguientes ya no deben recibir ese estado.
                            if (nuevaImagen.EsPrincipal)
                            {
                                yaTienePrincipal = true;
                            }
                        }
                    }
                }

                // 11. Recuperar imágenes actualizadas.
                var imagenesActualizadas =
                    await _imagenLibroRepository.ObtenerPorLibro(id);

                _logger.LogInformation(
                    "Libro con ID {Id} actualizado exitosamente por el usuario {UsuarioId}.",
                    id,
                    vendedorIdLogueado
                );

                // 12. Respuesta JSON.
                return Ok(new
                {
                    success = true,
                    message = "El libro fue actualizado correctamente.",
                    libroId = id,
                    imagenes = imagenesActualizadas.Select(img => new
                    {
                        img.Id,
                        img.Path,
                        img.EsPrincipal
                    }).ToList()
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al actualizar el libro con ID {Id}.",
                    id
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error inesperado al intentar guardar los cambios."
                });
            }
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

                if (libro == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = "No se encontró el libro que desea dar de baja."
                    });
                }

                await _libroRepository.Eliminar(libro);

                _logger.LogInformation(
                    "Libro con ID {LibroId} dado de baja correctamente.",
                    id
                );

                TempData["Mensaje"] = "El libro fue dado de baja correctamente.";
                TempData["TipoMensaje"] = "success";

                return Ok(new
                {
                    success = true,
                    redirectUrl = Url.Action("Index", "Libros")
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error al dar de baja el libro con ID {LibroId}.",
                    id
                );

                return StatusCode(500, new
                {
                    success = false,
                    message = "Ocurrió un error al intentar dar de baja el libro."
                });
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