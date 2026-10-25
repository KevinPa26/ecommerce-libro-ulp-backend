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

	public async Task<List<Compra>> ObtenerPorFecha(DateTime fecha)
	{
		return await _context.Compras
			.Where(c => c.FechaCompra.Equals(fecha))
			.ToListAsync();
	}

	public async Task<List<Compra>> ObtenerPorFecha(DateTime desde, DateTime hasta)
	{
		return await _context.Compras
			.Where(c => c.FechaCompra >= desde && c.FechaCompra <= hasta)
			.ToListAsync();
	}

	public async Task<List<Compra>> ObtenerPorFecha(DateTime desde, DateTime hasta, Usuario usuario)
	{
		return await _context.Compras
			.Where(c =>
				c.FechaCompra >= desde &&
				c.FechaCompra <= hasta &&
				c.UsuarioId.Equals(usuario.Id)
			)
			.ToListAsync();
	}

	public async Task<List<Compra>> ObtenerPorFecha(DateTime fecha, Usuario usuario)
	{
		return await _context.Compras
			.Where(c => 
				c.FechaCompra.Equals(fecha) && 
				c.UsuarioId.Equals(usuario.Id)
			)
			.ToListAsync();
	}

	public async Task<Compra?> ObtenerPorId(int id)
	{
		return await _context.Compras
			.Where(c => c.Id.Equals(id))
			.FirstAsync();
	}

	public async Task<List<Compra>> ObtenerPorUsuarioId(int user_id)
	{
		return await _context.Compras
			.Where(c => c.UsuarioId.Equals(user_id))
			.ToListAsync();
	}

	public async Task<List<Compra>> ObtenerTodos()
	{
		return await _context.Compras.ToListAsync();
	}
}