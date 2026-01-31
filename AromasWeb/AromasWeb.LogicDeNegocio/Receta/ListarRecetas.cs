using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Recetas
{
    public class ListarRecetas : IListarRecetas
    {
        private IListarRecetas _listarRecetas;

        public ListarRecetas()
        {
            _listarRecetas = new AccesoADatos.Receta.ListarRecetas();
        }

        public List<Receta> Obtener()
        {
            return _listarRecetas.Obtener();
        }

        public List<Receta> BuscarPorNombre(string nombre)
        {
            return _listarRecetas.BuscarPorNombre(nombre);
        }

        public List<Receta> BuscarPorCategoria(int idCategoria)
        {
            return _listarRecetas.BuscarPorCategoria(idCategoria);
        }

        public Receta ObtenerPorId(int id)
        {
            return _listarRecetas.ObtenerPorId(id);
        }

        public List<Receta> ObtenerDisponibles()
        {
            return _listarRecetas.ObtenerDisponibles();
        }

        public List<Receta> ObtenerNoDisponibles()
        {
            return _listarRecetas.ObtenerNoDisponibles();
        }
    }
}