using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class CompraRepository : ICompraRepository
{
	private readonly DataContext _context;

	public CompraRepository(DataContext context)
	{
		_context = context;
	}

	public async Task Crear(Compra obj)
	{
		await _context.Compras.AddAsync(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Eliminar(Compra obj)
	{
		_context.Compras.Remove(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Modificar(Compra obj)
	{
		_context.Compras.Update(obj);
		await _context.SaveChangesAsync();
	}

	public async Task<Compra?> ObtenerPorId(int id)
	{
		return await _context.Compras
			.Where(c => c.Id.Equals(id))
			.FirstAsync();
	}

	public async Task<List<Compra>> ObtenerTodos()
	{
		return await _context.Compras.ToListAsync();
	}

	public async Task<(List<Compra> Items, int TotalCount)> ObtenerTodos(
		DateOnly? fechaExacta, 
		DateOnly? desde, 
		DateOnly? hasta, 
		int pageNumber, 
		int pageSize, 
		int? usuarioId = null)
	{
		var query = _context.Compras.AsQueryable();

		if (usuarioId.HasValue)
		{
			query = query.Where(c => c.UsuarioId == usuarioId.Value);
		}

		if (fechaExacta.HasValue)
		{
			query = query.Where(c => c.FechaCompra.Equals(fechaExacta.Value));
		}
		else if (desde.HasValue && hasta.HasValue)
		{
			query = query.Where(c => c.FechaCompra >= desde.Value && c.FechaCompra <= hasta.Value);
		}

		int totalCount = await query.CountAsync();

		var items = await query
			.OrderByDescending(c => c.FechaCompra)
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync();

		// Se retorna la tupla con los dos valores
		return (items, totalCount);
	}
}