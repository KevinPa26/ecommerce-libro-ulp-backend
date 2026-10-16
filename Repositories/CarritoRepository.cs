using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class CarritoRepository : ICarritoRepository
{
	private readonly AppDbContext _context;

	public CarritoRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task<Carrito?> ObtenerPorId(int id)
	{
		return await _context.Carritos.FindAsync(id);
	}

	public async Task<List<Carrito>> ObtenerTodos()
	{
		return await _context.Carritos.ToListAsync();
	}

	public async Task Crear(Carrito obj)
	{
		await _context.Carritos.AddAsync(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Eliminar(Carrito obj)
	{
		_context.Carritos.Remove(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Modificar(Carrito obj)
	{
		_context.Carritos.Update(obj);
		await _context.SaveChangesAsync();
	}

	public async Task<List<Carrito>> ObtenerPorUsuarioId(int user_id)
	{
		return await _context.Carritos
			.Where(c => c.UsuarioId.Equals(user_id))
			.ToListAsync();
	}

	public async Task<List<Carrito>> ObtenerPorUsuario(Usuario usuario)
	{
		return await _context.Carritos
			.Where(c => c.UsuarioId.Equals(usuario.Id))
			.ToListAsync();
	}
}
