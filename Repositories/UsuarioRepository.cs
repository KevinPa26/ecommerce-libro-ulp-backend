using ecommerce_libro_ulp_backend.Data;
using ecommerce_libro_ulp_backend.Models;
using Microsoft.EntityFrameworkCore;

public class UsuarioRepository : IUsuarioRepository
{
	private readonly AppDbContext _context;

	public UsuarioRepository(AppDbContext context)
	{
		_context = context;
	}

	public async Task Crear(Usuario obj)
	{
		await _context.Usuarios.AddAsync(obj);
		await _context.SaveChangesAsync();
	}

	public async Task Eliminar(Usuario obj)
	{
		if (obj != null)
		{
			obj.Activo = EstadoUsuario.INACTIVO;
			await _context.SaveChangesAsync();
		}
	}

	public async Task Modificar(Usuario usuario)
	{
		// Buscamos la entidad existente directamente en este contexto para evitar conflictos de tracking
		var usuarioDb = await _context.Usuarios.FindAsync(usuario.Id);

		if (usuarioDb != null)
		{
			usuarioDb.Nombre = usuario.Nombre;
			usuarioDb.Email = usuario.Email;
			usuarioDb.Rol = usuario.Rol;
			usuarioDb.Activo = usuario.Activo;
			usuarioDb.Avatar = usuario.Avatar;
			usuarioDb.PasswordHash = usuario.PasswordHash;
			// La FechaRegistro no hace falta tocarla porque se preserva intacta

			await _context.SaveChangesAsync();
		}
	}

	public async Task<Usuario?> ObtenerPorEmail(string email)
	{
		var emailNormalizado = email.Trim();

		return await _context.Usuarios
			.FirstOrDefaultAsync(u =>
				u.Activo == EstadoUsuario.ACTIVO &&
				u.Email == emailNormalizado);
	}

	public async Task<List<Usuario>> ObtenerPorEstado(EstadoUsuario estado)
	{
		return await _context.Usuarios
			.Where(u => u.Activo.Equals(estado))
			.ToListAsync();
	}

	public async Task<Usuario?> ObtenerPorId(int id)
	{
		return await _context.Usuarios.FirstOrDefaultAsync(u => u.Id == id);
	}

	public async Task<List<Usuario>> ObtenerPorNombre(string nombre)
	{
		return await _context.Usuarios
			.Where(u => u.Activo.Equals(EstadoUsuario.ACTIVO) && u.Nombre.Contains(nombre))
			.ToListAsync();
	}

	public async Task<List<Usuario>> ObtenerPorRol(RolUsuario rol)
	{
		return await _context.Usuarios
			.Where(u => u.Activo.Equals(EstadoUsuario.ACTIVO) && u.Rol.Equals(rol))
			.ToListAsync();
	}

	public async Task<List<Usuario>> ObtenerTodos()
	{
		return await _context.Usuarios
			.Where(u => u.Activo.Equals(EstadoUsuario.ACTIVO))
			.ToListAsync();
	}

	public async Task<string?> ObtenerAvatarPorId(int id)
	{
		var usuario = await _context.Usuarios
			.Where(u => u.Id == id)
			.Select(u => u.Avatar)
			.FirstOrDefaultAsync();

		return usuario;
	}
}