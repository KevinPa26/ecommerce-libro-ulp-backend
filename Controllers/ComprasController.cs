using System.Security.Claims;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace ecommerce_libro_ulp_backend.Controllers
{
    public class ComprasController : Controller
    {
        private readonly ICompraRepository _compraRepository;

        public ComprasController(ICompraRepository compraRepository)
        {
            _compraRepository = compraRepository;
        }

        public async Task<IActionResult> Index(DateOnly? fechaExacta, DateOnly? desde, DateOnly? hasta, int page = 1, int pageSize = 10)
        {
            int? usuarioId = null; 

            var (compras, totalCount) = await _compraRepository.ObtenerTodos(
                fechaExacta, desde, hasta, page, pageSize, usuarioId
            );

            int totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            ViewBag.FechaExacta = fechaExacta?.ToString("yyyy-MM-dd");
            ViewBag.Desde = desde?.ToString("yyyy-MM-dd");
            ViewBag.Hasta = hasta?.ToString("yyyy-MM-dd");
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages == 0 ? 1 : totalPages;
            ViewBag.PageSize = pageSize;

            return View(compras);
        }

        // GET: Compras/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Uso del repositorio en lugar del contexto de base de datos
            var compra = await _compraRepository.ObtenerPorId(id.Value);
            
            if (compra == null)
            {
                return NotFound();
            }

            return View(compra);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear([FromBody] List<DetalleCompra> detalles)
        {
            if (detalles == null || !detalles.Any())
            {
                TempData["ErrorMessage"] = "El carrito está vacío.";
                return RedirectToAction("Index", "Libros");
            }

            // 1. Obtener el ID del usuario actual de manera segura desde los Claims
            // var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier); // O ClaimTypes.Name según tu configuración de Identity
            
            // if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int usuarioId))
            // {
            //     return Unauthorized();
            // }

            // 2. Instanciar la Compra y asignar los datos de cabecera
            var compra = new Compra
            {
                UsuarioId = 19,
                FechaCompra = DateOnly.FromDateTime(DateTime.Now),
                Detalles = detalles
            };

            try
            {
                // 3. Llamar al repositorio transaccional
                await _compraRepository.RegistrarCompra(compra);

                TempData["SuccessMessage"] = "¡Compra registrada con éxito!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"No se pudo procesar la compra: {ex.Message}";
                return RedirectToAction("Index", "Libros");
            }
        }
    }
}