using ReservaUI = AromasWeb.Abstracciones.ModeloUI.Reserva;

namespace AromasWeb.Abstracciones.Logica.Reserva
{
    public interface IActualizarReserva
    {
        int Actualizar(ReservaUI reserva);
        int ActualizarEstado(int idReserva, string estado);
    }
}