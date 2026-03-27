using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Reservas
{
    public class ActualizarReserva : IActualizarReserva
    {
        private readonly IActualizarReserva _actualizarReserva;

        public ActualizarReserva()
        {
            _actualizarReserva = new AccesoADatos.Reservas.ActualizarReserva();
        }

        public int Actualizar(Reserva reserva)
        {
            return _actualizarReserva.Actualizar(reserva);
        }

        public int ActualizarEstado(int idReserva, string estado)
        {
            return _actualizarReserva.ActualizarEstado(idReserva, estado);
        }
    }
}