using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class PlanillaController : Controller
    {
        // GET: Planilla/ListadoPlanillas
        public IActionResult ListadoPlanillas(int? empleado, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.EmpleadoFiltro = empleado;
            ViewBag.EstadoFiltro = estado;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // Datos quemados de planillas
            var planillas = new List<Planilla>();

            return View(planillas);
        }

        // GET: Planilla/CalcularPlanilla
        public IActionResult CalcularPlanilla()
        {
            // Cargar empleados para el dropdown
            CargarEmpleados();

            var nuevaPlanilla = new Planilla
            {
                PeriodoInicio = DateTime.Today.AddDays(-14),
                PeriodoFin = DateTime.Today,
                Estado = "Calculado"
            };

            return View(nuevaPlanilla);
        }

        // POST: Planilla/CalcularPlanilla
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CalcularPlanilla(int idEmpleado, DateTime periodoInicio, DateTime periodoFin)
        {
            if (periodoInicio >= periodoFin)
            {
                TempData["Error"] = "La fecha de inicio debe ser anterior a la fecha de fin";
                CargarEmpleados();
                return View();
            }

            // Simular cálculo de planilla
            TempData["Mensaje"] = "Planilla calculada correctamente";
            return RedirectToAction(nameof(VerDetallePlanilla), new { id = 1 });
        }

        // GET: Planilla/VerDetallePlanilla/1
        public IActionResult VerDetallePlanilla(int id)
        {
            ViewBag.IdPlanilla = id;

            // PLANILLA QUEMADA
            var planilla = new Planilla
            {
                IdPlanilla = 1,
                IdEmpleado = 1,
                NombreEmpleado = "Kenneth Salas",
                IdentificacionEmpleado = "1-2345-6789",
                CargoEmpleado = "Chef Principal",
                PeriodoInicio = new DateTime(2024, 11, 1),
                PeriodoFin = new DateTime(2024, 11, 15),
                TarifaHora = 2800.00m,
                TotalHorasRegulares = 120.00m,
                TotalHorasExtras = 8.00m,
                PagoHorasRegulares = 336000.00m,
                PagoHorasExtras = 33600.00m,
                PagoBruto = 369600.00m,
                DeduccionCCSS = 39436.32m,
                ImpuestoRenta = 0m,
                OtrasDeducciones = 0m,
                TotalDeducciones = 39436.32m,
                PagoNeto = 330163.68m,
                Estado = "Calculado"
            };

            ViewBag.Planilla = planilla;

            // DETALLES DIARIOS QUEMADOS
            var detalles = new List<DetallePlanilla>
            {
                new DetallePlanilla
                {
                    IdDetallePlanilla = 1,
                    IdPlanilla = 1,
                    Fecha = new DateTime(2024, 11, 1),
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(17, 0, 0),
                    TiempoAlmuerzo = new TimeSpan(1, 0, 0),
                    HorasRegulares = 8.00m,
                    HorasExtras = 0.00m,
                    Subtotal = 22400.00m
                },
                new DetallePlanilla
                {
                    IdDetallePlanilla = 2,
                    IdPlanilla = 1,
                    Fecha = new DateTime(2024, 11, 2),
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(17, 0, 0),
                    TiempoAlmuerzo = new TimeSpan(1, 0, 0),
                    HorasRegulares = 8.00m,
                    HorasExtras = 0.00m,
                    Subtotal = 22400.00m
                },
                new DetallePlanilla
                {
                    IdDetallePlanilla = 3,
                    IdPlanilla = 1,
                    Fecha = new DateTime(2024, 11, 4),
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(17, 0, 0),
                    TiempoAlmuerzo = new TimeSpan(1, 0, 0),
                    HorasRegulares = 8.00m,
                    HorasExtras = 0.00m,
                    Subtotal = 22400.00m
                },
                new DetallePlanilla
                {
                    IdDetallePlanilla = 4,
                    IdPlanilla = 1,
                    Fecha = new DateTime(2024, 11, 5),
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(18, 0, 0),
                    TiempoAlmuerzo = new TimeSpan(1, 0, 0),
                    HorasRegulares = 8.00m,
                    HorasExtras = 1.00m,
                    Subtotal = 26600.00m
                },
                new DetallePlanilla
                {
                    IdDetallePlanilla = 5,
                    IdPlanilla = 1,
                    Fecha = new DateTime(2024, 11, 6),
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(17, 0, 0),
                    TiempoAlmuerzo = new TimeSpan(1, 0, 0),
                    HorasRegulares = 8.00m,
                    HorasExtras = 0.00m,
                    Subtotal = 22400.00m
                }
            };

            return View(detalles);
        }

        // POST: Planilla/MarcarComoPagado/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoPagado(int id)
        {
            TempData["Mensaje"] = "Planilla marcada como pagada correctamente";
            return RedirectToAction(nameof(ListadoPlanillas));
        }

        // POST: Planilla/AnularPlanilla/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnularPlanilla(int id)
        {
            TempData["Mensaje"] = "Planilla anulada correctamente";
            return RedirectToAction(nameof(ListadoPlanillas));
        }

        // GET: Planilla/MiPlanilla
        public IActionResult MiPlanilla()
        {
            // Simulando el empleado actual (ID = 1 - Kenneth Salas)
            int idEmpleadoActual = 1;
            ViewBag.IdEmpleado = idEmpleadoActual;

            var empleadoInfo = new
            {
                IdEmpleado = 1,
                NombreCompleto = "Kenneth Salas",
                Identificacion = "1-2345-6789",
                Cargo = "Chef Principal",
                TarifaActual = 2800.00m
            };

            ViewBag.EmpleadoInfo = empleadoInfo;

            // MIS PLANILLAS QUEMADAS
            var misPlanillas = new List<Planilla>
            {
                new Planilla
                {
                    IdPlanilla = 1,
                    IdEmpleado = 1,
                    PeriodoInicio = new DateTime(2024, 11, 1),
                    PeriodoFin = new DateTime(2024, 11, 15),
                    TarifaHora = 2800.00m,
                    TotalHorasRegulares = 120.00m,
                    TotalHorasExtras = 8.00m,
                    PagoBruto = 369600.00m,
                    TotalDeducciones = 39436.32m,
                    PagoNeto = 330163.68m,
                    Estado = "Pagado"
                },
                new Planilla
                {
                    IdPlanilla = 2,
                    IdEmpleado = 1,
                    PeriodoInicio = new DateTime(2024, 10, 16),
                    PeriodoFin = new DateTime(2024, 10, 31),
                    TarifaHora = 2800.00m,
                    TotalHorasRegulares = 128.00m,
                    TotalHorasExtras = 6.00m,
                    PagoBruto = 383600.00m,
                    TotalDeducciones = 40930.12m,
                    PagoNeto = 342669.88m,
                    Estado = "Pagado"
                }
            };

            return View(misPlanillas);
        }

        // GET: Planilla/PlanillasEmpleado/1
        public IActionResult PlanillasEmpleado(int id, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.IdEmpleado = id;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // DATOS QUEMADOS DEL EMPLEADO
            var empleado = new EmpleadoInfo();

            if (id == 1)
            {
                empleado.IdEmpleado = 1;
                empleado.NombreCompleto = "Kenneth Salas";
                empleado.Identificacion = "1-2345-6789";
                empleado.Cargo = "Chef Principal";
                empleado.FechaContratacion = new DateTime(2023, 1, 15);
                empleado.TiempoServicio = "1 año 10 meses";
            }
            else if (id == 2)
            {
                empleado.IdEmpleado = 2;
                empleado.NombreCompleto = "María González";
                empleado.Identificacion = "2-3456-7890";
                empleado.Cargo = "Asistente de Cocina";
                empleado.FechaContratacion = new DateTime(2023, 3, 20);
                empleado.TiempoServicio = "1 año 8 meses";
            }
            else if (id == 3)
            {
                empleado.IdEmpleado = 3;
                empleado.NombreCompleto = "David Rodríguez";
                empleado.Identificacion = "3-4567-8901";
                empleado.Cargo = "Barista";
                empleado.FechaContratacion = new DateTime(2023, 2, 10);
                empleado.TiempoServicio = "1 año 9 meses";
            }
            else
            {
                empleado.IdEmpleado = 4;
                empleado.NombreCompleto = "Ana Jiménez";
                empleado.Identificacion = "4-5678-9012";
                empleado.Cargo = "Cajera";
                empleado.FechaContratacion = new DateTime(2023, 4, 5);
                empleado.TiempoServicio = "1 año 7 meses";
            }

            ViewBag.Empleado = empleado;

            // PLANILLAS QUEMADAS DEL EMPLEADO
            var planillas = new List<Planilla>();

            if (id == 1)
            {
                planillas.Add(new Planilla
                {
                    IdPlanilla = 1,
                    IdEmpleado = 1,
                    PeriodoInicio = new DateTime(2024, 11, 1),
                    PeriodoFin = new DateTime(2024, 11, 15),
                    TarifaHora = 2800.00m,
                    TotalHorasRegulares = 120.00m,
                    TotalHorasExtras = 8.00m,
                    PagoBruto = 369600.00m,
                    TotalDeducciones = 39436.32m,
                    PagoNeto = 330163.68m,
                    Estado = "Pagado"
                });
                planillas.Add(new Planilla
                {
                    IdPlanilla = 5,
                    IdEmpleado = 1,
                    PeriodoInicio = new DateTime(2024, 10, 16),
                    PeriodoFin = new DateTime(2024, 10, 31),
                    TarifaHora = 2800.00m,
                    TotalHorasRegulares = 128.00m,
                    TotalHorasExtras = 6.00m,
                    PagoBruto = 383600.00m,
                    TotalDeducciones = 40930.12m,
                    PagoNeto = 342669.88m,
                    Estado = "Pagado"
                });
            }
            else if (id == 2)
            {
                planillas.Add(new Planilla
                {
                    IdPlanilla = 2,
                    IdEmpleado = 2,
                    PeriodoInicio = new DateTime(2024, 11, 1),
                    PeriodoFin = new DateTime(2024, 11, 15),
                    TarifaHora = 1900.00m,
                    TotalHorasRegulares = 120.00m,
                    TotalHorasExtras = 4.00m,
                    PagoBruto = 239400.00m,
                    TotalDeducciones = 25539.48m,
                    PagoNeto = 213860.52m,
                    Estado = "Calculado"
                });
            }
            else if (id == 3)
            {
                planillas.Add(new Planilla
                {
                    IdPlanilla = 3,
                    IdEmpleado = 3,
                    PeriodoInicio = new DateTime(2024, 11, 1),
                    PeriodoFin = new DateTime(2024, 11, 15),
                    TarifaHora = 2400.00m,
                    TotalHorasRegulares = 116.00m,
                    TotalHorasExtras = 6.00m,
                    PagoBruto = 300000.00m,
                    TotalDeducciones = 32010.00m,
                    PagoNeto = 267990.00m,
                    Estado = "Calculado"
                });
            }
            else if (id == 4)
            {
                planillas.Add(new Planilla
                {
                    IdPlanilla = 4,
                    IdEmpleado = 4,
                    PeriodoInicio = new DateTime(2024, 10, 16),
                    PeriodoFin = new DateTime(2024, 10, 31),
                    TarifaHora = 1800.00m,
                    TotalHorasRegulares = 128.00m,
                    TotalHorasExtras = 2.00m,
                    PagoBruto = 235800.00m,
                    TotalDeducciones = 25160.86m,
                    PagoNeto = 210639.14m,
                    Estado = "Pagado"
                });
            }

            return View(planillas);
        }

        // Método auxiliar para cargar empleados
        private void CargarEmpleados()
        {
            var empleados = new List<dynamic>
            {
                new { IdEmpleado = 1, NombreCompleto = "Kenneth Salas", Cargo = "Chef Principal" },
                new { IdEmpleado = 2, NombreCompleto = "María González", Cargo = "Asistente de Cocina" },
                new { IdEmpleado = 3, NombreCompleto = "David Rodríguez", Cargo = "Barista" },
                new { IdEmpleado = 4, NombreCompleto = "Ana Jiménez", Cargo = "Cajera" }
            };

            ViewBag.Empleados = empleados;
        }
    }
}