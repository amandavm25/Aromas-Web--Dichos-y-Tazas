using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.Reservas
{
    public class CrearReserva : ICrearReserva
    {
        public async Task<int> Crear(Abstracciones.ModeloUI.Reserva reserva)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    bool clienteExiste = await contexto.Cliente
                        .AnyAsync(c => c.IdCliente == reserva.IdCliente && c.Estado == true);
                    if (!clienteExiste)
                        throw new Exception("El cliente seleccionado no existe o está inactivo.");

                    DateTime fechaReserva = reserva.Fecha.Date;
                    if (fechaReserva < DateTime.Now.Date)
                        throw new Exception("La fecha de la reserva no puede ser en el pasado.");

                    bool solapada = await contexto.Reserva
                        .AnyAsync(r => r.IdCliente == reserva.IdCliente
                                    && r.Fecha.Date == fechaReserva
                                    && r.Hora == reserva.Hora
                                    && r.Estado != "Cancelada");
                    if (solapada)
                        throw new Exception("Ya existe una reserva para ese cliente en la misma fecha y hora.");

                    var nuevaReserva = new ReservaAD
                    {
                        IdCliente        = reserva.IdCliente,
                        CantidadPersonas = reserva.CantidadPersonas,
                        Fecha            = DateTime.SpecifyKind(fechaReserva, DateTimeKind.Utc),
                        Hora             = reserva.Hora,
                        Estado           = string.IsNullOrWhiteSpace(reserva.Estado) ? "Pendiente" : reserva.Estado,
                        Observaciones    = reserva.Observaciones,
                        FechaCreacion    = DateTime.UtcNow
                    };

                    contexto.Reserva.Add(nuevaReserva);
                    return await contexto.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al crear reserva: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
