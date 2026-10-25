using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class DetalleCompraRepository : IDetalleCompraRepository
{
    private readonly DataContext _context;

    public DetalleCompraRepository(DataContext context)
    {
        _context = context;
    }

    public async Task<DetalleCompra?> ObtenerPorId(int id)
    {
        return await _context.DetallesCompra.FindAsync(id);
    }

    public async Task<List<DetalleCompra>> ObtenerTodos()
    {
        return await _context.DetallesCompra.ToListAsync();
    }

    public async Task Crear(DetalleCompra obj)
    {
        await _context.DetallesCompra.AddAsync(obj);
        await _context.SaveChangesAsync();
    }

    public async Task Eliminar(DetalleCompra obj)
    {
        _context.DetallesCompra.Remove(obj);
        await _context.SaveChangesAsync();
    }

    public async Task Modificar(DetalleCompra obj)
    {
        _context.DetallesCompra.Update(obj);
        await _context.SaveChangesAsync();
    }

    public async Task<List<DetalleCompra>> ObtenerPorCompra(int compra_id)
    {
        return await _context.DetallesCompra
            .Where(d => d.CompraId.Equals(compra_id))
            .ToListAsync();
    }

    public async Task<List<DetalleCompra>> ObtenerPorCompra(Compra compra)
    {
        return await ObtenerPorCompra(compra.Id);
    }

    public async Task<List<DetalleCompra>> ObtenerPorLibro(int libro_id)
    {
        return await _context.DetallesCompra
            .Where(d => d.LibroId.Equals(libro_id))
            .ToListAsync();
    }

    public async Task<List<DetalleCompra>> ObtenerPorLibro(Libro libro)
    {
        return await ObtenerPorLibro(libro.Id);
    }
}
