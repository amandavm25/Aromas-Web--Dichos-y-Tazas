using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.AccesoADatos.Modelos;
using System.Linq;

namespace AromasWeb.AccesoADatos.HistorialTarifas
{
    public class ObtenerHistorialTarifa : IObtenerHistorialTarifa
    {
        private Contexto _contexto;

        public ObtenerHistorialTarifa()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.HistorialTarifa Obtener(int id)
        {
            var historialAD = _contexto.HistorialTarifa
                .FirstOrDefault(h => h.IdHistorialTarifa == id);

            if (historialAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(historialAD);
        }

        private Abstracciones.ModeloUI.HistorialTarifa ConvertirObjetoParaUI(HistorialTarifaAD historialAD)
        {
            return new Abstracciones.ModeloUI.HistorialTarifa
            {
                IdHistorialTarifa = historialAD.IdHistorialTarifa,
                IdEmpleado = historialAD.IdEmpleado,
                TarifaHora = historialAD.TarifaHora,
                Motivo = historialAD.Motivo,
                FechaInicio = historialAD.FechaInicio,
                FechaFin = historialAD.FechaFin,
                FechaRegistro = historialAD.FechaRegistro
            };
        }
    }
}