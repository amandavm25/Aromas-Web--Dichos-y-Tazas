using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Reservas
{
    public class ListarReservas : IListarReservas
    {
        public List<Abstracciones.ModeloUI.Reserva> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(p => p.Cliente)
                        .OrderByDescending(r => r.Fecha)
                        .ThenByDescending(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reservas: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> BuscarPorEstado(string estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Estado.ToLower() == estado.ToLower())
                        .OrderByDescending(r => r.Fecha)
                        .ThenByDescending(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar reservas por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> BuscarPorFecha(DateTime fecha)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Fecha.Date == fecha.Date)
                        .OrderBy(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar reservas por fecha: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> ObtenerPorCliente(int idCliente)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.IdCliente == idCliente)
                        .OrderByDescending(r => r.Fecha)
                        .ThenByDescending(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reservas por cliente: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> ObtenerReservasHoy()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime hoy = DateTime.Now.Date;
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Fecha.Date == hoy)
                        .OrderBy(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reservas de hoy: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> ObtenerReservasFuturas()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime ahora = DateTime.Now;
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Fecha > ahora.Date ||
                               (r.Fecha.Date == ahora.Date && r.Hora > ahora.TimeOfDay))
                        .OrderBy(r => r.Fecha)
                        .ThenBy(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reservas futuras: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> ObtenerReservasPasadas()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime ahora = DateTime.Now;
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Fecha < ahora.Date ||
                               (r.Fecha.Date == ahora.Date && r.Hora < ahora.TimeOfDay))
                        .OrderByDescending(r => r.Fecha)
                        .ThenByDescending(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reservas pasadas: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> ObtenerReservasProximas(int dias)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime ahora = DateTime.Now;
                    DateTime fechaLimite = ahora.Date.AddDays(dias);
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => (r.Fecha > ahora.Date ||
                                    (r.Fecha.Date == ahora.Date && r.Hora > ahora.TimeOfDay)) &&
                                    r.Fecha.Date <= fechaLimite)
                        .OrderBy(r => r.Fecha)
                        .ThenBy(r => r.Hora)
                        .ToList();
                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reservas próximas: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Reserva ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var reservaAD = contexto.Reserva
                        .Include(p => p.Cliente)
                        .FirstOrDefault(r => r.IdReserva == id);
                    return reservaAD != null ? ConvertirObjetoParaUI(reservaAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener reserva por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Buscar reservas donde el nombre o apellidos del cliente coincidan
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Cliente != null &&
                                   (r.Cliente.Nombre + " " + r.Cliente.Apellidos)
                                   .ToLower()
                                   .Contains(nombre.ToLower()))
                        .OrderByDescending(r => r.Fecha)
                        .ThenByDescending(r => r.Hora)
                        .ToList();

                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar reservas por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Reserva> BuscarPorTelefono(string telefono)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Buscar reservas donde el teléfono del cliente coincida
                    List<ReservaAD> reservasAD = contexto.Reserva
                        .Include(r => r.Cliente)
                        .Where(r => r.Cliente != null &&
                                   r.Cliente.Telefono.Contains(telefono))
                        .OrderByDescending(r => r.Fecha)
                        .ThenByDescending(r => r.Hora)
                        .ToList();

                    return reservasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar reservas por teléfono: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Reserva ConvertirObjetoParaUI(ReservaAD reservaAD)
        {
            return new Abstracciones.ModeloUI.Reserva
            {
                IdReserva = reservaAD.IdReserva,
                IdCliente = reservaAD.IdCliente,
                NombreCliente = reservaAD.Cliente != null
                    ? $"{reservaAD.Cliente.Nombre} {reservaAD.Cliente.Apellidos}"
                    : "Cliente no encontrado",
                TelefonoCliente = reservaAD.Cliente?.Telefono ?? "N/A",
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