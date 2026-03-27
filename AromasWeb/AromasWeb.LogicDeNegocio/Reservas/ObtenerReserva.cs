using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Reservas
{
    public class ObtenerReserva : IObtenerReserva
    {
        private readonly IObtenerReserva _obtenerReserva;

        public ObtenerReserva()
        {
            _obtenerReserva = new AccesoADatos.Reservas.ObtenerReserva();
        }

        public Reserva Obtener(int idReserva)
        {
            Reserva reserva = _obtenerReserva.Obtener(idReserva);
            return reserva;
        }
    }
}