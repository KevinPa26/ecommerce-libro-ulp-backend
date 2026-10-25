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

    public async Task<List<Libro>> ObtenerPorISBN(string isbn)
    {
        return await _context.Libros
            .Where(l => !l.Estado.Equals(EstadoLibro.INACTIVO) && l.Isbn13 == isbn)
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorNombre(string nombre)
    {
        return await _context.Libros
            .Where(l => !l.Estado.Equals(EstadoLibro.INACTIVO) && l.Titulo.Contains(nombre))
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorAutor(string autor)
    {
        return await _context.Libros
            .Where(l => !l.Estado.Equals(EstadoLibro.INACTIVO) && l.Autor.Contains(autor))
            .ToListAsync();
    }

    public async Task<List<Libro>> ObtenerPorVendedor(string vendedor)
    {
        if (int.TryParse(vendedor, out int vendedorId))
        {
            return await _context.Libros
                .Where(l => !l.Estado.Equals(EstadoLibro.INACTIVO) && l.VendedorId == vendedorId)
                .ToListAsync();
        }

        return new List<Libro>();
    }

    public async Task<List<Libro>> ObtenerPorRangoDePrecios(decimal min, decimal max)
    {
        return await _context.Libros
            .Where(l => !l.Estado.Equals(EstadoLibro.INACTIVO) && l.Precio >= min && l.Precio <= max)
            .ToListAsync();
    }
}
