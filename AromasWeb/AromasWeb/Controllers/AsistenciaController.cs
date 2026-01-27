using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class AsistenciaController : Controller
    {
        // GET: Asistencia/ListadoAsistencias (ADMIN)
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

        // GET: Asistencia/RegistrarEntrada (ADMIN)
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

        // POST: Asistencia/RegistrarEntrada (ADMIN)
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

        // GET: Asistencia/RegistrarSalida/5 (ADMIN)
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

        // POST: Asistencia/RegistrarSalida (ADMIN)
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

        // GET: Asistencia/EditarAsistencia/5 (ADMIN)
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

        // POST: Asistencia/EditarAsistencia (ADMIN)
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

        // POST: Asistencia/EliminarAsistencia/5 (ADMIN)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarAsistencia(int id)
        {
            TempData["Mensaje"] = "Asistencia eliminada correctamente";
            return RedirectToAction(nameof(ListadoAsistencias));
        }

        // GET: Asistencia/HistorialEmpleado/5 (ADMIN)
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

        // ============================================
        // NUEVAS FUNCIONES PARA EMPLEADOS
        // ============================================

        // GET: Asistencia/MiEntrada (EMPLEADO)
        public IActionResult MiEntrada()
        {
            // TODO: Obtener el ID del empleado de la sesión actual
            int idEmpleadoActual = 2; // Este valor vendría de la sesión

            // Verificar si ya tiene una entrada registrada hoy sin salida
            var asistenciaHoy = ObtenerAsistenciaHoy(idEmpleadoActual);

            if (asistenciaHoy != null && asistenciaHoy.HoraSalida == null)
            {
                TempData["Error"] = "Ya tienes una entrada registrada para hoy. Debes registrar tu salida primero.";
                return RedirectToAction(nameof(MiHistorialAsistencias));
            }

            // Información del empleado
            var empleado = ObtenerEmpleadoActual(idEmpleadoActual);
            ViewBag.EmpleadoActual = empleado;

            var model = new Asistencia
            {
                IdEmpleado = idEmpleadoActual,
                Fecha = DateTime.Now.Date,
                HoraEntrada = DateTime.Now.TimeOfDay
            };

            return View(model);
        }

        // POST: Asistencia/MiEntrada (EMPLEADO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MiEntrada(Asistencia asistencia)
        {
            // TODO: Obtener el ID del empleado de la sesión actual
            int idEmpleadoActual = 2;
            asistencia.IdEmpleado = idEmpleadoActual;

            if (ModelState.IsValid)
            {
                // Aquí se guardaría en la base de datos
                TempData["Mensaje"] = "¡Entrada registrada exitosamente! ¡Que tengas un excelente día de trabajo!";
                return RedirectToAction(nameof(MiHistorialAsistencias));
            }

            var empleado = ObtenerEmpleadoActual(idEmpleadoActual);
            ViewBag.EmpleadoActual = empleado;
            return View(asistencia);
        }

        // GET: Asistencia/MiSalida (EMPLEADO)
        public IActionResult MiSalida()
        {
            // TODO: Obtener el ID del empleado de la sesión actual
            int idEmpleadoActual = 2;

            // Buscar la asistencia de hoy sin salida registrada
            var asistenciaHoy = ObtenerAsistenciaHoy(idEmpleadoActual);

            if (asistenciaHoy == null || asistenciaHoy.HoraSalida != null)
            {
                TempData["Error"] = "No tienes una entrada registrada pendiente de salida para hoy.";
                return RedirectToAction(nameof(MiHistorialAsistencias));
            }

            // Información del empleado
            var empleado = ObtenerEmpleadoActual(idEmpleadoActual);
            ViewBag.EmpleadoActual = empleado;

            // Llenar datos de la asistencia
            asistenciaHoy.HoraSalida = DateTime.Now.TimeOfDay;
            asistenciaHoy.TiempoAlmuerzo = 60; // Valor por defecto

            return View(asistenciaHoy);
        }

        // POST: Asistencia/MiSalida (EMPLEADO)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MiSalida(Asistencia asistencia)
        {
            // TODO: Obtener el ID del empleado de la sesión actual
            int idEmpleadoActual = 2;

            if (ModelState.IsValid)
            {
                asistencia.CalcularHoras();
                // Aquí se actualizaría en la base de datos
                TempData["Mensaje"] = "¡Salida registrada exitosamente! ¡Hasta mañana!";
                return RedirectToAction(nameof(MiHistorialAsistencias));
            }

            var empleado = ObtenerEmpleadoActual(idEmpleadoActual);
            ViewBag.EmpleadoActual = empleado;
            return View(asistencia);
        }

        // GET: Asistencia/MiHistorialAsistencias (EMPLEADO)
        public IActionResult MiHistorialAsistencias(DateTime? fechaInicio, DateTime? fechaFin)
        {
            // TODO: Obtener el ID del empleado de la sesión actual
            int idEmpleadoActual = 2;

            // Información del empleado
            var empleado = ObtenerEmpleadoActual(idEmpleadoActual);
            ViewBag.EmpleadoActual = empleado;

            // Fechas por defecto (último mes)
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd") ?? DateTime.Now.AddMonths(-1).ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd") ?? DateTime.Now.ToString("yyyy-MM-dd");

            // Historial de ejemplo del empleado actual
            var historial = new List<Asistencia>
            {
                new Asistencia
                {
                    IdAsistencia = 1,
                    IdEmpleado = idEmpleadoActual,
                    NombreEmpleado = empleado.NombreCompleto,
                    Fecha = DateTime.Now.Date,
                    HoraEntrada = new TimeSpan(7, 30, 0),
                    HoraSalida = new TimeSpan(16, 0, 0),
                    TiempoAlmuerzo = 45
                },
                new Asistencia
                {
                    IdAsistencia = 2,
                    IdEmpleado = idEmpleadoActual,
                    NombreEmpleado = empleado.NombreCompleto,
                    Fecha = DateTime.Now.AddDays(-1).Date,
                    HoraEntrada = new TimeSpan(7, 0, 0),
                    HoraSalida = new TimeSpan(16, 30, 0),
                    TiempoAlmuerzo = 60
                },
                new Asistencia
                {
                    IdAsistencia = 3,
                    IdEmpleado = idEmpleadoActual,
                    NombreEmpleado = empleado.NombreCompleto,
                    Fecha = DateTime.Now.AddDays(-2).Date,
                    HoraEntrada = new TimeSpan(7, 15, 0),
                    HoraSalida = new TimeSpan(16, 15, 0),
                    TiempoAlmuerzo = 45
                },
                new Asistencia
                {
                    IdAsistencia = 4,
                    IdEmpleado = idEmpleadoActual,
                    NombreEmpleado = empleado.NombreCompleto,
                    Fecha = DateTime.Now.AddDays(-3).Date,
                    HoraEntrada = new TimeSpan(7, 45, 0),
                    HoraSalida = new TimeSpan(17, 0, 0),
                    TiempoAlmuerzo = 60
                },
                new Asistencia
                {
                    IdAsistencia = 5,
                    IdEmpleado = idEmpleadoActual,
                    NombreEmpleado = empleado.NombreCompleto,
                    Fecha = DateTime.Now.AddDays(-4).Date,
                    HoraEntrada = new TimeSpan(7, 30, 0),
                    HoraSalida = new TimeSpan(16, 45, 0),
                    TiempoAlmuerzo = 45
                }
            };

            foreach (var asistencia in historial)
            {
                asistencia.CalcularHoras();
            }

            return View(historial);
        }

        // ============================================
        // MÉTODOS AUXILIARES
        // ============================================

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

        // Método auxiliar para obtener información del empleado actual
        private Empleado ObtenerEmpleadoActual(int idEmpleado)
        {
            // En producción, esto vendría de la base de datos
            // Por ahora retornamos datos de ejemplo
            return new Empleado
            {
                IdEmpleado = idEmpleado,
                NombreCompleto = "Carlos Jiménez Mora",
                Identificacion = "2-2345-6789",
                Cargo = "Barista",
                FechaContratacion = DateTime.Now.AddMonths(-8),
                Estado = true
            };
        }

        // Método auxiliar para obtener la asistencia de hoy del empleado
        private Asistencia ObtenerAsistenciaHoy(int idEmpleado)
        {
            // En producción, esto consultaría la base de datos
            // Por ahora retornamos un ejemplo o null según el caso

            // Simulamos que el empleado ya tiene entrada registrada hoy
            return new Asistencia
            {
                IdAsistencia = 10,
                IdEmpleado = idEmpleado,
                NombreEmpleado = "Carlos Jiménez Mora",
                CargoEmpleado = "Barista",
                Fecha = DateTime.Now.Date,
                HoraEntrada = new TimeSpan(7, 30, 0),
                HoraSalida = null, // Sin salida aún
                TiempoAlmuerzo = 0
            };
        }
    }
}