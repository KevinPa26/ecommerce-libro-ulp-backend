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
    }
}