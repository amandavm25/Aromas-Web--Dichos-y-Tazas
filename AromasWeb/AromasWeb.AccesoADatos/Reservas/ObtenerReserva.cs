using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Reservas
{
    public class ObtenerReserva : IObtenerReserva
    {
        public Abstracciones.ModeloUI.Reserva Obtener(int idReserva)
        {
            using (var contexto = new Contexto())
            {
                var reservaAD = contexto.Reserva
                    .FirstOrDefault(r => r.IdReserva == idReserva);

                if (reservaAD == null)
                    return null;

                return ConvertirObjetoParaUI(reservaAD);
            }
        }

        private Abstracciones.ModeloUI.Reserva ConvertirObjetoParaUI(ReservaAD reservaAD)
        {
            string nombreCliente = "Cliente no encontrado";
            string telefonoCliente = "N/A";

            using (var contexto = new Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .FirstOrDefault(c => c.IdCliente == reservaAD.IdCliente);

                    if (cliente != null)
                    {
                        nombreCliente = $"{cliente.Nombre} {cliente.Apellidos}";
                        telefonoCliente = cliente.Telefono;
                    }
                }
                catch
                {
                    // Usar valores por defecto si hay error
                }

                return new Abstracciones.ModeloUI.Reserva
                {
                    IdReserva = reservaAD.IdReserva,
                    IdCliente = reservaAD.IdCliente,
                    NombreCliente = nombreCliente,
                    TelefonoCliente = telefonoCliente,
                    CantidadPersonas = reservaAD.CantidadPersonas,
                    Fecha = reservaAD.Fecha,
                    Hora = reservaAD.Hora,
                    Estado = reservaAD.Estado,
                    Observaciones = reservaAD.Observaciones,
                    FechaCreacion = reservaAD.FechaCreacion
                };
            }
        }
    }
}