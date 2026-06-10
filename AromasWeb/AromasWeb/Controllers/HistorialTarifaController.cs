using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using Microsoft.AspNetCore.Mvc;
using AromasWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AromasWeb.Controllers
{
    public class HistorialTarifaController : Controller
    {
        private IListarHistorialTarifa _listarHistorialTarifa;
        private IListarEmpleados _listarEmpleados;
        private readonly CrearBitacora _crearBitacora;

        public HistorialTarifaController()
        {
            _listarHistorialTarifa = new LogicaDeNegocio.HistorialTarifas.ListarHistorialTarifa();
            _listarEmpleados = new LogicaDeNegocio.Empleados.ListarEmpleados();
            _crearBitacora = new CrearBitacora();
        }

        // Helper de sesión
        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;

            throw new InvalidOperationException("No hay empleado autenticado en sesión.");
        }

        // GET: HistorialTarifa/ListadoTarifas
        public IActionResult ListadoTarifas(int? empleado, string estado, string buscar)
        {
            ViewBag.EmpleadoFiltro = empleado;
            ViewBag.EstadoFiltro = estado;
            ViewBag.Buscar = buscar;

            List<HistorialTarifa> historial;

            if (empleado.HasValue && !string.IsNullOrEmpty(estado))
            {
                historial = _listarHistorialTarifa.ObtenerPorEmpleado(empleado.Value)
                    .Where(h => h.EstadoVigencia.ToLower() == estado.ToLower())
                    .ToList();
            }
            else if (empleado.HasValue)
            {
                historial = _listarHistorialTarifa.ObtenerPorEmpleado(empleado.Value);
            }
            else if (!string.IsNullOrEmpty(estado))
            {
                historial = _listarHistorialTarifa.ObtenerPorEstado(estado);
            }
            else
            {
                historial = _listarHistorialTarifa.Obtener();
            }

            return View(historial);
        }

        // GET: HistorialTarifa/CrearTarifa
        public IActionResult CrearTarifa(int? idEmpleado)
        {
            CargarEmpleados();
            var nuevaTarifa = new HistorialTarifa
            {
                FechaInicio = DateTime.Today,
                FechaRegistro = DateTime.Now
            };
            if (idEmpleado.HasValue)
            {
                nuevaTarifa.IdEmpleado = idEmpleado.Value;
            }
            return View(nuevaTarifa);
        }

        // POST: HistorialTarifa/CrearTarifa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearTarifa(HistorialTarifa tarifa)
        {
            if (tarifa.FechaRegistro == DateTime.MinValue)
                tarifa.FechaRegistro = DateTime.Now;

            ModelState.Remove("NombreEmpleado");
            ModelState.Remove("IdentificacionEmpleado");
            ModelState.Remove("CargoEmpleado");

            if (ModelState.IsValid)
            {
                if (!tarifa.CumpleSalarioMinimo)
                {
                    TempData["Error"] = $"La tarifa debe ser mayor o igual a ₡{tarifa.TarifaMinimaLegal:N2} (salario mínimo legal)";
                    CargarEmpleados();
                    return View(tarifa);
                }

                if (tarifa.FechaFin.HasValue && tarifa.FechaFin.Value <= tarifa.FechaInicio)
                {
                    TempData["Error"] = "La fecha de fin debe ser posterior a la fecha de inicio";
                    CargarEmpleados();
                    return View(tarifa);
                }

                try
                {
                    // Instanciar y llamar al creador
                    var crearTarifa = new LogicaDeNegocio.HistorialTarifas.CrearHistorialTarifa();
                    tarifa.FechaRegistro = DateTime.Now;
                    crearTarifa.Crear(tarifa).GetAwaiter().GetResult();

                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Historial de tarifas"),
                        accion: Bitacora.Acciones.ActualizarTarifa,
                        tablaAfectada: "HistorialTarifa",
                        descripcion: $"Se registró nueva tarifa para el empleado ID {tarifa.IdEmpleado} — Tarifa: ₡{tarifa.TarifaHora:N2}/h, vigente desde {tarifa.FechaInicio:dd/MM/yyyy}",
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            tarifa.IdEmpleado,
                            tarifa.TarifaHora,
                            tarifa.Motivo,
                            FechaInicio = tarifa.FechaInicio.ToString("dd/MM/yyyy"),
                            FechaFin = tarifa.FechaFin?.ToString("dd/MM/yyyy")
                        })
                    );

                    TempData["Mensaje"] = "Tarifa registrada correctamente";
                    return RedirectToAction(nameof(ListadoTarifas));
                }
                catch (Exception ex)
                {
                    // Mostrar el error en vez de silenciarlo
                    TempData["Error"] = "Error al registrar la tarifa: " + ex.Message;
                    CargarEmpleados();
                    return View(tarifa);
                }
            }

            // Mostrar errores de validación del modelo
            TempData["Error"] = "Por favor corrige los errores del formulario";
            CargarEmpleados();
            return View(tarifa);
        }

        // GET: HistorialTarifa/VerHistorialEmpleado/1
        public IActionResult VerHistorialEmpleado(int id)
        {
            ViewBag.EmpleadoId = id;

            var historial = _listarHistorialTarifa.ObtenerPorEmpleado(id);

            if (historial == null || !historial.Any())
            {
                TempData["Error"] = "No se encontró historial para este empleado";
                return RedirectToAction(nameof(ListadoTarifas));
            }

            var empleado = ObtenerDatosEmpleado(id);
            ViewBag.Empleado = empleado;

            return View(historial);
        }

        // POST: HistorialTarifa/FinalizarTarifa/2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult FinalizarTarifa(int id, DateTime fechaFin)
        {
            try
            {
                //  Obtener la tarifa actual
                var tarifa = _listarHistorialTarifa.ObtenerPorId(id);
                if (tarifa == null)
                {
                    TempData["Error"] = "Tarifa no encontrada";
                    return RedirectToAction(nameof(ListadoTarifas));
                }

                // Actualizar la fecha de fin en BD
                tarifa.FechaFin = fechaFin;
                var actualizarTarifa = new LogicaDeNegocio.HistorialTarifas.ActualizarHistorialTarifa();
                actualizarTarifa.Actualizar(tarifa);

                // Registrar bitácora
                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Historial de tarifas"),
                    accion: Bitacora.Acciones.FinalizarTarifa,
                    tablaAfectada: "HistorialTarifa",
                    descripcion: $"Se finalizó la tarifa ID {id} del empleado ID {tarifa.IdEmpleado} — Fecha de fin: {fechaFin:dd/MM/yyyy}",
                    datosAnteriores: JsonSerializer.Serialize(new
                    {
                        tarifa.IdEmpleado,
                        tarifa.TarifaHora,
                        FechaInicio = tarifa.FechaInicio.ToString("dd/MM/yyyy"),
                        FechaFin = "Sin definir"
                    }),
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        FechaFin = fechaFin.ToString("dd/MM/yyyy")
                    })
                );

                TempData["Mensaje"] = "Tarifa finalizada correctamente";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al finalizar la tarifa: " + ex.Message;
            }

            return RedirectToAction(nameof(ListadoTarifas));
        }

        // GET: HistorialTarifa/MiHistorial
        public IActionResult MiHistorial()
        {
            int idEmpleadoActual = 1;
            ViewBag.IdEmpleado = idEmpleadoActual;

            var empleadoActual = ObtenerDatosEmpleado(idEmpleadoActual);
            ViewBag.EmpleadoActual = empleadoActual;

            var miHistorial = _listarHistorialTarifa.ObtenerPorEmpleado(idEmpleadoActual);

            return View(miHistorial);
        }

        // Consultar la base de datos
        private Empleado ObtenerDatosEmpleado(int id)
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
                    Console.WriteLine($"Error al obtener empleado: {ex.Message}");
                    return null;
                }
            }
        }

        // GET: HistorialTarifa/GenerarPdf/1
        public IActionResult GenerarPdf(int id)
        {
            var historial = _listarHistorialTarifa.ObtenerPorEmpleado(id);

            if (historial == null || !historial.Any())
            {
                TempData["Error"] = "No se encontró historial para este empleado";
                return RedirectToAction(nameof(ListadoTarifas));
            }

            var empleado = ObtenerDatosEmpleado(id);
            if (empleado == null)
            {
                TempData["Error"] = "Empleado no encontrado";
                return RedirectToAction(nameof(ListadoTarifas));
            }

            try
            {
                byte[] pdf = PdfService.GenerarHistorialTarifas(empleado, historial);

                string nombreArchivo = $"HistorialTarifas_{empleado.Nombre}_{empleado.Apellidos}_{DateTime.Now:yyyyMMdd}.pdf"
                    .Replace(" ", "_");

                return File(pdf, "application/pdf", nombreArchivo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al generar el PDF: " + ex.Message;
                return RedirectToAction(nameof(VerHistorialEmpleado), new { id });
            }
        }

        // Método auxiliar privado
        private void CargarEmpleados()
        {
            var empleados = _listarEmpleados.Obtener();

            var empleadosDropdown = empleados.Select(e => new
            {
                IdEmpleado = e.IdEmpleado,
                NombreCompleto = $"{e.Nombre} {e.Apellidos}",
                Cargo = e.Cargo,
                Identificacion = e.Identificacion,
                // Obtener tarifa vigente de cada empleado
                TarifaActual = _listarHistorialTarifa
                    .ObtenerTarifaActualPorEmpleado(e.IdEmpleado)?.TarifaHora.ToString("F2") ?? ""
            }).ToList();

            ViewBag.Empleados = empleadosDropdown;
        }
    }
}