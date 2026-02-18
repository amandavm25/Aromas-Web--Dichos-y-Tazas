using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Planilla;
using AromasWeb.AccesoADatos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class PlanillaController : Controller
    {
        private IListarPlanillas _listarPlanillas;

        public PlanillaController()
        {
            _listarPlanillas = new LogicaDeNegocio.Planillas.ListarPlanillas();
        }

        // GET: Planilla/ListadoPlanillas
        public IActionResult ListadoPlanillas(int? empleado, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            // Cargar empleados para el filtro
            CargarEmpleados();

            ViewBag.EmpleadoFiltro = empleado;
            ViewBag.EstadoFiltro = estado;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // Obtener planillas según los filtros
            List<Planilla> planillas;

            if (empleado.HasValue && !string.IsNullOrEmpty(estado) && fechaInicio.HasValue && fechaFin.HasValue)
            {
                planillas = _listarPlanillas.ObtenerPorEmpleado(empleado.Value)
                    .Where(p => p.Estado.ToLower() == estado.ToLower()
                             && p.PeriodoInicio >= fechaInicio.Value
                             && p.PeriodoFin <= fechaFin.Value)
                    .ToList();
            }
            else if (empleado.HasValue && !string.IsNullOrEmpty(estado))
            {
                planillas = _listarPlanillas.ObtenerPorEmpleado(empleado.Value)
                    .Where(p => p.Estado.ToLower() == estado.ToLower())
                    .ToList();
            }
            else if (empleado.HasValue && fechaInicio.HasValue && fechaFin.HasValue)
            {
                planillas = _listarPlanillas.ObtenerPorEmpleado(empleado.Value)
                    .Where(p => p.PeriodoInicio >= fechaInicio.Value && p.PeriodoFin <= fechaFin.Value)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(estado) && fechaInicio.HasValue && fechaFin.HasValue)
            {
                planillas = _listarPlanillas.ObtenerPorPeriodo(fechaInicio.Value, fechaFin.Value)
                    .Where(p => p.Estado.ToLower() == estado.ToLower())
                    .ToList();
            }
            else if (empleado.HasValue)
            {
                planillas = _listarPlanillas.ObtenerPorEmpleado(empleado.Value);
            }
            else if (!string.IsNullOrEmpty(estado))
            {
                planillas = _listarPlanillas.ObtenerPorEstado(estado);
            }
            else if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                planillas = _listarPlanillas.ObtenerPorPeriodo(fechaInicio.Value, fechaFin.Value);
            }
            else
            {
                planillas = _listarPlanillas.Obtener();
            }

            // Calcular resumen dinámico
            ViewBag.TotalPlanillas = planillas.Count;
            ViewBag.PlanillasPagadas = planillas.Count(p => p.Estado.Equals("Pagado", StringComparison.OrdinalIgnoreCase));
            ViewBag.PlanillasPendientes = planillas.Count(p => p.Estado.Equals("Calculado", StringComparison.OrdinalIgnoreCase));
            ViewBag.TotalAPagar = planillas
                .Where(p => p.Estado.Equals("Calculado", StringComparison.OrdinalIgnoreCase))
                .Sum(p => p.PagoNeto);

            return View(planillas);
        }

        // GET: Planilla/CalcularPlanilla
        public IActionResult CalcularPlanilla()
        {
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

            // Aquí iría la lógica para calcular la planilla y guardarla en la base de datos
            TempData["Mensaje"] = "Planilla calculada correctamente";
            return RedirectToAction(nameof(VerDetallePlanilla), new { id = 1 });
        }

        // GET: Planilla/VerDetallePlanilla/1
        public IActionResult VerDetallePlanilla(int id)
        {
            ViewBag.IdPlanilla = id;

            var planilla = _listarPlanillas.ObtenerPorId(id);

            if (planilla == null)
            {
                TempData["Error"] = "Planilla no encontrada";
                return RedirectToAction(nameof(ListadoPlanillas));
            }

            ViewBag.Planilla = planilla;

            var detalles = _listarPlanillas.ObtenerDetallesPorPlanilla(id);

            return View(detalles);
        }

        // POST: Planilla/MarcarComoPagado/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoPagado(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var planilla = contexto.Planilla.FirstOrDefault(p => p.IdPlanilla == id);
                    if (planilla == null)
                    {
                        TempData["Error"] = "Planilla no encontrada";
                        return RedirectToAction(nameof(ListadoPlanillas));
                    }
                    if (planilla.Estado != null && planilla.Estado.Equals("Pagado", StringComparison.OrdinalIgnoreCase)

                      {
                        TempData["Error"] = "La planilla ya está marcada como pagada";
                        return RedirectToAction(nameof(ListadoPlanillas));
                    }

                    planilla.Estado = "Pagado";
                    contexto.SaveChanges();


                }
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
            int idEmpleadoActual = 1;
            ViewBag.IdEmpleado = idEmpleadoActual;

            var empleadoInfo = ObtenerDatosEmpleadoPorId(idEmpleadoActual);
            ViewBag.EmpleadoInfo = empleadoInfo;

            var misPlanillas = _listarPlanillas.ObtenerPorEmpleado(idEmpleadoActual);

            return View(misPlanillas);
        }

        // GET: Planilla/PlanillasEmpleado/1
        public IActionResult PlanillasEmpleado(int id, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.IdEmpleado = id;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // CORREGIDO: Ahora retorna Empleado de UI
            var empleado = ObtenerDatosEmpleadoPorId(id);
            ViewBag.Empleado = empleado;

            var planillas = _listarPlanillas.ObtenerPorEmpleado(id);

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                planillas = planillas
                    .Where(p => p.PeriodoInicio >= fechaInicio.Value && p.PeriodoFin <= fechaFin.Value)
                    .ToList();
            }

            return View(planillas);
        }

        // MÉTODO CORREGIDO: Ahora retorna Empleado de UI en lugar de dynamic
        private Empleado ObtenerDatosEmpleadoPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var empleadoAD = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == id);

                    if (empleadoAD == null)
                        return null;

                    // Convertir de AccesoADatos.Modelos.EmpleadoAD a ModeloUI.Empleado
                    return new Empleado
                    {
                        IdEmpleado = empleadoAD.IdEmpleado,
                        IdRol = empleadoAD.IdRol,
                        Identificacion = empleadoAD.Identificacion,
                        Nombre = empleadoAD.Nombre,
                        Apellidos = empleadoAD.Apellidos,
                        Correo = empleadoAD.Correo,
                        Telefono = empleadoAD.Telefono,
                        Cargo = empleadoAD.Cargo,
                        FechaContratacion = empleadoAD.FechaContratacion,
                        Estado = empleadoAD.Estado
                        // Las propiedades calculadas se generan automáticamente:
                        // - AnosTrabajados
                        // - FechaContratacionFormateada
                        // - EstadoTexto
                        // - MesesTrabajados
                        // - EsEmpleadoAntiguo
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener datos del empleado: {ex.Message}");
                    return null;
                }
            }
        }

        // Cargar empleados desde la base de datos
        private void CargarEmpleados()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var empleados = contexto.Empleado
                        .Where(e => e.Estado == true)
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .Select(e => new
                        {
                            IdEmpleado = e.IdEmpleado,
                            NombreCompleto = e.Nombre + " " + e.Apellidos,
                            Cargo = e.Cargo
                        })
                        .ToList();

                    ViewBag.Empleados = empleados;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar empleados: {ex.Message}");
                    ViewBag.Empleados = new List<dynamic>();
                }
            }
        }
    }
}