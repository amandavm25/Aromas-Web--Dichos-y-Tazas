
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
                    var inicioUtc = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);
                    var finUtc = DateTime.SpecifyKind(fechaFin.Date.AddDays(1), DateTimeKind.Utc);

                    var planillasAD = contexto.Planilla
                        .Where(p => p.PeriodoInicio >= inicioUtc && p.PeriodoFin <= finUtc)
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
                    // JOIN con la tabla Asistencia para obtener HoraEntrada, HoraSalida y TiempoAlmuerzo
                    var detallesConAsistencia = contexto.DetallePlanilla
                        .Where(d => d.IdPlanilla == idPlanilla)
                        .Join(
                            contexto.Asistencia,
                            detalle => detalle.IdAsistencia,
                            asistencia => asistencia.IdAsistencia,
                            (detalle, asistencia) => new
                            {
                                Detalle = detalle,
                                Asistencia = asistencia
                            }
                        )
                        .OrderBy(x => x.Detalle.Fecha)
                        .ToList();

                    var detallesUI = new List<Abstracciones.ModeloUI.DetallePlanilla>();

                    foreach (var item in detallesConAsistencia)
                    {
                        var detalleUI = new Abstracciones.ModeloUI.DetallePlanilla
                        {
                            IdDetallePlanilla = item.Detalle.IdDetallePlanilla,
                            IdPlanilla = item.Detalle.IdPlanilla,
                            IdAsistencia = item.Detalle.IdAsistencia,
                            Fecha = item.Detalle.Fecha,
                            HorasRegulares = item.Detalle.HorasRegulares,
                            HorasExtras = item.Detalle.HorasExtras,
                            Subtotal = item.Detalle.Subtotal,
                            // Obtener datos de la asistencia
                            HoraEntrada = item.Asistencia.HoraEntrada,
                            HoraSalida = item.Asistencia.HoraSalida,
                            TiempoAlmuerzo = item.Asistencia.TiempoAlmuerzo > 0
                                ? TimeSpan.FromMinutes(item.Asistencia.TiempoAlmuerzo)
                                : (TimeSpan?)null
                        };

                        detallesUI.Add(detalleUI);
                    }

                    return detallesUI;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener detalles de planilla: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }
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

        public void MarcarComoPagado(int idPlanilla)
        {
            using (var contexto = new Contexto())
            {
                var planilla = contexto.Planilla.FirstOrDefault(p => p.IdPlanilla == idPlanilla);

                if (planilla == null)
                    throw new Exception("La planilla no existe");

                if (planilla.Estado == "Pagado")
                    throw new Exception("La planilla ya se encuentra pagada");

                planilla.Estado = "Pagado";
                contexto.SaveChanges();
            }
        }
    }
}
