using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.HistorialTarifas
{
    public class CrearHistorialTarifa : ICrearHistorialTarifa
    {
        private ICrearHistorialTarifa _crearHistorialTarifa;

        public CrearHistorialTarifa()
        {
            _crearHistorialTarifa = new AccesoADatos.HistorialTarifas.CrearHistorialTarifa();
        }

        public async Task<int> Crear(HistorialTarifa historialTarifa)
        {
            int cantidadDeDatosInsertados = await _crearHistorialTarifa.Crear(historialTarifa);
            return cantidadDeDatosInsertados;
        }
    }
}
