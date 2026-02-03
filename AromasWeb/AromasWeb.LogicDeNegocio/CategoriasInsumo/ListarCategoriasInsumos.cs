using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.CategoriasInsumo
{
    public class ListarCategoriasInsumo : IListarCategoriasInsumo
    {
        private IListarCategoriasInsumo _listarCategoriasInsumo;

        public ListarCategoriasInsumo()
        {
            _listarCategoriasInsumo = new AccesoADatos.CategoriasInsumo.ListarCategoriasInsumo();
        }

        public List<CategoriaInsumo> Obtener()
        {
            return _listarCategoriasInsumo.Obtener();
        }

        public List<CategoriaInsumo> BuscarPorNombre(string nombre)
        {
            return _listarCategoriasInsumo.BuscarPorNombre(nombre);
        }

        public CategoriaInsumo ObtenerPorId(int id)
        {
            return _listarCategoriasInsumo.ObtenerPorId(id);
        }
    }
}