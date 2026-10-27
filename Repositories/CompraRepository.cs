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

	public async Task Crear(Compra compra)
	{
		using var transaction = await _context.Database.BeginTransactionAsync();
		try
		{
			decimal totalCompra = 0;

			foreach (var detalle in compra.Detalles)
			{
				// Buscamos el libro en la base de datos para asegurar stock y precio real
				var libro = await _context.Libros.FindAsync(detalle.LibroId);

				if (libro == null || libro.Estado != EstadoLibro.DISPONIBLE)
				{
					await transaction.RollbackAsync();
					throw new Exception($"El libro con ID {detalle.LibroId} no se encuentra disponible.");
				}

				if (libro.Stock < detalle.Cantidad)
				{
					await transaction.RollbackAsync();
					throw new Exception($"Stock insuficiente para '{libro.Titulo}'. Disponibles: {libro.Stock}");
				}

				// Descontar stock
				libro.Stock -= detalle.Cantidad;
				
				if (libro.Stock == 0)
				{
					libro.Estado = EstadoLibro.AGOTADO;
				}

				// Asignar precio unitario oficial del sistema (evita que manipulen precios desde el front)
				detalle.PrecioUnitario = libro.Precio;

				// Calcular acumulador total
				totalCompra += detalle.Cantidad * detalle.PrecioUnitario;
			}

			// Guardar la cabecera de la compra primero para obtener su ID
			_context.Compras.Add(compra);
			await _context.SaveChangesAsync(); // Aquí EF Core ya asigna compra.Id a los detalles por la navegación

			await transaction.CommitAsync();
		}
		catch (Exception)
		{
			await transaction.RollbackAsync();
			throw;
		}
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