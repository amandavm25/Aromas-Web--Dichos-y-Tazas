using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Planilla;
using AromasWeb.AccesoADatos;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AromasWeb.Controllers
{
    public class PlanillaController : Controller
    {
        private IListarPlanillas _listarPlanillas;
        private ICalcularPlanilla _calcularPlanilla;
        private readonly CrearBitacora _crearBitacora;

        public PlanillaController()
        {
            _listarPlanillas = new LogicaDeNegocio.Planillas.ListarPlanillas();
            _calcularPlanilla = new LogicaDeNegocio.Planillas.CalcularPlanilla();
            _crearBitacora = new CrearBitacora();
        }

        // Helper de sesión
        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;

            return 1;
        }

        // GET: Planilla/ListadoPlanillas
        public IActionResult ListadoPlanillas(int? empleado, string estado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            CargarEmpleados();

            ViewBag.EmpleadoFiltro = empleado;
            ViewBag.EstadoFiltro = estado;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

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

            try
            {
                int idPlanillaCreada = _calcularPlanilla.CalcularYGuardar(idEmpleado, periodoInicio, periodoFin);

                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de planilla"),
                    accion: Bitacora.Acciones.Calcular,
                    tablaAfectada: "Planilla",
                    descripcion: $"Se calculó la planilla del empleado ID {idEmpleado} — Período: {periodoInicio:dd/MM/yyyy} al {periodoFin:dd/MM/yyyy} (ID planilla: {idPlanillaCreada})",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        idEmpleado,
                        PeriodoInicio = periodoInicio.ToString("dd/MM/yyyy"),
                        PeriodoFin = periodoFin.ToString("dd/MM/yyyy"),
                        IdPlanillaCreada = idPlanillaCreada
                    })
                );

                TempData["Mensaje"] = "Planilla calculada correctamente";
                return RedirectToAction(nameof(VerDetallePlanilla), new { id = idPlanillaCreada });
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al calcular la planilla: " + ex.Message;
                CargarEmpleados();
                return View();
            }
        }

        // GET: Planilla/VerDetallePlanilla/1
        public IActionResult VerDetallePlanilla(int id, string returnUrl = null)
        {
            ViewBag.IdPlanilla = id;

            var planilla = _listarPlanillas.ObtenerPorId(id);

            if (planilla == null)
            {
                TempData["Error"] = "Planilla no encontrada";
                return RedirectToAction(nameof(ListadoPlanillas));
            }

            ViewBag.Planilla = planilla;

            if (string.IsNullOrEmpty(returnUrl))
            {
                var tipoUsuario = HttpContext.Session.GetString("UsuarioTipo");
                returnUrl = tipoUsuario == "empleado"
                    ? Url.Action(nameof(MiPlanilla), "Planilla")
                    : Url.Action(nameof(ListadoPlanillas), "Planilla");
            }

            ViewBag.ReturnUrl = returnUrl;

            var detalles = _listarPlanillas.ObtenerDetallesPorPlanilla(id);

            return View(detalles);
        }

        // POST: Planilla/MarcarComoPagado/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MarcarComoPagado(int IdPlanilla)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Capturar datos ANTES de marcar como pagado
                    var planilla = _listarPlanillas.ObtenerPorId(IdPlanilla);

                    _listarPlanillas.MarcarComoPagado(IdPlanilla);

                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de planilla"),
                        accion: Bitacora.Acciones.MarcarPagado,
                        tablaAfectada: "Planilla",
                        descripcion: $"Se marcó como pagada la planilla ID {IdPlanilla}" + (planilla != null ? $" del empleado ID {planilla.IdEmpleado} — Pago neto: ₡{planilla.PagoNeto:N2}" : ""),
                        datosAnteriores: planilla != null
                            ? JsonSerializer.Serialize(new { planilla.Estado })
                            : null,
                        datosNuevos: JsonSerializer.Serialize(new { Estado = "Pagado" })
                    );

                    TempData["Mensaje"] = "Planilla marcada como pagada correctamente";
                }
                catch (Exception ex)
                {
                    TempData["Error"] = "No se pudo marcar la planilla como pagada: " + ex.Message;
                }

                return RedirectToAction(nameof(ListadoPlanillas));
            }
        }

        // POST: Planilla/AnularPlanilla/1
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AnularPlanilla(int id)
        {
            // Aquí iría la lógica para anular la planilla
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

        private Empleado ObtenerDatosEmpleadoPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var empleadoAD = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == id);

                    if (empleadoAD == null)
                        return null;

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
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener datos del empleado: {ex.Message}");
                    return null;
                }
            }
        }

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