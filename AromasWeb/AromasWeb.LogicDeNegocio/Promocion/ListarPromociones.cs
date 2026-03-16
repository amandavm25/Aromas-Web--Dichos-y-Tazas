using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using PromocionUI = AromasWeb.Abstracciones.ModeloUI.Promocion;

namespace AromasWeb.LogicaDeNegocio.Promociones
{
    public class ListarPromociones : IListarPromociones
    {
        private IListarPromociones _listarPromociones;

        public ListarPromociones()
        {
            _listarPromociones = new AccesoADatos.Promocion.ListarPromociones();
        }

        public List<PromocionUI> Obtener()
        {
            return _listarPromociones.Obtener();
        }

        public List<PromocionUI> BuscarPorNombre(string nombre)
        {
            return _listarPromociones.BuscarPorNombre(nombre);
        }

        public List<PromocionUI> BuscarPorTipo(int idTipoPromocion)
        {
            return _listarPromociones.BuscarPorTipo(idTipoPromocion);
        }

        public List<PromocionUI> BuscarPorVigencia(string vigencia)
        {
            return _listarPromociones.BuscarPorVigencia(vigencia);
        }

        public PromocionUI ObtenerPorId(int id)
        {
            return _listarPromociones.ObtenerPorId(id);
        }

        public List<PromocionUI> ObtenerVigentes()
        {
            return _listarPromociones.ObtenerVigentes();
        }

        public List<PromocionUI> ObtenerPorFecha(DateTime fecha)
        {
            return _listarPromociones.ObtenerPorFecha(fecha);
        }
    }
}