using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
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

        // Método auxiliar para cargar empleados
        private void CargarEmpleados()
        {
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