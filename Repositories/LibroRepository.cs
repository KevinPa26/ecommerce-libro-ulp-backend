using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class LibroRepository : ILibroRepository
{
    private readonly DataContext _context;

    public LibroRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<Libro?> ObtenerPorId(int id)
    {
        return await _context.Libros
            .Include(l => l.Imagenes) 
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<List<Libro>> ObtenerTodos()
    {
        return await _context.Libros
            .Where(l => !l.Estado.Equals(EstadoLibro.INACTIVO))
            .Include(l => l.Imagenes)
            .ToListAsync();
    }

    public async Task Crear(Libro obj)
    {
        await _context.Libros.AddAsync(obj);
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(Libro obj)
    {
        if (obj != null)
        {
            obj.Estado = EstadoLibro.INACTIVO;
            await _context.SaveChangesAsync();
        }
    }

    /* No deberia ser necesario ya que no estamos trabajando con entidades sin trackear.
     * Cambiar propiedades de entidades trackeadas se hace directo con EF:
     * T entidad = await _context.T.FindAsync(id);
     * entidad.nombre = "otro nombre";
     * await _context.SaveChangesAsync();
     */ 
    public async Task Modificar(Libro obj)
    {
        _context.Libros.Update(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<(List<Libro> Items, int TotalCount)> ObtenerTodos(
        string? titulo, 
        string? autor, 
        string? isbn, 
        int? estado, 
        int pageNumber, 
        int pageSize)
    {
        var query = _context.Libros
            .Include(l => l.Imagenes) // Incluimos las imágenes para las tarjetas
            .AsQueryable();

        // 1. Filtro por Título
        if (!string.IsNullOrWhiteSpace(titulo))
        {
            string term = titulo.ToLower();
            query = query.Where(l => l.Titulo != null && l.Titulo.ToLower().Contains(term));
        }

        // 2. Filtro por Autor
        if (!string.IsNullOrWhiteSpace(autor))
        {
            string term = autor.ToLower();
            query = query.Where(l => l.Autor != null && l.Autor.ToLower().Contains(term));
        }

        // 3. Filtro por ISBN
        if (!string.IsNullOrWhiteSpace(isbn))
        {
            string term = isbn.ToLower();
            query = query.Where(l => l.Isbn13 != null && l.Isbn13.ToLower().Contains(term));
        }

        // 4. Filtro por Estado (Mapeando el entero recibido al Enum EstadoLibro)
        if (estado.HasValue)
        {
            EstadoLibro estadoEnum = (EstadoLibro)estado.Value;
            query = query.Where(l => l.Estado == estadoEnum);
        }

        // 5. Contar el total de registros filtrados antes de la paginación
        int totalCount = await query.CountAsync();

        // 6. Aplicar orden y paginación (Skip y Take)
        var items = await query
            .OrderByDescending(l => l.Id)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        // 7. Retornar la tupla con nombre exacto
        return (items, totalCount);
    }
}
