using SolicitudVacacionesUI = AromasWeb.Abstracciones.ModeloUI.SolicitudVacaciones;

namespace AromasWeb.Abstracciones.Logica.SolicitudVacaciones
{
    public interface IActualizarSolicitudVacaciones
    {
        int Actualizar(SolicitudVacacionesUI solicitud);
        int ActualizarEstado(int idSolicitud, string estado);
    }
}