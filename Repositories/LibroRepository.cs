using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class LibroRepository : ILibroRepository
{
    private readonly AppDbContext _context;

    public LibroRepository(AppDbContext context)
    {
        _context = context;
    }

    public Libro? ObtenerPorId(int id)
    {
        return _context.Libros.Find(id);
    }

    public List<Libro> ObtenerTodos()
    {
        return _context.Libros.ToList();
    }

    public void Crear(Libro obj)
    {
        _context.Libros.Add(obj);
        _context.SaveChanges();
    }

    public void Eliminar(Libro obj)
    {
        _context.Libros.Remove(obj);
        _context.SaveChanges();
    }

    public List<Libro> ObtenerPorISBN(string isbn)
    {
        return _context.Libros
            .Where(l => l.Isbn13 == isbn)
            .ToList();
    }

    public List<Libro> ObtenerPorNombre(string nombre)
    {
        return _context.Libros
            .Where(l => l.Titulo.Contains(nombre))
            .ToList();
    }

    public List<Libro> ObtenerPorAutor(string autor)
    {
        return _context.Libros
            .Where(l => l.Autor.Contains(autor))
            .ToList();
    }

    public List<Libro> ObtenerPorVendedor(string vendedor)
    {
        if (int.TryParse(vendedor, out int vendedorId))
        {
            return _context.Libros
                .Where(l => l.VendedorId == vendedorId)
                .ToList();
        }

        return new List<Libro>();
    }

    public List<Libro> ObtenerPorRangoDePrecios(decimal min, decimal max)
    {
        return _context.Libros
            .Where(l => l.Precio >= min && l.Precio <= max)
            .ToList();
    }
}
