using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Promociones
{
    public class ListarPromociones : IListarPromociones
    {
        private IListarPromociones _listarPromociones;

        public ListarPromociones()
        {
            _listarPromociones = new AccesoADatos.Promocion.ListarPromociones();
        }

        public List<Promocion> Obtener()
        {
            return _listarPromociones.Obtener();
        }

        public List<Promocion> BuscarPorNombre(string nombre)
        {
            return _listarPromociones.BuscarPorNombre(nombre);
        }

        public List<Promocion> BuscarPorTipo(int idTipoPromocion)
        {
            return _listarPromociones.BuscarPorTipo(idTipoPromocion);
        }

        public List<Promocion> BuscarPorVigencia(string vigencia)
        {
            return _listarPromociones.BuscarPorVigencia(vigencia);
        }

        public Promocion ObtenerPorId(int id)
        {
            return _listarPromociones.ObtenerPorId(id);
        }

        public List<Promocion> ObtenerVigentes()
        {
            return _listarPromociones.ObtenerVigentes();
        }

        public List<Promocion> ObtenerPorFecha(DateTime fecha)
        {
            return _listarPromociones.ObtenerPorFecha(fecha);
        }
    }
}