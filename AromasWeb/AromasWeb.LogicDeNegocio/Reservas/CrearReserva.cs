using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.Reservas
{
    public class CrearReserva : ICrearReserva
    {
        private readonly ICrearReserva _crearReserva;

        public CrearReserva()
        {
            _crearReserva = new AccesoADatos.Reservas.CrearReserva();
        }

        public async Task<int> Crear(Reserva reserva)
        {
            return await _crearReserva.Crear(reserva);
        }
    }
}
