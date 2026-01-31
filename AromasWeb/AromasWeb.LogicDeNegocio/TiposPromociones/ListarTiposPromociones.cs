using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.TiposPromociones
{
    public class ListarTiposPromociones : IListarTiposPromociones
    {
        private IListarTiposPromociones _listarTiposPromociones;

        public ListarTiposPromociones()
        {
            _listarTiposPromociones = new AccesoADatos.TiposPromociones.ListarTiposPromociones();
        }

        public List<TipoPromocion> Obtener()
        {
            return _listarTiposPromociones.Obtener();
        }

        public List<TipoPromocion> BuscarPorNombre(string nombre)
        {
            return _listarTiposPromociones.BuscarPorNombre(nombre);
        }

        public TipoPromocion ObtenerPorId(int id)
        {
            return _listarTiposPromociones.ObtenerPorId(id);
        }
    }
}