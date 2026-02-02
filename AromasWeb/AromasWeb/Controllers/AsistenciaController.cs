using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Asistencia;
using AromasWeb.Abstracciones.Logica.Empleado;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class AsistenciaController : Controller
    {
        private IListarAsistencias _listarAsistencias;
        private IListarEmpleados _listarEmpleados;

        public AsistenciaController()
        {
            _listarAsistencias = new LogicaDeNegocio.Asistencias.ListarAsistencias();
            _listarEmpleados = new LogicaDeNegocio.Empleados.ListarEmpleados();
        }

        // GET: Asistencia/ListadoAsistencias
        public IActionResult ListadoAsistencias(string buscar, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // Obtener asistencias según los filtros
            List<Asistencia> asistencias;

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                // Buscar por rango de fechas
                asistencias = _listarAsistencias.BuscarPorRangoFechas(fechaInicio.Value, fechaFin.Value);
            }
            else
            {
                // Obtener todas
                asistencias = _listarAsistencias.Obtener();
            }

            // Aplicar filtro de búsqueda por nombre/identificación/cargo
            if (!string.IsNullOrEmpty(buscar))
            {
                asistencias = asistencias.Where(a =>
                    (a.NombreEmpleado != null && a.NombreEmpleado.ToLower().Contains(buscar.ToLower())) ||
                    (a.IdentificacionEmpleado != null && a.IdentificacionEmpleado.Contains(buscar)) ||
                    (a.CargoEmpleado != null && a.CargoEmpleado.ToLower().Contains(buscar.ToLower()))
                ).ToList();
            }

            // Calcular horas para cada registro
            foreach (var asistencia in asistencias)
            {
                asistencia.CalcularHoras();
            }

            return View(asistencias);
        }

        // GET: Asistencia/ListadoAsistencias
        public IActionResult MiHistorialAsistencias()
        {
            return View();
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
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Registro de inicio de jornada correctamente";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            CargarEmpleados();
            return View(asistencia);
        }

        // GET: Asistencia/RegistrarSalida/5
        public IActionResult RegistrarSalida(int id)
        {
            var asistencia = _listarAsistencias.ObtenerPorId(id);

            if (asistencia == null)
            {
                TempData["Error"] = "Asistencia no encontrada";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            asistencia.HoraSalida = DateTime.Now.TimeOfDay;

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
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Registro de finalización de jornada correctamente";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            return View(asistencia);
        }

        // GET: Asistencia/RegistrarSalida/5
        public IActionResult MiEntrada(int id)
        {
            return View();
        }

        // POST: Asistencia/RegistrarSalida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MiSalida(Asistencia asistencia)
        {
            return View();
        }

        // GET: Asistencia/EditarAsistencia/5
        public IActionResult EditarAsistencia(int id)
        {
            var asistencia = _listarAsistencias.ObtenerPorId(id);

            if (asistencia == null)
            {
                TempData["Error"] = "Asistencia no encontrada";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

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
                // Aquí iría la lógica para actualizar en la base de datos
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
            // Aquí iría la lógica para eliminar la asistencia
            TempData["Mensaje"] = "Asistencia eliminada correctamente";
            return RedirectToAction(nameof(ListadoAsistencias));
        }

        // Método auxiliar para cargar empleados dinámicamente desde la base de datos
        private void CargarEmpleados()
        {
            var empleados = _listarEmpleados.Obtener();

            // Crear lista con el formato necesario para el dropdown
            var empleadosDropdown = empleados.Select(e => new
            {
                IdEmpleado = e.IdEmpleado,
                NombreCompleto = $"{e.Nombre} {e.Apellidos}",
                Cargo = e.Cargo
            }).ToList();

            ViewBag.Empleados = empleadosDropdown;
        }
    }
}