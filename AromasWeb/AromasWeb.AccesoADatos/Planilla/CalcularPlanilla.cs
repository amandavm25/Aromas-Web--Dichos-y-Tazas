using System;
using System.Linq;
using AromasWeb.Abstracciones.Logica.Planilla;
using AromasWeb.AccesoADatos.Modelos;

namespace AromasWeb.AccesoADatos.Planilla
{
    public class CalcularPlanilla : ICalcularPlanilla
    {
        public int CalcularYGuardar(int idEmpleado, DateTime periodoInicio, DateTime periodoFin)
        {
            using (var contexto = new Contexto())
            {
                // ===============================
                // VALIDACIONES INICIALES
                // ===============================
                if (periodoInicio > periodoFin)
                    throw new InvalidOperationException("La fecha de inicio debe ser anterior a la fecha de fin");

                var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == idEmpleado);
                if (empleado == null)
                    throw new InvalidOperationException("Empleado no encontrado");

                decimal tarifaHora = empleado.TarifaHora;
                if (tarifaHora <= 0)
                    throw new InvalidOperationException("La tarifa por hora debe ser mayor a cero");

                var asistencias = contexto.Asistencia
                    .Where(a => a.IdEmpleado == idEmpleado
                             && a.Fecha >= periodoInicio.Date
                             && a.Fecha <= periodoFin.Date)
                    .OrderBy(a => a.Fecha)
                    .ToList();

                if (!asistencias.Any())
                    throw new InvalidOperationException("No hay asistencias registradas en el período seleccionado.");

                // ===============================
                // CREAR PLANILLA
                // ===============================
                var planillaAD = new PlanillaAD
                {
                    IdEmpleado = idEmpleado,
                    PeriodoInicio = periodoInicio.Date,
                    PeriodoFin = periodoFin.Date,
                    TarifaHora = tarifaHora,
                    Estado = "Calculado",

                    TotalHorasRegulares = 0,
                    TotalHorasExtras = 0,
                    PagoHorasRegulares = 0,
                    PagoHorasExtras = 0,
                    PagoBruto = 0,
                    DeduccionCCSS = 0,
                    ImpuestoRenta = 0,
                    OtrasDeducciones = 0,
                    TotalDeducciones = 0,
                    PagoNeto = 0
                };

                contexto.Planilla.Add(planillaAD);
                contexto.SaveChanges(); // genera IdPlanilla

                decimal totalHorasRegulares = 0m;
                decimal totalHorasExtras = 0m;

                // ===============================
                // PROCESAR ASISTENCIAS
                // ===============================
                foreach (var a in asistencias)
                {
                    var almuerzo = TimeSpan.FromMinutes(a.TiempoAlmuerzo);
                    
                    if(!a.HoraSalida.HasValue)
                        throw new InvalidOperationException($"La asistencia del {a.Fecha:dd/MM/yyyy} no tiene hora de salida registrada.");

                    var horasDia = (a.HoraSalida.Value - a.HoraEntrada) - almuerzo;

                    if (horasDia < TimeSpan.Zero)
                        horasDia = TimeSpan.Zero;

                    decimal horasTotalesDia = (decimal)horasDia.TotalHours;

                    decimal horasRegulares = horasTotalesDia <= 8m ? horasTotalesDia : 8m;
                    decimal horasExtras = horasTotalesDia > 8m ? horasTotalesDia - 8m : 0m;

                    totalHorasRegulares += horasRegulares;
                    totalHorasExtras += horasExtras;

                    decimal subtotalDia =
                        (horasRegulares * tarifaHora) +
                        (horasExtras * tarifaHora * 1.5m);

                    var detalle = new DetallePlanillaAD
                    {
                        IdPlanilla = planillaAD.IdPlanilla,
                        IdAsistencia = a.IdAsistencia,
                        Fecha = a.Fecha,
                        HorasRegulares = Math.Round(horasRegulares, 2),
                        HorasExtras = Math.Round(horasExtras, 2),
                        Subtotal = Math.Round(subtotalDia, 2)
                    };

                    contexto.DetallePlanilla.Add(detalle);
                }

                // ===============================
                // CÁLCULOS FINALES
                // ===============================
                var pagoHorasRegulares = totalHorasRegulares * tarifaHora;
                var pagoHorasExtras = totalHorasExtras * tarifaHora * 1.5m;
                var pagoBruto = pagoHorasRegulares + pagoHorasExtras;

                var deduccionCcss = Math.Round(pagoBruto * 0.1067m, 2);
                var impuestoRenta = CalcularImpuestoRentaCR(pagoBruto);

                var otrasDeducciones = 0m;
                var totalDeducciones = deduccionCcss + impuestoRenta + otrasDeducciones;
                var pagoNeto = pagoBruto - totalDeducciones;

                planillaAD.TotalHorasRegulares = Math.Round(totalHorasRegulares, 2);
                planillaAD.TotalHorasExtras = Math.Round(totalHorasExtras, 2);
                planillaAD.PagoHorasRegulares = Math.Round(pagoHorasRegulares, 2);
                planillaAD.PagoHorasExtras = Math.Round(pagoHorasExtras, 2);
                planillaAD.PagoBruto = Math.Round(pagoBruto, 2);
                planillaAD.DeduccionCCSS = deduccionCcss;
                planillaAD.ImpuestoRenta = impuestoRenta;
                planillaAD.OtrasDeducciones = otrasDeducciones;
                planillaAD.TotalDeducciones = totalDeducciones;
                planillaAD.PagoNeto = pagoNeto;

                contexto.SaveChanges();

                return planillaAD.IdPlanilla;
            }
        }

        private decimal CalcularImpuestoRentaCR(decimal salarioBruto)
        {
            decimal impuesto;

            if (salarioBruto <= 941000m)
                impuesto = 0m;
            else if (salarioBruto <= 1381000m)
                impuesto = (salarioBruto - 941000m) * 0.10m;
            else if (salarioBruto <= 2423000m)
                impuesto = (440000m * 0.10m) +
                           ((salarioBruto - 1381000m) * 0.15m);
            else if (salarioBruto <= 4845000m)
                impuesto = (440000m * 0.10m) +
                           (1042000m * 0.15m) +
                           ((salarioBruto - 2423000m) * 0.20m);
            else
                impuesto = (440000m * 0.10m) +
                           (1042000m * 0.15m) +
                           (2422000m * 0.20m) +
                           ((salarioBruto - 4845000m) * 0.25m);

            return Math.Round(impuesto, 2);
        }
    }
}
