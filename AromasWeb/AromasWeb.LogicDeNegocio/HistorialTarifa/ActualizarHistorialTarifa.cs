using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.HistorialTarifas
{
    public class ActualizarHistorialTarifa : IActualizarHistorialTarifa
    {
        private IActualizarHistorialTarifa _actualizarHistorialTarifa;

        public ActualizarHistorialTarifa()
        {
            _actualizarHistorialTarifa = new AccesoADatos.HistorialTarifas.ActualizarHistorialTarifa();
        }

        public int Actualizar(HistorialTarifa historialTarifa)
        {
            int resultado = _actualizarHistorialTarifa.Actualizar(historialTarifa);
            return resultado;
        }
    }
}