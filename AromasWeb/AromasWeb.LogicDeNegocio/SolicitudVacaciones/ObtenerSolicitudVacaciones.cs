using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.SolicitudesVacaciones
{
    public class ObtenerSolicitudVacaciones : IObtenerSolicitudVacaciones
    {
        private IObtenerSolicitudVacaciones _obtenerSolicitudVacaciones;

        public ObtenerSolicitudVacaciones()
        {
            _obtenerSolicitudVacaciones = new AccesoADatos.SolicitudesVacaciones.ObtenerSolicitudVacaciones();
        }

        public SolicitudVacaciones Obtener(int id)
        {
            SolicitudVacaciones solicitud = _obtenerSolicitudVacaciones.Obtener(id);
            return solicitud;
        }
    }
}