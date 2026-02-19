using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.AccesoADatos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AromasWeb.Controllers
{
    public class SolicitudVacacionesController : Controller
    {
        private IListarSolicitudesVacaciones _listarSolicitudesVacaciones;
        private ICrearSolicitudVacaciones _crearSolicitudVacaciones;
        private IActualizarSolicitudVacaciones _actualizarSolicitudVacaciones;
        private IObtenerSolicitudVacaciones _obtenerSolicitudVacaciones;

        public SolicitudVacacionesController()
        {
            _listarSolicitudesVacaciones = new LogicaDeNegocio.SolicitudesVacaciones.ListarSolicitudesVacaciones();
            _crearSolicitudVacaciones = new LogicaDeNegocio.SolicitudesVacaciones.CrearSolicitudVacaciones();
            _actualizarSolicitudVacaciones = new LogicaDeNegocio.SolicitudesVacaciones.ActualizarSolicitudVacaciones();
            _obtenerSolicitudVacaciones = new LogicaDeNegocio.SolicitudesVacaciones.ObtenerSolicitudVacaciones();
        }

        // Empleado

        // GET: SolicitudVacaciones/MisSolicitudes
        public IActionResult MisSolicitudes()
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 1;
            ViewBag.IdEmpleado = idEmpleado;
            CargarDatosEmpleadoEnViewBag(idEmpleado);

            var misSolicitudes = _listarSolicitudesVacaciones.BuscarPorEmpleado(idEmpleado);
            return View(misSolicitudes);
        }

        // GET: SolicitudVacaciones/SolicitarVacacionesEmpleado
        public IActionResult SolicitarVacacionesEmpleado()
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 1;
            CargarDatosEmpleadoEnViewBag(idEmpleado);

            var model = new SolicitudVacaciones
            {
                IdEmpleado = idEmpleado,
                FechaSolicitud = DateTime.Now.Date,
                FechaInicio = DateTime.Now.AddDays(1).Date,
                FechaFin = DateTime.Now.AddDays(1).Date,
                Estado = "Pendiente"
            };

            return View("RegistrarSolicitud", model);
        }

        // POST: SolicitudVacaciones/SolicitarVacacionesEmpleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SolicitarVacacionesEmpleado(SolicitudVacaciones solicitud)
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 1;

            try
            {
                ModelState.Remove("NombreEmpleado");
                ModelState.Remove("IdentificacionEmpleado");
                ModelState.Remove("CargoEmpleado");
                ModelState.Remove("FechaContratacionEmpleado");
                ModelState.Remove("DiasDisponibles");
                ModelState.Remove("DiasTomados");
                ModelState.Remove("FechaRespuesta");

                if (solicitud.FechaInicio > solicitud.FechaFin)
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor a la fecha de inicio");

                if (solicitud.DiasSolicitados <= 0)
                    ModelState.AddModelError("DiasSolicitados", "Debe seleccionar al menos un día");

                if (!ModelState.IsValid)
                {
                    CargarDatosEmpleadoEnViewBag(idEmpleado);
                    return View("RegistrarSolicitud", solicitud);
                }

                // Forzar siempre el empleado de sesión — el empleado no puede solicitar por otro
                solicitud.IdEmpleado = idEmpleado;
                solicitud.FechaSolicitud = DateTime.Now.Date;
                solicitud.Estado = "Pendiente";

                int resultado = await _crearSolicitudVacaciones.Crear(solicitud);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Solicitud de vacaciones registrada correctamente";
                    return RedirectToAction(nameof(MisSolicitudes));
                }

                ModelState.AddModelError("", "No se pudo registrar la solicitud en la base de datos");
                CargarDatosEmpleadoEnViewBag(idEmpleado);
                return View("RegistrarSolicitud", solicitud);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al registrar la solicitud: {ex.Message}");
                CargarDatosEmpleadoEnViewBag(idEmpleado);
                return View("RegistrarSolicitud", solicitud);
            }
        }

        // POST: SolicitudVacaciones/CancelarSolicitud/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarSolicitud(int id)
        {
            try
            {
                var solicitud = _listarSolicitudesVacaciones.ObtenerPorId(id);

                if (solicitud == null)
                {
                    TempData["Error"] = "Solicitud no encontrada";
                    return RedirectToAction(nameof(MisSolicitudes));
                }

                if (solicitud.Estado != "Pendiente")
                {
                    TempData["Error"] = "Solo se pueden cancelar solicitudes en estado Pendiente";
                    return RedirectToAction(nameof(MisSolicitudes));
                }

                int resultado = _actualizarSolicitudVacaciones.ActualizarEstado(id, "Cancelada");

                TempData[resultado > 0 ? "Mensaje" : "Error"] =
                    resultado > 0 ? "Solicitud cancelada correctamente" : "No se pudo cancelar la solicitud";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cancelar la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(MisSolicitudes));
        }

        // Administrador

        // GET: SolicitudVacaciones/VerSolicitudesEmpleado/5
        public IActionResult VerSolicitudesEmpleado(int id)
        {
            ViewBag.IdEmpleado = id;

            var empleado = ObtenerDatosEmpleadoPorId(id);
            ViewBag.Empleado = empleado;

            if (empleado != null)
            {
                var meses = (int)((DateTime.Now - empleado.FechaContratacion).TotalDays / 30);
                ViewBag.MesesTrabajados = meses;
                ViewBag.DiasAcumulados = meses;

                decimal diasTomados = 0;
                using (var ctx = new Contexto())
                {
                    diasTomados = ctx.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == id && s.Estado == "Aprobada")
                        .Sum(s => (decimal?)s.DiasSolicitados) ?? 0;
                }
                ViewBag.DiasTomados = diasTomados;
                ViewBag.DiasDisponibles = meses - diasTomados;
            }

            var solicitudes = _listarSolicitudesVacaciones.BuscarPorEmpleado(id);
            return View(solicitudes);
        }

        // GET: SolicitudVacaciones/ListadoSolicitudes
        public IActionResult ListadoSolicitudes(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            List<SolicitudVacaciones> solicitudes;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                solicitudes = _listarSolicitudesVacaciones.BuscarPorEstado(filtroEstado)
                    .Where(s => s.NombreEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                                s.IdentificacionEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                solicitudes = _listarSolicitudesVacaciones.BuscarPorEstado(filtroEstado);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                solicitudes = _listarSolicitudesVacaciones.Obtener()
                    .Where(s => s.NombreEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                                s.IdentificacionEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                solicitudes = _listarSolicitudesVacaciones.Obtener();
            }

            return View(solicitudes);
        }

        // GET: SolicitudVacaciones/CrearSolicitud
        public IActionResult CrearSolicitud()
        {
            CargarEmpleados();

            var model = new SolicitudVacaciones
            {
                FechaSolicitud = DateTime.Now.Date,
                FechaInicio = DateTime.Now.AddDays(1).Date,
                FechaFin = DateTime.Now.AddDays(1).Date,
                Estado = "Pendiente"
            };

            return View(model);
        }

        // POST: SolicitudVacaciones/CrearSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearSolicitud(SolicitudVacaciones solicitud)
        {
            try
            {
                ModelState.Remove("NombreEmpleado");
                ModelState.Remove("IdentificacionEmpleado");
                ModelState.Remove("CargoEmpleado");
                ModelState.Remove("FechaContratacionEmpleado");
                ModelState.Remove("DiasDisponibles");
                ModelState.Remove("DiasTomados");
                ModelState.Remove("FechaRespuesta");

                if (solicitud.FechaInicio > solicitud.FechaFin)
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor a la fecha de inicio");

                if (solicitud.DiasSolicitados <= 0)
                    ModelState.AddModelError("DiasSolicitados", "Debe seleccionar al menos un día");

                if (!ModelState.IsValid)
                {
                    CargarEmpleados();
                    return View(solicitud);
                }

                solicitud.FechaSolicitud = DateTime.Now.Date;
                solicitud.Estado = "Pendiente";

                int resultado = await _crearSolicitudVacaciones.Crear(solicitud);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Solicitud de vacaciones registrada correctamente";
                    return RedirectToAction(nameof(ListadoSolicitudes));
                }

                ModelState.AddModelError("", "No se pudo registrar la solicitud en la base de datos");
                CargarEmpleados();
                return View(solicitud);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al registrar la solicitud: {ex.Message}");
                CargarEmpleados();
                return View(solicitud);
            }
        }

        // GET: SolicitudVacaciones/EditarSolicitud/5
        public IActionResult EditarSolicitud(int id)
        {
            try
            {
                var solicitud = _listarSolicitudesVacaciones.ObtenerPorId(id);

                if (solicitud == null)
                {
                    TempData["Error"] = "Solicitud no encontrada";
                    return RedirectToAction(nameof(ListadoSolicitudes));
                }

                CargarEmpleados();
                return View(solicitud);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la solicitud: {ex.Message}";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }
        }

        // POST: SolicitudVacaciones/EditarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarSolicitud(SolicitudVacaciones solicitud)
        {
            try
            {
                ModelState.Remove("NombreEmpleado");
                ModelState.Remove("IdentificacionEmpleado");
                ModelState.Remove("CargoEmpleado");
                ModelState.Remove("FechaContratacionEmpleado");
                ModelState.Remove("DiasDisponibles");
                ModelState.Remove("DiasTomados");
                ModelState.Remove("FechaRespuesta");

                if (solicitud.FechaInicio > solicitud.FechaFin)
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor a la fecha de inicio");

                if (solicitud.DiasSolicitados <= 0)
                    ModelState.AddModelError("DiasSolicitados", "Debe seleccionar al menos un día");

                if (!ModelState.IsValid)
                {
                    CargarEmpleados();
                    return View(solicitud);
                }

                int resultado = _actualizarSolicitudVacaciones.Actualizar(solicitud);

                if (resultado > 0)
                {
                    return RedirectToAction(nameof(ListadoSolicitudes));
                }

                ModelState.AddModelError("", "No se pudo actualizar la solicitud en la base de datos");
                CargarEmpleados();
                return View(solicitud);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar la solicitud: {ex.Message}");
                CargarEmpleados();
                return View(solicitud);
            }
        }

        // POST: SolicitudVacaciones/AprobarSolicitud/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AprobarSolicitud(int id)
        {
            try
            {
                int resultado = _actualizarSolicitudVacaciones.ActualizarEstado(id, "Aprobada");

                TempData[resultado > 0 ? "Mensaje" : "Error"] =
                    resultado > 0 ? "Solicitud aprobada correctamente" : "No se pudo aprobar la solicitud";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al aprobar la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // POST: SolicitudVacaciones/RechazarSolicitud/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RechazarSolicitud(int id)
        {
            try
            {
                int resultado = _actualizarSolicitudVacaciones.ActualizarEstado(id, "Rechazada");

                TempData[resultado > 0 ? "Mensaje" : "Error"] =
                    resultado > 0 ? "Solicitud rechazada correctamente" : "No se pudo rechazar la solicitud";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al rechazar la solicitud: {ex.Message}";
            }

            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // Cargar datos del empleado
        private void CargarDatosEmpleadoEnViewBag(int idEmpleado)
        {
            var empleado = ObtenerDatosEmpleadoPorId(idEmpleado);

            if (empleado != null)
            {
                ViewBag.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                ViewBag.CargoEmpleado = empleado.Cargo;

                var meses = (int)((DateTime.Now - empleado.FechaContratacion).TotalDays / 30);
                ViewBag.MesesTrabajados = meses;
                ViewBag.DiasAcumulados = meses;

                decimal diasTomados = 0;
                using (var ctx = new Contexto())
                {
                    diasTomados = ctx.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == idEmpleado && s.Estado == "Aprobada")
                        .Sum(s => (decimal?)s.DiasSolicitados) ?? 0;
                }
                ViewBag.DiasTomados = diasTomados;
                ViewBag.DiasDisponibles = meses - diasTomados;
            }
            else
            {
                ViewBag.NombreEmpleado = "Empleado";
                ViewBag.CargoEmpleado = "N/A";
                ViewBag.MesesTrabajados = 0;
                ViewBag.DiasAcumulados = 0;
                ViewBag.DiasTomados = 0;
                ViewBag.DiasDisponibles = 0;
            }
        }

        private Empleado ObtenerDatosEmpleadoPorId(int id)
        {
            using (var ctx = new Contexto())
            {
                try
                {
                    var e = ctx.Empleado.FirstOrDefault(emp => emp.IdEmpleado == id);
                    if (e == null) return null;

                    return new Empleado
                    {
                        IdEmpleado = e.IdEmpleado,
                        IdRol = e.IdRol,
                        Identificacion = e.Identificacion,
                        Nombre = e.Nombre,
                        Apellidos = e.Apellidos,
                        Correo = e.Correo,
                        Telefono = e.Telefono,
                        Cargo = e.Cargo,
                        FechaContratacion = e.FechaContratacion,
                        Estado = e.Estado
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
            using (var ctx = new Contexto())
            {
                try
                {
                    // Se incluye FechaContratacion y DiasDisponibles calculados
                    ViewBag.Empleados = ctx.Empleado
                        .Where(e => e.Estado == true)
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .Select(e => new
                        {
                            IdEmpleado = e.IdEmpleado,
                            NombreCompleto = e.Nombre + " " + e.Apellidos,
                            Cargo = e.Cargo,
                            FechaContratacion = e.FechaContratacion,
                            DiasDisponibles =
                                (decimal)(int)((DateTime.Now - e.FechaContratacion).TotalDays / 30)
                                - (ctx.SolicitudVacaciones
                                    .Where(s => s.IdEmpleado == e.IdEmpleado && s.Estado == "Aprobada")
                                    .Sum(s => (decimal?)s.DiasSolicitados) ?? 0)
                        })
                        .ToList();
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