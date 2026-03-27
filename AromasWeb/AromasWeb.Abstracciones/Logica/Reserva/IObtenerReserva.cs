using ReservaUI = AromasWeb.Abstracciones.ModeloUI.Reserva;

namespace AromasWeb.Abstracciones.Logica.Reserva
{
    public interface IObtenerReserva
    {
        ReservaUI Obtener(int idReserva);
    }
}