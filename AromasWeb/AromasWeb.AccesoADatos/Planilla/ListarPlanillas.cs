using AromasWeb.Abstracciones.Logica.Planilla;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Planillas
{
    public class ListarPlanillas : IListarPlanillas
    {
        public List<Abstracciones.ModeloUI.Planilla> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var planillasAD = contexto.Planilla
                        .Include(p => p.Empleado)
                        .OrderByDescending(p => p.PeriodoFin)
                        .ToList();

                    var planillasUI = new List<Abstracciones.ModeloUI.Planilla>();

                    foreach (var planillaAD in planillasAD)
                    {
                        var planillaUI = ConvertirObjetoParaUI(planillaAD);

                        // Obtener datos del empleado
                        var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == planillaAD.IdEmpleado);
                        if (empleado != null)
                        {
                            planillaUI.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                            planillaUI.IdentificacionEmpleado = empleado.Identificacion;
                            planillaUI.CargoEmpleado = empleado.Cargo;
                        }

                        planillasUI.Add(planillaUI);
                    }

                    return planillasUI;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener planillas: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Planilla> ObtenerPorEmpleado(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var planillasAD = contexto.Planilla
                        .Where(p => p.IdEmpleado == idEmpleado)
                        .OrderByDescending(p => p.PeriodoFin)
                        .ToList();

                    var planillasUI = new List<Abstracciones.ModeloUI.Planilla>();

                    foreach (var planillaAD in planillasAD)
                    {
                        var planillaUI = ConvertirObjetoParaUI(planillaAD);

                        // Obtener datos del empleado
                        var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == planillaAD.IdEmpleado);
                        if (empleado != null)
                        {
                            planillaUI.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                            planillaUI.IdentificacionEmpleado = empleado.Identificacion;
                            planillaUI.CargoEmpleado = empleado.Cargo;
                        }

                        planillasUI.Add(planillaUI);
                    }

                    return planillasUI;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener planillas por empleado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Planilla> ObtenerPorEstado(string estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var planillasAD = contexto.Planilla
                        .Where(p => p.Estado.ToLower() == estado.ToLower())
                        .OrderByDescending(p => p.PeriodoFin)
                        .ToList();

                    var planillasUI = new List<Abstracciones.ModeloUI.Planilla>();

                    foreach (var planillaAD in planillasAD)
                    {
                        var planillaUI = ConvertirObjetoParaUI(planillaAD);

                        // Obtener datos del empleado
                        var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == planillaAD.IdEmpleado);
                        if (empleado != null)
                        {
                            planillaUI.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                            planillaUI.IdentificacionEmpleado = empleado.Identificacion;
                            planillaUI.CargoEmpleado = empleado.Cargo;
                        }

                        planillasUI.Add(planillaUI);
                    }

                    return planillasUI;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener planillas por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Planilla> ObtenerPorPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var planillasAD = contexto.Planilla
                        .Where(p => p.PeriodoInicio >= fechaInicio && p.PeriodoFin <= fechaFin)
                        .OrderByDescending(p => p.PeriodoFin)
                        .ToList();

                    var planillasUI = new List<Abstracciones.ModeloUI.Planilla>();

                    foreach (var planillaAD in planillasAD)
                    {
                        var planillaUI = ConvertirObjetoParaUI(planillaAD);

                        // Obtener datos del empleado
                        var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == planillaAD.IdEmpleado);
                        if (empleado != null)
                        {
                            planillaUI.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                            planillaUI.IdentificacionEmpleado = empleado.Identificacion;
                            planillaUI.CargoEmpleado = empleado.Cargo;
                        }

                        planillasUI.Add(planillaUI);
                    }

                    return planillasUI;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener planillas por período: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Planilla ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var planillaAD = contexto.Planilla
                        .Include(p => p.Empleado)
                        .FirstOrDefault(p => p.IdPlanilla == id);

                    if (planillaAD == null)
                        return null;

                    var planillaUI = ConvertirObjetoParaUI(planillaAD);

                    // Obtener datos del empleado
                    var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == planillaAD.IdEmpleado);
                    if (empleado != null)
                    {
                        planillaUI.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                        planillaUI.IdentificacionEmpleado = empleado.Identificacion;
                        planillaUI.CargoEmpleado = empleado.Cargo;
                    }

                    return planillaUI;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener planilla por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.DetallePlanilla> ObtenerDetallesPorPlanilla(int idPlanilla)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var detallesAD = contexto.DetallePlanilla
                        .Where(d => d.IdPlanilla == idPlanilla)
                        .OrderBy(d => d.Fecha)
                        .ToList();

                    return detallesAD.Select(d => ConvertirDetalleParaUI(d)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener detalles de planilla: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Planilla ConvertirObjetoParaUI(PlanillaAD planillaAD)
        {
            return new Abstracciones.ModeloUI.Planilla
            {
                IdPlanilla = planillaAD.IdPlanilla,
                IdEmpleado = planillaAD.IdEmpleado,
                PeriodoInicio = planillaAD.PeriodoInicio,
                PeriodoFin = planillaAD.PeriodoFin,
                TarifaHora = planillaAD.TarifaHora,
                TotalHorasRegulares = planillaAD.TotalHorasRegulares,
                TotalHorasExtras = planillaAD.TotalHorasExtras,
                PagoHorasRegulares = planillaAD.PagoHorasRegulares,
                PagoHorasExtras = planillaAD.PagoHorasExtras,
                PagoBruto = planillaAD.PagoBruto,
                DeduccionCCSS = planillaAD.DeduccionCCSS,
                ImpuestoRenta = planillaAD.ImpuestoRenta,
                OtrasDeducciones = planillaAD.OtrasDeducciones,
                TotalDeducciones = planillaAD.TotalDeducciones,
                PagoNeto = planillaAD.PagoNeto,
                Estado = planillaAD.Estado,
                NombreEmpleado = planillaAD.Empleado != null
                    ? $"{planillaAD.Empleado.Nombre} {planillaAD.Empleado.Apellidos}"
                    : "Empleado no encontrado",
                IdentificacionEmpleado = planillaAD.Empleado?.Identificacion ?? "N/A",
                CargoEmpleado = planillaAD.Empleado?.Cargo ?? "N/A"
            };
        }

        private Abstracciones.ModeloUI.DetallePlanilla ConvertirDetalleParaUI(DetallePlanillaAD detalleAD)
        {
            return new Abstracciones.ModeloUI.DetallePlanilla
            {
                IdDetallePlanilla = detalleAD.IdDetallePlanilla,
                IdPlanilla = detalleAD.IdPlanilla,
                IdAsistencia = detalleAD.IdAsistencia,
                Fecha = detalleAD.Fecha,
                HoraEntrada = detalleAD.HoraEntrada,
                HoraSalida = detalleAD.HoraSalida,
                TiempoAlmuerzo = detalleAD.TiempoAlmuerzo,
                HorasRegulares = detalleAD.HorasRegulares,
                HorasExtras = detalleAD.HorasExtras,
                Subtotal = detalleAD.Subtotal
            };
        }
    }
}