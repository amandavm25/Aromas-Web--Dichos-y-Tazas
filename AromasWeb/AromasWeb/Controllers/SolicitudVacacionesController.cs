using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class SolicitudVacacionesController : Controller
    {
        // GET: SolicitudVacaciones/ListadoSolicitudes
        public IActionResult ListadoSolicitudes(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Solicitudes de ejemplo
            var solicitudes = new List<SolicitudVacaciones>
            {
                new SolicitudVacaciones
                {
                    IdSolicitud = 1,
                    IdEmpleado = 1,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    FechaContratacionEmpleado = DateTime.Now.AddYears(-2).AddMonths(-10),
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaInicio = DateTime.Now.AddDays(10),
                    FechaFin = DateTime.Now.AddDays(17),
                    DiasSolicitados = 6,
                    DiasDisponibles = 34,
                    DiasTomados = 0,
                    Estado = "Pendiente",
                    FechaRespuesta = null
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 2,
                    IdEmpleado = 4,
                    NombreEmpleado = "Roberto Fernández Castro",
                    IdentificacionEmpleado = "1-4567-8901",
                    CargoEmpleado = "Chef",
                    FechaContratacionEmpleado = DateTime.Now.AddYears(-3),
                    FechaSolicitud = DateTime.Now.AddDays(-15),
                    FechaInicio = DateTime.Now.AddDays(-10),
                    FechaFin = DateTime.Now.AddDays(-3),
                    DiasSolicitados = 6,
                    DiasDisponibles = 30,
                    DiasTomados = 6,
                    Estado = "Aprobada",
                    FechaRespuesta = DateTime.Now.AddDays(-14)
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 3,
                    IdEmpleado = 6,
                    NombreEmpleado = "José Luis Ramírez Quesada",
                    IdentificacionEmpleado = "1-6789-0123",
                    CargoEmpleado = "Barista",
                    FechaContratacionEmpleado = DateTime.Now.AddMonths(-18),
                    FechaSolicitud = DateTime.Now.AddDays(-20),
                    FechaInicio = DateTime.Now.AddDays(-15),
                    FechaFin = DateTime.Now.AddDays(-11),
                    DiasSolicitados = 4,
                    DiasDisponibles = 14,
                    DiasTomados = 4,
                    Estado = "Aprobada",
                    FechaRespuesta = DateTime.Now.AddDays(-19)
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 4,
                    IdEmpleado = 2,
                    NombreEmpleado = "Carlos Jiménez Mora",
                    IdentificacionEmpleado = "2-2345-6789",
                    CargoEmpleado = "Barista",
                    FechaContratacionEmpleado = DateTime.Now.AddMonths(-8),
                    FechaSolicitud = DateTime.Now.AddDays(-3),
                    FechaInicio = DateTime.Now.AddDays(30),
                    FechaFin = DateTime.Now.AddDays(32),
                    DiasSolicitados = 2,
                    DiasDisponibles = 8,
                    DiasTomados = 0,
                    Estado = "Pendiente",
                    FechaRespuesta = null
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 5,
                    IdEmpleado = 3,
                    NombreEmpleado = "Ana Patricia Vargas Solís",
                    IdentificacionEmpleado = "1-3456-7890",
                    CargoEmpleado = "Mesera",
                    FechaContratacionEmpleado = DateTime.Now.AddMonths(-14),
                    FechaSolicitud = DateTime.Now.AddDays(-7),
                    FechaInicio = DateTime.Now.AddDays(5),
                    FechaFin = DateTime.Now.AddDays(9),
                    DiasSolicitados = 4,
                    DiasDisponibles = 14,
                    DiasTomados = 0,
                    Estado = "Rechazada",
                    FechaRespuesta = DateTime.Now.AddDays(-6)
                }
            };

            // Aplicar filtro de búsqueda
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                solicitudes = solicitudes.Where(s =>
                    s.NombreEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    s.IdentificacionEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    s.PeriodoVacaciones.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            // Aplicar filtro de estado
            if (!string.IsNullOrWhiteSpace(filtroEstado))
            {
                solicitudes = solicitudes.Where(s => s.Estado.Equals(filtroEstado, StringComparison.OrdinalIgnoreCase)).ToList();
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
                // Calcular días laborables (excluyendo domingos)
                solicitud.DiasSolicitados = solicitud.CalcularDiasLaborables();

                // Validar días disponibles
                if (solicitud.DiasSolicitados > solicitud.DiasDisponibles)
                {
                    TempData["Error"] = $"No tiene suficientes días disponibles. Solicitó {solicitud.DiasSolicitados} días pero solo tiene {solicitud.DiasDisponibles} disponibles";
                    CargarEmpleados();
                    return View(solicitud);
                }

                // Validar que las fechas sean coherentes
                if (solicitud.FechaFin < solicitud.FechaInicio)
                {
                    TempData["Error"] = "La fecha de fin no puede ser anterior a la fecha de inicio";
                    CargarEmpleados();
                    return View(solicitud);
                }

                // Aquí iría la lógica para guardar en la base de datos
                // await _solicitudVacacionesService.CrearSolicitud(solicitud);

                TempData["Mensaje"] = "Solicitud de vacaciones registrada correctamente";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            CargarEmpleados();
            return View(solicitud);
        }

        // GET: SolicitudVacaciones/EditarSolicitud/5
        public IActionResult EditarSolicitud(int id)
        {
            // Solicitud de ejemplo - En producción esto vendría de la base de datos
            var solicitud = new SolicitudVacaciones
            {
                IdSolicitud = id,
                IdEmpleado = 1,
                NombreEmpleado = "María González Rodríguez",
                FechaContratacionEmpleado = DateTime.Now.AddYears(-2).AddMonths(-10),
                FechaSolicitud = DateTime.Now.AddDays(-5),
                FechaInicio = DateTime.Now.AddDays(10),
                FechaFin = DateTime.Now.AddDays(17),
                DiasSolicitados = 6,
                DiasDisponibles = 34,
                DiasTomados = 0,
                Estado = "Pendiente"
            };

            // Solo permitir editar si está pendiente
            if (solicitud.Estado != "Pendiente")
            {
                TempData["Error"] = "Solo se pueden editar solicitudes en estado Pendiente";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            CargarEmpleados();
            ViewBag.CargoEmpleado = "Gerente General"; // En producción vendría del empleado
            return View(solicitud);
        }

        // POST: SolicitudVacaciones/EditarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarSolicitud(SolicitudVacaciones solicitud)
        {
            if (ModelState.IsValid)
            {
                // Calcular días laborables (excluyendo domingos)
                solicitud.DiasSolicitados = solicitud.CalcularDiasLaborables();

                // Validar días disponibles
                if (solicitud.DiasSolicitados > solicitud.DiasDisponibles)
                {
                    TempData["Error"] = $"No tiene suficientes días disponibles. Solicitó {solicitud.DiasSolicitados} días pero solo tiene {solicitud.DiasDisponibles} disponibles";
                    CargarEmpleados();
                    ViewBag.CargoEmpleado = "Gerente General";
                    return View(solicitud);
                }

                // Validar que las fechas sean coherentes
                if (solicitud.FechaFin < solicitud.FechaInicio)
                {
                    TempData["Error"] = "La fecha de fin no puede ser anterior a la fecha de inicio";
                    CargarEmpleados();
                    ViewBag.CargoEmpleado = "Gerente General";
                    return View(solicitud);
                }

                // Aquí iría la lógica para actualizar en la base de datos
                // await _solicitudVacacionesService.ActualizarSolicitud(solicitud);

                TempData["Mensaje"] = "Solicitud de vacaciones actualizada correctamente";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            CargarEmpleados();
            ViewBag.CargoEmpleado = "Gerente General";
            return View(solicitud);
        }

        // POST: SolicitudVacaciones/EliminarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarSolicitud(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID de solicitud inválido";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            // Aquí iría la lógica para eliminar de la base de datos
            // Solo se debe permitir eliminar solicitudes pendientes
            // await _solicitudVacacionesService.EliminarSolicitud(id);

            TempData["Mensaje"] = "Solicitud de vacaciones eliminada correctamente";
            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // POST: SolicitudVacaciones/AprobarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult AprobarSolicitud(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID de solicitud inválido";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            // Aquí iría la lógica para aprobar la solicitud
            // - Cambiar estado a "Aprobada"
            // - Establecer FechaRespuesta = DateTime.Now
            // - Actualizar DiasTomados del empleado
            // await _solicitudVacacionesService.AprobarSolicitud(id);

            TempData["Mensaje"] = "Solicitud de vacaciones aprobada correctamente";
            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // POST: SolicitudVacaciones/RechazarSolicitud
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RechazarSolicitud(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "ID de solicitud inválido";
                return RedirectToAction(nameof(ListadoSolicitudes));
            }

            // Aquí iría la lógica para rechazar la solicitud
            // - Cambiar estado a "Rechazada"
            // - Establecer FechaRespuesta = DateTime.Now
            // await _solicitudVacacionesService.RechazarSolicitud(id);

            TempData["Mensaje"] = "Solicitud de vacaciones rechazada";
            return RedirectToAction(nameof(ListadoSolicitudes));
        }

        // GET: SolicitudVacaciones/ReporteVacaciones/5
        public IActionResult ReporteVacaciones(int idEmpleado)
        {
            // Datos del empleado - En producción vendría de la base de datos
            ViewBag.IdEmpleado = idEmpleado;
            ViewBag.NombreEmpleado = "María González Rodríguez";
            ViewBag.IdentificacionEmpleado = "1-1234-5678";
            ViewBag.CargoEmpleado = "Gerente General";
            ViewBag.FechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10);

            var fechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10);
            var mesesTrabajados = (int)((DateTime.Now - fechaContratacion).TotalDays / 30);

            ViewBag.MesesTrabajados = mesesTrabajados;
            ViewBag.DiasAcumulados = mesesTrabajados; // 1 día por mes trabajado
            ViewBag.DiasTomados = 6;
            ViewBag.DiasDisponibles = mesesTrabajados - 6;

            // Historial de solicitudes del empleado
            var historial = new List<SolicitudVacaciones>
            {
                new SolicitudVacaciones
                {
                    IdSolicitud = 1,
                    IdEmpleado = idEmpleado,
                    NombreEmpleado = "María González Rodríguez",
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaInicio = DateTime.Now.AddDays(10),
                    FechaFin = DateTime.Now.AddDays(17),
                    DiasSolicitados = 6,
                    Estado = "Pendiente"
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 2,
                    IdEmpleado = idEmpleado,
                    NombreEmpleado = "María González Rodríguez",
                    FechaSolicitud = DateTime.Now.AddMonths(-3),
                    FechaInicio = DateTime.Now.AddMonths(-2),
                    FechaFin = DateTime.Now.AddMonths(-2).AddDays(4),
                    DiasSolicitados = 4,
                    Estado = "Aprobada",
                    FechaRespuesta = DateTime.Now.AddMonths(-3).AddDays(1)
                }
            };

            return View(historial);
        }

        // GET: SolicitudVacaciones/VerSolicitudesEmpleado/5
        public IActionResult VerSolicitudesEmpleado(int idEmpleado)
        {
            // Datos del empleado - En producción vendría de la base de datos
            ViewBag.IdEmpleado = idEmpleado;
            ViewBag.NombreEmpleado = "María González Rodríguez";
            ViewBag.IdentificacionEmpleado = "1-1234-5678";
            ViewBag.CargoEmpleado = "Gerente General";
            ViewBag.FechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10);

            var fechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10);
            var mesesTrabajados = (int)((DateTime.Now - fechaContratacion).TotalDays / 30);

            ViewBag.MesesTrabajados = mesesTrabajados;
            ViewBag.DiasAcumulados = mesesTrabajados;
            ViewBag.DiasTomados = 6;
            ViewBag.DiasDisponibles = mesesTrabajados - 6;

            // Solicitudes del empleado - En producción vendría de la base de datos
            var solicitudes = new List<SolicitudVacaciones>
            {
                new SolicitudVacaciones
                {
                    IdSolicitud = 1,
                    IdEmpleado = idEmpleado,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    FechaContratacionEmpleado = fechaContratacion,
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaInicio = DateTime.Now.AddDays(10),
                    FechaFin = DateTime.Now.AddDays(17),
                    DiasSolicitados = 6,
                    DiasDisponibles = 34,
                    DiasTomados = 0,
                    Estado = "Pendiente",
                    FechaRespuesta = null
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 2,
                    IdEmpleado = idEmpleado,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    FechaContratacionEmpleado = fechaContratacion,
                    FechaSolicitud = DateTime.Now.AddMonths(-3),
                    FechaInicio = DateTime.Now.AddMonths(-2),
                    FechaFin = DateTime.Now.AddMonths(-2).AddDays(4),
                    DiasSolicitados = 4,
                    DiasDisponibles = 38,
                    DiasTomados = 4,
                    Estado = "Aprobada",
                    FechaRespuesta = DateTime.Now.AddMonths(-3).AddDays(1)
                }
            };

            return View(solicitudes);
        }

        // GET: SolicitudVacaciones/MisSolicitudes
        public IActionResult MisSolicitudes()
        {
            // En producción, obtener el ID del empleado desde la sesión
            // var idEmpleado = HttpContext.Session.GetInt32("IdEmpleado");

            // Datos del empleado actual - En producción vendría de la sesión/base de datos
            ViewBag.IdEmpleado = 1;
            ViewBag.NombreEmpleado = "María González Rodríguez";
            ViewBag.IdentificacionEmpleado = "1-1234-5678";
            ViewBag.CargoEmpleado = "Gerente General";
            ViewBag.FechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10);

            var fechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10);
            var mesesTrabajados = (int)((DateTime.Now - fechaContratacion).TotalDays / 30);

            ViewBag.MesesTrabajados = mesesTrabajados;
            ViewBag.DiasAcumulados = mesesTrabajados;
            ViewBag.DiasTomados = 6;
            ViewBag.DiasDisponibles = mesesTrabajados - 6;

            // Solicitudes del empleado actual
            var solicitudes = new List<SolicitudVacaciones>
            {
                new SolicitudVacaciones
                {
                    IdSolicitud = 1,
                    IdEmpleado = 1,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    FechaContratacionEmpleado = fechaContratacion,
                    FechaSolicitud = DateTime.Now.AddDays(-5),
                    FechaInicio = DateTime.Now.AddDays(10),
                    FechaFin = DateTime.Now.AddDays(17),
                    DiasSolicitados = 6,
                    DiasDisponibles = 34,
                    DiasTomados = 0,
                    Estado = "Pendiente",
                    FechaRespuesta = null
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 2,
                    IdEmpleado = 1,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    FechaContratacionEmpleado = fechaContratacion,
                    FechaSolicitud = DateTime.Now.AddMonths(-3),
                    FechaInicio = DateTime.Now.AddMonths(-2),
                    FechaFin = DateTime.Now.AddMonths(-2).AddDays(4),
                    DiasSolicitados = 4,
                    DiasDisponibles = 38,
                    DiasTomados = 4,
                    Estado = "Aprobada",
                    FechaRespuesta = DateTime.Now.AddMonths(-3).AddDays(1)
                },
                new SolicitudVacaciones
                {
                    IdSolicitud = 3,
                    IdEmpleado = 1,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    FechaContratacionEmpleado = fechaContratacion,
                    FechaSolicitud = DateTime.Now.AddMonths(-6),
                    FechaInicio = DateTime.Now.AddMonths(-5),
                    FechaFin = DateTime.Now.AddMonths(-5).AddDays(2),
                    DiasSolicitados = 2,
                    DiasDisponibles = 42,
                    DiasTomados = 2,
                    Estado = "Rechazada",
                    FechaRespuesta = DateTime.Now.AddMonths(-6).AddDays(2)
                }
            };

            return View(solicitudes);
        }

        // Método auxiliar para cargar empleados
        private void CargarEmpleados()
        {
            // En producción, estos datos vendrían de la base de datos
            var empleados = new List<dynamic>
            {
                new {
                    IdEmpleado = 1,
                    NombreCompleto = "María González Rodríguez",
                    Cargo = "Gerente General",
                    FechaContratacion = DateTime.Now.AddYears(-2).AddMonths(-10),
                    DiasDisponibles = 34
                },
                new {
                    IdEmpleado = 2,
                    NombreCompleto = "Carlos Jiménez Mora",
                    Cargo = "Barista",
                    FechaContratacion = DateTime.Now.AddMonths(-8),
                    DiasDisponibles = 8
                },
                new {
                    IdEmpleado = 3,
                    NombreCompleto = "Ana Patricia Vargas Solís",
                    Cargo = "Mesera",
                    FechaContratacion = DateTime.Now.AddMonths(-14),
                    DiasDisponibles = 14
                },
                new {
                    IdEmpleado = 4,
                    NombreCompleto = "Roberto Fernández Castro",
                    Cargo = "Chef",
                    FechaContratacion = DateTime.Now.AddYears(-3),
                    DiasDisponibles = 30
                },
                new {
                    IdEmpleado = 5,
                    NombreCompleto = "Laura Martínez Pérez",
                    Cargo = "Cajera",
                    FechaContratacion = DateTime.Now.AddMonths(-6),
                    DiasDisponibles = 6
                },
                new {
                    IdEmpleado = 6,
                    NombreCompleto = "José Luis Ramírez Quesada",
                    Cargo = "Barista",
                    FechaContratacion = DateTime.Now.AddMonths(-18),
                    DiasDisponibles = 14
                }
            };

            ViewBag.Empleados = empleados;
        }
    }
}