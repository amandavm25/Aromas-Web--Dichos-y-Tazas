using System;
using System.Collections.Generic;
using BitacoraUI = AromasWeb.Abstracciones.ModeloUI.Bitacora;

namespace AromasWeb.Abstracciones.Logica.Bitacora
{
    public interface IListarBitacora
    {
        List<BitacoraUI> Obtener();
        List<BitacoraUI> BuscarPorFiltros(string buscar, string filtroModulo, DateTime? fechaInicio, DateTime? fechaFin);
        BitacoraUI ObtenerPorId(int id);
        List<BitacoraUI> ObtenerPorEmpleado(int idEmpleado);
        List<BitacoraUI> ObtenerPorModulo(int idModulo);
        List<BitacoraUI> ObtenerPorRangoDeFechas(DateTime fechaInicio, DateTime fechaFin);
    }
}