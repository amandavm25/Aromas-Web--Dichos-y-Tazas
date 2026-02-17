using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class HistorialTarifaController : Controller
    {
        private IListarHistorialTarifa _listarHistorialTarifa;
        private IListarEmpleados _listarEmpleados;

        public HistorialTarifaController()
        {
            _listarHistorialTarifa = new LogicaDeNegocio.HistorialTarifas.ListarHistorialTarifa();
            _listarEmpleados = new LogicaDeNegocio.Empleados.ListarEmpleados();
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
            TempData["Mensaje"] = "Tarifa registrada correctamente";
            return RedirectToAction(nameof(ListadoTarifas));
        }
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
            TempData["Mensaje"] = "Tarifa finalizada correctamente";
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
                    Console.WriteLine($"Error al obtener empleado: {ex.Message}");
                    return null;
                }
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
                Identificacion = e.Identificacion
            }).ToList();

            ViewBag.Empleados = empleadosDropdown;
        }
    }
}