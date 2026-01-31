using System.Collections.Generic;
using HistorialTarifaUI = AromasWeb.Abstracciones.ModeloUI.HistorialTarifa;

namespace AromasWeb.Abstracciones.Logica.HistorialTarifa
{
    public interface IListarHistorialTarifa
    {
        List<HistorialTarifaUI> Obtener();
        List<HistorialTarifaUI> ObtenerPorEmpleado(int idEmpleado);
        List<HistorialTarifaUI> ObtenerPorEstado(string estado);
        HistorialTarifaUI ObtenerPorId(int id);
        HistorialTarifaUI ObtenerTarifaActualPorEmpleado(int idEmpleado);
    }
}