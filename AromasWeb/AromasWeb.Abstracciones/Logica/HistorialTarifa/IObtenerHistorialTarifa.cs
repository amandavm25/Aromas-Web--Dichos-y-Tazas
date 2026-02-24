using HistorialTarifaUI = AromasWeb.Abstracciones.ModeloUI.HistorialTarifa;

namespace AromasWeb.Abstracciones.Logica.HistorialTarifa
{
    public interface IObtenerHistorialTarifa
    {
        HistorialTarifaUI Obtener(int id);
    }
}