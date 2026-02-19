using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.SolicitudesVacaciones
{
    public class ActualizarSolicitudVacaciones : IActualizarSolicitudVacaciones
    {
        private IActualizarSolicitudVacaciones _actualizarSolicitudVacaciones;

        public ActualizarSolicitudVacaciones()
        {
            _actualizarSolicitudVacaciones = new AccesoADatos.SolicitudesVacaciones.ActualizarSolicitudVacaciones();
        }

        public int Actualizar(SolicitudVacaciones solicitud)
        {
            int resultado = _actualizarSolicitudVacaciones.Actualizar(solicitud);
            return resultado;
        }

        public int ActualizarEstado(int idSolicitud, string estado)
        {
            int resultado = _actualizarSolicitudVacaciones.ActualizarEstado(idSolicitud, estado);
            return resultado;
        }
    }
}