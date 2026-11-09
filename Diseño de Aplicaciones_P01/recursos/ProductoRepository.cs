using Diseño_de_Aplicaciones_P01.model;

namespace Diseño_de_Aplicaciones_P01.recursos
{
    public class ProductoRepository
    {
        private readonly List<Producto> _productos = new()
        {
            new Producto { Id = 1, Nombre = "Café Americano", Precio = 2.50m, Activo = true },
            new Producto { Id = 2, Nombre = "Capuchino", Precio = 3.50m, Activo = true }
        };

        public List<Producto> ObtenerTodos() => _productos;

        public Producto? BuscarPorId(int id) => _productos.FirstOrDefault(p => p.Id == id);

        public Producto Agregar(Producto producto)
        {
            producto.Id = _productos.Count > 0 ? _productos.Max(p => p.Id) + 1 : 1;
            _productos.Add(producto);
            return producto;
        }

        public bool EliminarPorId(int id)
        {
            var p = BuscarPorId(id);
            if (p == null) return false;
            _productos.Remove(p);
            return true;
        }
    }
}
