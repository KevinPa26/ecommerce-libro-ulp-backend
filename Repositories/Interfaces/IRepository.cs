public interface IRepository<T> where T : class
{
	abstract T? ObtenerPorId(int id);
	abstract List<T> ObtenerTodos();
	abstract void Crear(T obj);
	abstract void Eliminar(T obj);
}