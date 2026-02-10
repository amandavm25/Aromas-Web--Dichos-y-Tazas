using AromasWeb.Abstracciones.Logica.Insumo;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Insumos
{
    public class ListarInsumos : IListarInsumos
    {
        private IListarInsumos _listarInsumos;

        public ListarInsumos()
        {
            _listarInsumos = new AccesoADatos.Insumos.ListarInsumos();
        }

        public List<Insumo> Obtener()
        {
            return _listarInsumos.Obtener();
        }

        public List<Insumo> BuscarPorNombre(string nombre)
        {
            return _listarInsumos.BuscarPorNombre(nombre);
        }

        public List<Insumo> BuscarPorCategoria(int idCategoria)
        {
            return _listarInsumos.BuscarPorCategoria(idCategoria);
        }

        public Insumo ObtenerPorId(int id)
        {
            return _listarInsumos.ObtenerPorId(id);
        }

        public List<Insumo> ObtenerBajoStock()
        {
            return _listarInsumos.ObtenerBajoStock();
        }

        public void Crear (Insumo insumo)
        {
            _listarInsumos.Crear(insumo);
        }

        public void Actualizar (Insumo insumo)
        {
            _listarInsumos.Actualizar(insumo);
        }
    }
}