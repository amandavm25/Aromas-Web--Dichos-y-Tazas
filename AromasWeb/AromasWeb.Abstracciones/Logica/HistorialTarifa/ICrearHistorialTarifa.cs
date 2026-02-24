using System.Threading.Tasks;
using HistorialTarifaUI = AromasWeb.Abstracciones.ModeloUI.HistorialTarifa;

namespace AromasWeb.Abstracciones.Logica.HistorialTarifa
{
    public interface ICrearHistorialTarifa
    {
        Task<int> Crear(HistorialTarifaUI historialTarifa);
    }
}
