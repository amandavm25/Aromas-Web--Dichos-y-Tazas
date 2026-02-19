using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.SolicitudesVacaciones
{
    public class CrearSolicitudVacaciones : ICrearSolicitudVacaciones
    {
        private ICrearSolicitudVacaciones _crearSolicitudVacaciones;

        public CrearSolicitudVacaciones()
        {
            _crearSolicitudVacaciones = new AccesoADatos.SolicitudesVacaciones.CrearSolicitudVacaciones();
        }

        public async Task<int> Crear(SolicitudVacaciones solicitud)
        {
            int resultado = await _crearSolicitudVacaciones.Crear(solicitud);
            return resultado;
        }
    }
}