using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Asistencia;
using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class AsistenciaController : Controller
    {
        private IListarAsistencias _listarAsistencias;
        private IListarEmpleados _listarEmpleados;
        private readonly CrearBitacora _crearBitacora;

        public AsistenciaController()
        {
            _listarAsistencias = new LogicaDeNegocio.Asistencias.ListarAsistencias();
            _listarEmpleados = new LogicaDeNegocio.Empleados.ListarEmpleados();
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

        //ADMIN-LISTADO GENERAL

        // GET: Asistencia/ListadoAsistencias
        public IActionResult ListadoAsistencias(string buscar, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            List<Asistencia> asistencias;

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                asistencias = _listarAsistencias.BuscarPorRangoFechas(fechaInicio.Value, fechaFin.Value);
            }
            else
            {
                asistencias = _listarAsistencias.Obtener();
            }

            if (!string.IsNullOrEmpty(buscar))
            {
                asistencias = asistencias.Where(a =>
                    (a.NombreEmpleado != null && a.NombreEmpleado.ToLower().Contains(buscar.ToLower())) ||
                    (a.IdentificacionEmpleado != null && a.IdentificacionEmpleado.Contains(buscar)) ||
                    (a.CargoEmpleado != null && a.CargoEmpleado.ToLower().Contains(buscar.ToLower()))
                ).ToList();
            }

            foreach (var asistencia in asistencias)
            {
                asistencia.CalcularHoras();
            }

            return View(asistencias);
        }

        //EMPLEADO - MI HISTORIAL

        public IActionResult MiHistorialAsistencias()
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 0;

            if (idEmpleado == 0)
                return RedirectToAction("Login", "Auth");

            var empleado = _listarEmpleados.ObtenerPorId(idEmpleado);
            ViewBag.Empleado = empleado;

            var asistencias = _listarAsistencias.BuscarPorEmpleado(idEmpleado);

            foreach (var asistencia in asistencias)
            {
                asistencia.CalcularHoras();
            }

            return View(asistencias);
        }

        // EMPLEADO - MI ENTRADA GET

        [HttpGet]
        public IActionResult MiEntrada()
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 0;

            if (idEmpleado == 0)
                return RedirectToAction("Login", "Auth");

            var empleado = _listarEmpleados.ObtenerPorId(idEmpleado);
            ViewBag.Empleado = empleado;

            var model = new Asistencia
            {
                IdEmpleado = idEmpleado,
                Fecha = DateTime.Now.Date,
                HoraEntrada = DateTime.Now.TimeOfDay
            };

            return View(model);
        }

        // EMPLEADO - MI ENTRADA POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MiEntrada(Asistencia asistencia)
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 0;
            if (idEmpleado == 0) return RedirectToAction("Login", "Auth");

            asistencia.IdEmpleado = idEmpleado;
            asistencia.Fecha = DateTime.SpecifyKind(asistencia.Fecha.Date, DateTimeKind.Utc);

            var empleado = _listarEmpleados.ObtenerPorId(asistencia.IdEmpleado);
            ViewBag.Empleado = empleado;

            if (!ModelState.IsValid) return View(asistencia);

            var yaExiste = _listarAsistencias.ExisteEntradaAbierta(asistencia.IdEmpleado, asistencia.Fecha);
            if (yaExiste)
            {
                ModelState.AddModelError("", "Ya existe una entrada sin salida para este empleado en la fecha seleccionada.");
                return View(asistencia);
            }

            _listarAsistencias.CrearEntrada(asistencia);

            _crearBitacora.RegistrarAccion(
                idEmpleado: idEmpleado,
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Control de asistencia"),
                accion: Bitacora.Acciones.RegistrarEntrada,
                tablaAfectada: "Asistencia",
                descripcion: $"El empleado registró su entrada — Fecha: {asistencia.Fecha:dd/MM/yyyy}, Hora: {asistencia.HoraEntrada:hh\\:mm}",
                datosNuevos: JsonSerializer.Serialize(new
                {
                    asistencia.IdEmpleado,
                    Fecha = asistencia.Fecha.ToString("dd/MM/yyyy"),
                    HoraEntrada = asistencia.HoraEntrada.ToString(@"hh\:mm")
                })
            );

            TempData["Mensaje"] = "Entrada registrada correctamente.";
            return RedirectToAction("MiHistorialAsistencias");
        }

        // EMPLEADO - MI SALIDA GET

        [HttpGet]
        public IActionResult MiSalida()
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 0;

            if (idEmpleado == 0)
                return RedirectToAction("Login", "Auth");

            var empleado = _listarEmpleados.ObtenerPorId(idEmpleado);
            ViewBag.Empleado = empleado;

            var entradaABierta = _listarAsistencias.ObtenerEntradaAbierta(idEmpleado);

            if (entradaABierta == null)
            {
                TempData["Error"] = "No tienes una entrada abierta para registrar la salida.";
                return RedirectToAction(nameof(MiEntrada));
            }

            entradaABierta.HoraSalida = DateTime.Now.TimeOfDay;
            return View(entradaABierta);
        }

        // EMPLEADO - MI SALIDA POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult MiSalida(Asistencia asistencia)
        {
            int idEmpleado = HttpContext.Session.GetInt32("IdEmpleado") ?? 0;

            if (idEmpleado == 0)
                return RedirectToAction("Login", "Auth");

            asistencia.IdEmpleado = idEmpleado;
            var empleado = _listarEmpleados.ObtenerPorId(idEmpleado);
            ViewBag.Empleado = empleado;

            if (!asistencia.HoraSalida.HasValue)
            {
                ModelState.AddModelError("", "La hora de salida es obligatoria.");
            }

            if (!ModelState.IsValid)
            {
                return View(asistencia);
            }

            asistencia.CalcularHoras();

            _listarAsistencias.RegistrarSalida(asistencia.IdAsistencia, asistencia.HoraSalida.Value);

            _crearBitacora.RegistrarAccion(
                idEmpleado: idEmpleado,
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Control de asistencia"),
                accion: Bitacora.Acciones.RegistrarSalida,
                tablaAfectada: "Asistencia",
                descripcion: $"El empleado registró su salida — Fecha: {asistencia.Fecha:dd/MM/yyyy}, Hora: {asistencia.HoraSalida.Value:hh\\:mm}, Horas trabajadas: {asistencia.HorasTotales:F2}h",
                datosNuevos: JsonSerializer.Serialize(new
                {
                    asistencia.IdEmpleado,
                    Fecha = asistencia.Fecha.ToString("dd/MM/yyyy"),
                    HoraSalida = asistencia.HoraSalida.Value.ToString(@"hh\:mm"),
                    asistencia.HorasRegulares,
                    asistencia.HorasExtras,
                    asistencia.HorasTotales
                })
            );

            TempData["Mensaje"] = "Salida registrada correctamente.";
            return RedirectToAction("MiHistorialAsistencias");
        }

        // ADMIN - HISTORIAL DE UN EMPLEADO ESPECÍFICO

        // GET: Asistencia/HistorialEmpleado/5
        public IActionResult HistorialEmpleado(int idEmpleado, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var empleado = _listarEmpleados.ObtenerPorId(idEmpleado);

            if (empleado == null)
            {
                TempData["Error"] = "Empleado no encontrado";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            ViewBag.IdEmpleado = idEmpleado;
            ViewBag.NombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
            ViewBag.CargoEmpleado = empleado.Cargo;
            ViewBag.Empleado = empleado;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            List<Asistencia> asistencias;

            if (fechaInicio.HasValue && fechaFin.HasValue)
            {
                asistencias = _listarAsistencias.BuscarPorRangoFechas(fechaInicio.Value, fechaFin.Value)
                    .Where(a => a.IdEmpleado == idEmpleado)
                    .ToList();
            }
            else
            {
                asistencias = _listarAsistencias.BuscarPorEmpleado(idEmpleado);
            }

            foreach (var asistencia in asistencias)
            {
                asistencia.CalcularHoras();
            }

            return View(asistencias);
        }

        // ADMIN - REGISTRAR ENTRADA

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
            if (!ModelState.IsValid)
            {
                CargarEmpleados();
                return View(asistencia);
            }

            var yaExiste = _listarAsistencias.ExisteEntradaAbierta(asistencia.IdEmpleado, asistencia.Fecha);
            if (yaExiste)
            {
                ModelState.AddModelError("", "Ya existe una entrada sin salida para este empleado en la fecha seleccionada.");
                CargarEmpleados();
                return View(asistencia);
            }

            _listarAsistencias.CrearEntrada(asistencia);

            _crearBitacora.RegistrarAccion(
                idEmpleado: ObtenerIdEmpleadoSesion(),
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Control de asistencia"),
                accion: Bitacora.Acciones.RegistrarEntrada,
                tablaAfectada: "Asistencia",
                descripcion: $"Admin registró entrada del empleado ID {asistencia.IdEmpleado} — Fecha: {asistencia.Fecha:dd/MM/yyyy}, Hora: {asistencia.HoraEntrada:hh\\:mm}",
                datosNuevos: JsonSerializer.Serialize(new
                {
                    asistencia.IdEmpleado,
                    Fecha = asistencia.Fecha.ToString("dd/MM/yyyy"),
                    HoraEntrada = asistencia.HoraEntrada.ToString(@"hh\:mm")
                })
            );

            TempData["Mensaje"] = "Registro de inicio de jornada correctamente";
            return RedirectToAction(nameof(ListadoAsistencias));
        }

        // ADMIN - REGISTRAR SALIDA

        // GET: Asistencia/RegistrarSalida/5
        public IActionResult RegistrarSalida(int id)
        {
            var asistencia = _listarAsistencias.ObtenerPorId(id);

            if (asistencia == null || asistencia.HoraSalida.HasValue)
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
            if (!asistencia.HoraSalida.HasValue)
                ModelState.AddModelError("", "La hora de salida es obligatoria.");

            if (!ModelState.IsValid)
                return View(asistencia);

            asistencia.CalcularHoras();
            _listarAsistencias.RegistrarSalida(asistencia.IdAsistencia, asistencia.HoraSalida.Value);

            _crearBitacora.RegistrarAccion(
                idEmpleado: ObtenerIdEmpleadoSesion(),
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Control de asistencia"),
                accion: Bitacora.Acciones.RegistrarSalida,
                tablaAfectada: "Asistencia",
                descripcion: $"Admin registró salida del empleado ID {asistencia.IdEmpleado} — Fecha: {asistencia.Fecha:dd/MM/yyyy}, Hora: {asistencia.HoraSalida.Value:hh\\:mm}, Horas trabajadas: {asistencia.HorasTotales:F2}h",
                datosNuevos: JsonSerializer.Serialize(new
                {
                    asistencia.IdEmpleado,
                    Fecha = asistencia.Fecha.ToString("dd/MM/yyyy"),
                    HoraSalida = asistencia.HoraSalida.Value.ToString(@"hh\:mm"),
                    asistencia.HorasRegulares,
                    asistencia.HorasExtras,
                    asistencia.HorasTotales
                })
            );

            TempData["Mensaje"] = "Registro de finalización de jornada correctamente";
            return RedirectToAction(nameof(ListadoAsistencias));
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

                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Control de asistencia"),
                    accion: Bitacora.Acciones.Actualizar,
                    tablaAfectada: "Asistencia",
                    descripcion: $"Se editó el registro de asistencia ID {asistencia.IdAsistencia} del empleado ID {asistencia.IdEmpleado}",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        asistencia.IdEmpleado,
                        Fecha = asistencia.Fecha.ToString("dd/MM/yyyy"),
                        HoraEntrada = asistencia.HoraEntrada.ToString(@"hh\:mm"),
                        HoraSalida = asistencia.HoraSalida?.ToString(@"hh\:mm") ?? "Pendiente",
                        asistencia.TiempoAlmuerzo,
                        asistencia.HorasRegulares,
                        asistencia.HorasExtras,
                        asistencia.HorasTotales
                    })
                );

                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Asistencia actualizada correctamente";
                return RedirectToAction(nameof(ListadoAsistencias));
            }

            CargarEmpleados();
            return View(asistencia);
        }

        // Método auxiliar para cargar empleados dinámicamente desde la base de datos
        private void CargarEmpleados()
        {
            var empleados = _listarEmpleados.Obtener();

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