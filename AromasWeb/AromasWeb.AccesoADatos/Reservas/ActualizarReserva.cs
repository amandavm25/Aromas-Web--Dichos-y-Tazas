using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Reservas
{
    public class ActualizarReserva : IActualizarReserva
    {
        public int Actualizar(Abstracciones.ModeloUI.Reserva reserva)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var existente = contexto.Reserva
                        .FirstOrDefault(r => r.IdReserva == reserva.IdReserva);

                    if (existente == null)
                        return 0;

                    existente.CantidadPersonas = reserva.CantidadPersonas;
                    existente.Fecha            = DateTime.SpecifyKind(reserva.Fecha.Date, DateTimeKind.Utc);
                    existente.Hora             = reserva.Hora;
                    existente.Estado           = reserva.Estado;
                    existente.Observaciones    = reserva.Observaciones;

                    return contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al actualizar reserva: {ex.Message}");
                    throw;
                }
            }
        }

        public int ActualizarEstado(int idReserva, string estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var existente = contexto.Reserva
                        .FirstOrDefault(r => r.IdReserva == idReserva);

                    if (existente == null)
                        return 0;

                    existente.Estado = estado;
                    return contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al actualizar estado de reserva: {ex.Message}");
                    throw;
                }
            }
        }
    }
}