using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.HistorialTarifas
{
    public class ObtenerHistorialTarifa : IObtenerHistorialTarifa
    {
        private IObtenerHistorialTarifa _obtenerHistorialTarifa;

        public ObtenerHistorialTarifa()
        {
            _obtenerHistorialTarifa = new AccesoADatos.HistorialTarifas.ObtenerHistorialTarifa();
        }

        public HistorialTarifa Obtener(int id)
        {
            HistorialTarifa historialTarifa = _obtenerHistorialTarifa.Obtener(id);
            return historialTarifa;
        }
    }
}