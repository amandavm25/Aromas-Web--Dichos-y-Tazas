using HistorialTarifaUI = AromasWeb.Abstracciones.ModeloUI.HistorialTarifa;

namespace AromasWeb.Abstracciones.Logica.HistorialTarifa
{
    public interface IActualizarHistorialTarifa
    {

        int Actualizar(HistorialTarifaUI historialTarifa);
    }
}
