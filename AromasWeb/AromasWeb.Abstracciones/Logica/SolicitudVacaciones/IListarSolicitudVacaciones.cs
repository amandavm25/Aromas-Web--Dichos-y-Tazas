using System.Collections.Generic;
using SolicitudVacacionesUI = AromasWeb.Abstracciones.ModeloUI.SolicitudVacaciones;

namespace AromasWeb.Abstracciones.Logica.SolicitudVacaciones
{
    public interface IListarSolicitudesVacaciones
    {
        List<SolicitudVacacionesUI> Obtener();
        List<SolicitudVacacionesUI> BuscarPorEmpleado(int idEmpleado);
        List<SolicitudVacacionesUI> BuscarPorEstado(string estado);
        SolicitudVacacionesUI ObtenerPorId(int id);
    }
}