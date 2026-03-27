using System.Threading.Tasks;
using ReservaUI = AromasWeb.Abstracciones.ModeloUI.Reserva;

namespace AromasWeb.Abstracciones.Logica.Reserva
{
    public interface ICrearReserva
    {
        Task<int> Crear(ReservaUI reserva);
    }
}
