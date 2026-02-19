using System.Threading.Tasks;
using SolicitudVacacionesUI = AromasWeb.Abstracciones.ModeloUI.SolicitudVacaciones;

namespace AromasWeb.Abstracciones.Logica.SolicitudVacaciones
{
    public interface ICrearSolicitudVacaciones
    {
        Task<int> Crear(SolicitudVacacionesUI solicitud);
    }
}