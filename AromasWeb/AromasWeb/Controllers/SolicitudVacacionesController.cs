using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.AccesoADatos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class SolicitudVacacionesController : Controller
    {
        private IListarSolicitudesVacaciones _listarSolicitudesVacaciones;

        public SolicitudVacacionesController()
        {
            _listarSolicitudesVacaciones = new LogicaDeNegocio.SolicitudesVacaciones.ListarSolicitudesVacaciones();
        }

        // GET: SolicitudVacaciones/MisSolicitudes
        public IActionResult MisSolicitudes()
        {
            // Obtener el ID del empleado de la sesión
            int idEmpleadoActual = HttpContext.Session.GetInt32("IdEmpleado") ?? 1;
            ViewBag.IdEmpleado = idEmpleadoActual;

            // Obtener información del empleado desde la base de datos
            var empleadoInfo = ObtenerDatosEmpleadoPorId(idEmpleadoActual);

            if (empleadoInfo != null)
            {
                ViewBag.NombreEmpleado = $"{empleadoInfo.Nombre} {empleadoInfo.Apellidos}";
                ViewBag.CargoEmpleado = empleadoInfo.Cargo;

                // Calcular meses trabajados
                var mesesTrabajados = (int)((DateTime.Now - empleadoInfo.FechaContratacion).TotalDays / 30);
                ViewBag.MesesTrabajados = mesesTrabajados;

                // Calcular días acumulados (1 día por mes trabajado)
                var diasAcumulados = mesesTrabajados;
                ViewBag.DiasAcumulados = diasAcumulados;

                // Calcular días tomados (solicitudes aprobadas)
                decimal diasTomados = 0;
                using (var contexto = new Contexto())
                {
                    diasTomados = contexto.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == idEmpleadoActual && s.Estado == "Aprobada")
                        .Sum(s => (decimal?)s.DiasSolicitados) ?? 0;
                }
                ViewBag.DiasTomados = diasTomados;

                // Calcular días disponibles
                var diasDisponibles = diasAcumulados - diasTomados;
                ViewBag.DiasDisponibles = diasDisponibles;
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

            // Obtener mis solicitudes
            var misSolicitudes = _listarSolicitudesVacaciones.BuscarPorEmpleado(idEmpleadoActual);

            return View(misSolicitudes);
        }

        // GET: SolicitudVacaciones/VerSolicitudesEmpleado/1
        public IActionResult VerSolicitudesEmpleado(int id)
        {
            ViewBag.IdEmpleado = id;

            // Obtener información del empleado
            var empleado = ObtenerDatosEmpleadoPorId(id);
            ViewBag.Empleado = empleado;

            if (empleado != null)
            {
                // Calcular meses trabajados
                var mesesTrabajados = (int)((DateTime.Now - empleado.FechaContratacion).TotalDays / 30);
                ViewBag.MesesTrabajados = mesesTrabajados;

                // Calcular días acumulados
                var diasAcumulados = mesesTrabajados;
                ViewBag.DiasAcumulados = diasAcumulados;

                // Calcular días tomados
                decimal diasTomados = 0;
                using (var contexto = new Contexto())
                {
                    diasTomados = contexto.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == id && s.Estado == "Aprobada")
                        .Sum(s => (decimal?)s.DiasSolicitados) ?? 0;
                }
                ViewBag.DiasTomados = diasTomados;

                // Calcular días disponibles
                var diasDisponibles = diasAcumulados - diasTomados;
                ViewBag.DiasDisponibles = diasDisponibles;
            }

            // Obtener solicitudes del empleado
            var solicitudes = _listarSolicitudesVacaciones.BuscarPorEmpleado(id);

            return View(solicitudes);
        }

        // GET: SolicitudVacaciones/ListadoSolicitudes
        public IActionResult ListadoSolicitudes(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Obtener solicitudes según los filtros
            List<SolicitudVacaciones> solicitudes;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                // Buscar por estado y filtrar por nombre
                solicitudes = _listarSolicitudesVacaciones.BuscarPorEstado(filtroEstado)
                    .Where(s => s.NombreEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                                s.IdentificacionEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                // Solo filtrar por estado
                solicitudes = _listarSolicitudesVacaciones.BuscarPorEstado(filtroEstado);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre
                solicitudes = _listarSolicitudesVacaciones.Obtener()
                    .Where(s => s.NombreEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                                s.IdentificacionEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            else
            {
                // Obtener todas
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
                FechaInicio = DateTime.Now.AddDays(7).Date,
                Estado = "Pendiente"
            };

            return View(model);
        }

        // POST: SolicitudVacaciones/CrearSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearSolicitud(SolicitudVacaciones solicitud)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Solicitud de vacaciones registrada correctamente";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            CargarEmpleados();
            return View(solicitud);
        }

        // GET: SolicitudVacaciones/EditarSolicitud/5
        public IActionResult EditarSolicitud(int id)
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

        // POST: SolicitudVacaciones/EditarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarSolicitud(SolicitudVacaciones solicitud)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Solicitud actualizada correctamente";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            CargarEmpleados();
            return View(solicitud);
        }

        // POST: SolicitudVacaciones/EliminarSolicitud/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarSolicitud(int id)
        {
            // Aquí iría la lógica para eliminar la solicitud
            TempData["Mensaje"] = "Solicitud eliminada correctamente";
            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // POST: SolicitudVacaciones/AprobarSolicitud/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AprobarSolicitud(int id)
        {
            // Aquí iría la lógica para aprobar la solicitud
            TempData["Mensaje"] = "Solicitud aprobada correctamente";
            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // POST: SolicitudVacaciones/RechazarSolicitud/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RechazarSolicitud(int id)
        {
            // Aquí iría la lógica para rechazar la solicitud
            TempData["Mensaje"] = "Solicitud rechazada correctamente";
            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // MÉTODO AUXILIAR: Obtener datos del empleado por ID
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
                    };
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener datos del empleado: {ex.Message}");
                    return null;
                }
            }
        }

        // Método auxiliar para cargar empleados
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