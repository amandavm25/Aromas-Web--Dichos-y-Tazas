using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class AsistenciaController : Controller
    {
        // GET: Asistencia/ListadoAsistencias
        public IActionResult ListadoAsistencias(string buscar, DateTime? fechaInicio, DateTime? fechaFin, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");
            ViewBag.FiltroEstado = filtroEstado;

            // Asistencias de ejemplo
            var asistencias = new List<Asistencia>
            {
                new Asistencia
                {
                    IdAsistencia = 1,
                    IdEmpleado = 1,
                    NombreEmpleado = "María González Rodríguez",
                    IdentificacionEmpleado = "1-1234-5678",
                    CargoEmpleado = "Gerente General",
                    Fecha = DateTime.Now.Date,
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(17, 30, 0),
                    TiempoAlmuerzo = 60
                },
                new Asistencia
                {
                    IdAsistencia = 2,
                    IdEmpleado = 2,
                    NombreEmpleado = "Carlos Jiménez Mora",
                    IdentificacionEmpleado = "2-2345-6789",
                    CargoEmpleado = "Barista",
                    Fecha = DateTime.Now.Date,
                    HoraEntrada = new TimeSpan(7, 30, 0),
                    HoraSalida = new TimeSpan(16, 0, 0),
                    TiempoAlmuerzo = 45
                },
                new Asistencia
                {
                    IdAsistencia = 3,
                    IdEmpleado = 3,
                    NombreEmpleado = "Ana Patricia Vargas Solís",
                    IdentificacionEmpleado = "1-3456-7890",
                    CargoEmpleado = "Mesera",
                    Fecha = DateTime.Now.Date,
                    HoraEntrada = new TimeSpan(9, 0, 0),
                    HoraSalida = null,
                    TiempoAlmuerzo = 0
                },
                new Asistencia
                {
                    IdAsistencia = 4,
                    IdEmpleado = 4,
                    NombreEmpleado = "Roberto Fernández Castro",
                    IdentificacionEmpleado = "1-4567-8901",
                    CargoEmpleado = "Chef",
                    Fecha = DateTime.Now.AddDays(-1).Date,
                    HoraEntrada = new TimeSpan(10, 0, 0),
                    HoraSalida = new TimeSpan(20, 30, 0),
                    TiempoAlmuerzo = 60
                },
                new Asistencia
                {
                    IdAsistencia = 5,
                    IdEmpleado = 2,
                    NombreEmpleado = "Carlos Jiménez Mora",
                    IdentificacionEmpleado = "2-2345-6789",
                    CargoEmpleado = "Barista",
                    Fecha = DateTime.Now.AddDays(-1).Date,
                    HoraEntrada = new TimeSpan(7, 0, 0),
                    HoraSalida = new TimeSpan(16, 30, 0),
                    TiempoAlmuerzo = 60
                },
                new Asistencia
                {
                    IdAsistencia = 6,
                    IdEmpleado = 6,
                    NombreEmpleado = "José Luis Ramírez Quesada",
                    IdentificacionEmpleado = "1-6789-0123",
                    CargoEmpleado = "Barista",
                    Fecha = DateTime.Now.AddDays(-2).Date,
                    HoraEntrada = new TimeSpan(8, 30, 0),
                    HoraSalida = new TimeSpan(17, 0, 0),
                    TiempoAlmuerzo = 45
                }
            };

            // Calcular horas para cada registro
            foreach (var asistencia in asistencias)
            {
                asistencia.CalcularHoras();
            }

            return View(asistencias);
        }

        // GET: Asistencia/RegistrarEntrada
        public IActionResult RegistrarEntrada()
        {
            CargarEmpleados();

            var model = new Asistencia
            {
                Fecha = DateTime.Now.Date,
                HoraEntrada = DateTime.Now.TimeOfDay
            };

            return View(model);
        }

        // POST: Asistencia/RegistrarEntrada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarEntrada(Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Registro de inicio de jornada correctamente";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            CargarEmpleados();
            return View(asistencia);
        }

        // GET: Asistencia/RegistrarSalida/5
        public IActionResult RegistrarSalida(int id)
        {
            // Asistencia de ejemplo
            var asistencia = new Asistencia
            {
                IdAsistencia = id,
                IdEmpleado = 1,
                NombreEmpleado = "María González Rodríguez",
                IdentificacionEmpleado = "1-1234-5678",
                CargoEmpleado = "Gerente General",
                Fecha = DateTime.Now.Date,
                HoraEntrada = new TimeSpan(8, 0, 0),
                HoraSalida = DateTime.Now.TimeOfDay,
                TiempoAlmuerzo = 60
            };

            return View(asistencia);
        }

        // POST: Asistencia/RegistrarSalida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarSalida(Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                asistencia.CalcularHoras();
                TempData["Mensaje"] = "Registro de finalización de jornada correctamente";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            return View(asistencia);
        }

        // GET: Asistencia/EditarAsistencia/5
        public IActionResult EditarAsistencia(int id)
        {
            // Asistencia de ejemplo
            var asistencia = new Asistencia
            {
                IdAsistencia = id,
                IdEmpleado = 1,
                NombreEmpleado = "María González Rodríguez",
                Fecha = DateTime.Now.Date,
                HoraEntrada = new TimeSpan(8, 0, 0),
                HoraSalida = new TimeSpan(17, 30, 0),
                TiempoAlmuerzo = 60
            };

            asistencia.CalcularHoras();
            CargarEmpleados();
            return View(asistencia);
        }

        // POST: Asistencia/EditarAsistencia
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarAsistencia(Asistencia asistencia)
        {
            if (ModelState.IsValid)
            {
                asistencia.CalcularHoras();
                TempData["Mensaje"] = "Asistencia actualizada correctamente";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            CargarEmpleados();
            return View(asistencia);
        }

        // POST: Asistencia/EliminarAsistencia/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAsistencia(int id)
        {
            TempData["Mensaje"] = "Asistencia eliminada correctamente";
            return RedirectToAction(nameof(ListadoAsistencias));
        }

        // GET: Asistencia/HistorialEmpleado/5
        public IActionResult HistorialEmpleado(int idEmpleado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");

            // Datos del empleado
            ViewBag.NombreEmpleado = "María González Rodríguez";
            ViewBag.IdentificacionEmpleado = "1-1234-5678";
            ViewBag.CargoEmpleado = "Gerente General";
            ViewBag.IdEmpleado = idEmpleado;

            // Historial de ejemplo
            var historial = new List<Asistencia>
            {
                new Asistencia
                {
                    IdAsistencia = 1,
                    IdEmpleado = idEmpleado,
                    NombreEmpleado = "María González Rodríguez",
                    Fecha = DateTime.Now.Date,
                    HoraEntrada = new TimeSpan(8, 0, 0),
                    HoraSalida = new TimeSpan(17, 30, 0),
                    TiempoAlmuerzo = 60
                },
                new Asistencia
                {
                    IdAsistencia = 2,
                    IdEmpleado = idEmpleado,
                    NombreEmpleado = "María González Rodríguez",
                    Fecha = DateTime.Now.AddDays(-1).Date,
                    HoraEntrada = new TimeSpan(8, 15, 0),
                    HoraSalida = new TimeSpan(18, 0, 0),
                    TiempoAlmuerzo = 60
                }
            };

            foreach (var asistencia in historial)
            {
                asistencia.CalcularHoras();
            }

            return View(historial);
        }

        // Método auxiliar para cargar empleados
        private void CargarEmpleados()
        {
            var empleados = new List<dynamic>
            {
                new { IdEmpleado = 1, NombreCompleto = "María González Rodríguez", Cargo = "Gerente General" },
                new { IdEmpleado = 2, NombreCompleto = "Carlos Jiménez Mora", Cargo = "Barista" },
                new { IdEmpleado = 3, NombreCompleto = "Ana Patricia Vargas Solís", Cargo = "Mesera" },
                new { IdEmpleado = 4, NombreCompleto = "Roberto Fernández Castro", Cargo = "Chef" },
                new { IdEmpleado = 5, NombreCompleto = "Laura Martínez Pérez", Cargo = "Cajera" },
                new { IdEmpleado = 6, NombreCompleto = "José Luis Ramírez Quesada", Cargo = "Barista" }
            };

            ViewBag.Empleados = empleados;
        }
    }
}