using SolicitudVacacionesUI = AromasWeb.Abstracciones.ModeloUI.SolicitudVacaciones;

namespace AromasWeb.Abstracciones.Logica.SolicitudVacaciones
{
    public interface IObtenerSolicitudVacaciones
    {
        SolicitudVacacionesUI Obtener(int id);
    }
}