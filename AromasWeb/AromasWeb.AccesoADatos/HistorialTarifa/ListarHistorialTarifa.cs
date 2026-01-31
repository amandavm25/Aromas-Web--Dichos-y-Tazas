using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.HistorialTarifas
{
    public class ListarHistorialTarifa : IListarHistorialTarifa
    {
        public List<Abstracciones.ModeloUI.HistorialTarifa> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<HistorialTarifaAD> historialAD = contexto.HistorialTarifa
                        .OrderByDescending(h => h.FechaRegistro)
                        .ToList();
                    return historialAD.Select(h => ConvertirObjetoParaUI(h)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener historial de tarifas: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.HistorialTarifa> ObtenerPorEmpleado(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<HistorialTarifaAD> historialAD = contexto.HistorialTarifa
                        .Where(h => h.IdEmpleado == idEmpleado)
                        .OrderByDescending(h => h.FechaInicio)
                        .ToList();
                    return historialAD.Select(h => ConvertirObjetoParaUI(h)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener historial por empleado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.HistorialTarifa> ObtenerPorEstado(string estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime hoy = DateTime.Today;
                    List<HistorialTarifaAD> historialAD;

                    if (estado.ToLower() == "vigente")
                    {
                        historialAD = contexto.HistorialTarifa
                            .Where(h => h.FechaInicio <= hoy && (!h.FechaFin.HasValue || h.FechaFin >= hoy))
                            .OrderByDescending(h => h.FechaRegistro)
                            .ToList();
                    }
                    else if (estado.ToLower() == "vencida")
                    {
                        historialAD = contexto.HistorialTarifa
                            .Where(h => h.FechaFin.HasValue && h.FechaFin < hoy)
                            .OrderByDescending(h => h.FechaRegistro)
                            .ToList();
                    }
                    else if (estado.ToLower() == "futura")
                    {
                        historialAD = contexto.HistorialTarifa
                            .Where(h => h.FechaInicio > hoy)
                            .OrderByDescending(h => h.FechaRegistro)
                            .ToList();
                    }
                    else
                    {
                        historialAD = contexto.HistorialTarifa
                            .OrderByDescending(h => h.FechaRegistro)
                            .ToList();
                    }

                    return historialAD.Select(h => ConvertirObjetoParaUI(h)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener historial por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.HistorialTarifa ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var historialAD = contexto.HistorialTarifa.FirstOrDefault(h => h.IdHistorialTarifa == id);
                    return historialAD != null ? ConvertirObjetoParaUI(historialAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener historial por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.HistorialTarifa ObtenerTarifaActualPorEmpleado(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime hoy = DateTime.Today;
                    var historialAD = contexto.HistorialTarifa
                        .Where(h => h.IdEmpleado == idEmpleado
                                 && h.FechaInicio <= hoy
                                 && (!h.FechaFin.HasValue || h.FechaFin >= hoy))
                        .OrderByDescending(h => h.FechaInicio)
                        .FirstOrDefault();

                    return historialAD != null ? ConvertirObjetoParaUI(historialAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener tarifa actual: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.HistorialTarifa ConvertirObjetoParaUI(HistorialTarifaAD historialAD)
        {
            return new Abstracciones.ModeloUI.HistorialTarifa
            {
                IdHistorialTarifa = historialAD.IdHistorialTarifa,
                IdEmpleado = historialAD.IdEmpleado,
                TarifaHora = historialAD.TarifaHora,
                Motivo = historialAD.Motivo,
                FechaInicio = historialAD.FechaInicio,
                FechaFin = historialAD.FechaFin,
                FechaRegistro = historialAD.FechaRegistro
            };
        }
    }
}