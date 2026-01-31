using AromasWeb.Abstracciones.Logica.MovimientoInsumo;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.MovimientosInsumo
{
    public class ListarMovimientos : IListarMovimientos
    {
        private IListarMovimientos _listarMovimientos;

        public ListarMovimientos()
        {
            _listarMovimientos = new AccesoADatos.MovimientosInsumo.ListarMovimientos();
        }

        public List<MovimientoInsumo> Obtener()
        {
            return _listarMovimientos.Obtener();
        }

        public List<MovimientoInsumo> BuscarPorInsumo(string nombreInsumo)
        {
            return _listarMovimientos.BuscarPorInsumo(nombreInsumo);
        }

        public List<MovimientoInsumo> BuscarPorTipo(string tipo)
        {
            return _listarMovimientos.BuscarPorTipo(tipo);
        }

        public List<MovimientoInsumo> BuscarPorRangoFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            return _listarMovimientos.BuscarPorRangoFechas(fechaDesde, fechaHasta);
        }

        public List<MovimientoInsumo> BuscarConFiltros(string nombreInsumo, string tipo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            return _listarMovimientos.BuscarConFiltros(nombreInsumo, tipo, fechaDesde, fechaHasta);
        }

        public MovimientoInsumo ObtenerPorId(int id)
        {
            return _listarMovimientos.ObtenerPorId(id);
        }

        public List<MovimientoInsumo> ObtenerUltimosMovimientos(int cantidad)
        {
            return _listarMovimientos.ObtenerUltimosMovimientos(cantidad);
        }
    }
}