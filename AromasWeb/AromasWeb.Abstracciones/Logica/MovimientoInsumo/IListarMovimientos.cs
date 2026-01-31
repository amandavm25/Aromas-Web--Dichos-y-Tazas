using System;
using System.Collections.Generic;
using MovimientoInsumoUI = AromasWeb.Abstracciones.ModeloUI.MovimientoInsumo;

namespace AromasWeb.Abstracciones.Logica.MovimientoInsumo
{
    public interface IListarMovimientos
    {
        List<MovimientoInsumoUI> Obtener();
        List<MovimientoInsumoUI> BuscarPorInsumo(string nombreInsumo);
        List<MovimientoInsumoUI> BuscarPorTipo(string tipo);
        List<MovimientoInsumoUI> BuscarPorRangoFechas(DateTime fechaDesde, DateTime fechaHasta);
        List<MovimientoInsumoUI> BuscarConFiltros(string nombreInsumo, string tipo, DateTime? fechaDesde, DateTime? fechaHasta);
        MovimientoInsumoUI ObtenerPorId(int id);
        List<MovimientoInsumoUI> ObtenerUltimosMovimientos(int cantidad);
    }
}