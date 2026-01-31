using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.CategoriasReceta
{
    public class ListarCategoriasReceta : IListarCategoriasReceta
    {
        private IListarCategoriasReceta _listarCategoriasReceta;

        public ListarCategoriasReceta()
        {
            _listarCategoriasReceta = new AccesoADatos.CategoriasReceta.ListarCategoriasReceta();
        }

        public List<CategoriaReceta> Obtener()
        {
            return _listarCategoriasReceta.Obtener();
        }

        public List<CategoriaReceta> BuscarPorNombre(string nombre)
        {
            return _listarCategoriasReceta.BuscarPorNombre(nombre);
        }

        public CategoriaReceta ObtenerPorId(int id)
        {
            return _listarCategoriasReceta.ObtenerPorId(id);
        }
    }
}