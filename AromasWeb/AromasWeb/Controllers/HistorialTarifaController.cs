using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    // Clase auxiliar para datos del empleado (sin dynamic)
    public class EmpleadoInfo
    {
        public int IdEmpleado { get; set; }
        public string NombreCompleto { get; set; }
        public string Identificacion { get; set; }
        public string Cargo { get; set; }
        public string TiempoServicio { get; set; }
        public DateTime FechaContratacion { get; set; }
    }

    public class HistorialTarifaController : Controller
    {
        // GET: HistorialTarifa/ListadoTarifas
        public IActionResult ListadoTarifas(int? empleado, string estado, string buscar)
        {
            ViewBag.EmpleadoFiltro = empleado;
            ViewBag.EstadoFiltro = estado;
            ViewBag.Buscar = buscar;

            // Los datos se muestran directamente en la vista (quemados en ListadoTarifas.cshtml)
            var historial = new List<HistorialTarifa>();

            return View(historial);
        }

        // GET: HistorialTarifa/CrearTarifa
        public IActionResult CrearTarifa(int? idEmpleado)
        {
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
                // Validar que la tarifa cumpla con el salario mínimo
                if (!tarifa.CumpleSalarioMinimo)
                {
                    TempData["Error"] = $"La tarifa debe ser mayor o igual a ₡{tarifa.TarifaMinimaLegal:N2} (salario mínimo legal)";
                    return View(tarifa);
                }

                // Validar que FechaFin sea posterior a FechaInicio si se proporciona
                if (tarifa.FechaFin.HasValue && tarifa.FechaFin.Value <= tarifa.FechaInicio)
                {
                    TempData["Error"] = "La fecha de fin debe ser posterior a la fecha de inicio";
                    return View(tarifa);
                }

                TempData["Mensaje"] = "Tarifa registrada correctamente";
                return RedirectToAction(nameof(ListadoTarifas));
            }

            return View(tarifa);
        }

        // GET: HistorialTarifa/VerHistorialEmpleado/1
        public IActionResult VerHistorialEmpleado(int id)
        {
            ViewBag.EmpleadoId = id;

            // DATOS QUEMADOS DEL EMPLEADO
            var empleado = new EmpleadoInfo();

            if (id == 1)
            {
                empleado.IdEmpleado = 1;
                empleado.NombreCompleto = "Kenneth Salas";
                empleado.Identificacion = "1-2345-6789";
                empleado.Cargo = "Chef Principal";
                empleado.FechaContratacion = new DateTime(2023, 1, 15);
            }
            else if (id == 2)
            {
                empleado.IdEmpleado = 2;
                empleado.NombreCompleto = "María González";
                empleado.Identificacion = "2-3456-7890";
                empleado.Cargo = "Asistente de Cocina";
                empleado.FechaContratacion = new DateTime(2023, 3, 20);
            }
            else if (id == 3)
            {
                empleado.IdEmpleado = 3;
                empleado.NombreCompleto = "David Rodríguez";
                empleado.Identificacion = "3-4567-8901";
                empleado.Cargo = "Barista";
                empleado.FechaContratacion = new DateTime(2023, 2, 10);
            }
            else
            {
                empleado.IdEmpleado = 4;
                empleado.NombreCompleto = "Ana Jiménez";
                empleado.Identificacion = "4-5678-9012";
                empleado.Cargo = "Cajera";
                empleado.FechaContratacion = new DateTime(2023, 4, 5);
            }

            ViewBag.Empleado = empleado;

            // HISTORIAL QUEMADO DEL EMPLEADO
            var historial = new List<HistorialTarifa>();

            if (id == 1)
            {
                historial.Add(new HistorialTarifa
                {
                    IdHistorialTarifa = 1,
                    IdEmpleado = 1,
                    TarifaHora = 2500.00m,
                    FechaInicio = new DateTime(2023, 1, 15),
                    FechaFin = new DateTime(2024, 6, 30),
                    Motivo = "Tarifa inicial al contratar",
                    FechaRegistro = new DateTime(2023, 1, 15, 8, 0, 0)
                });
                historial.Add(new HistorialTarifa
                {
                    IdHistorialTarifa = 2,
                    IdEmpleado = 1,
                    TarifaHora = 2800.00m,
                    FechaInicio = new DateTime(2024, 7, 1),
                    FechaFin = null,
                    Motivo = "Aumento por desempeño excepcional",
                    FechaRegistro = new DateTime(2024, 6, 25, 10, 30, 0)
                });
            }
            else if (id == 2)
            {
                historial.Add(new HistorialTarifa
                {
                    IdHistorialTarifa = 3,
                    IdEmpleado = 2,
                    TarifaHora = 1900.00m,
                    FechaInicio = new DateTime(2023, 3, 20),
                    FechaFin = null,
                    Motivo = "Tarifa inicial - Asistente de cocina",
                    FechaRegistro = new DateTime(2023, 3, 20, 9, 0, 0)
                });
            }
            else if (id == 3)
            {
                historial.Add(new HistorialTarifa
                {
                    IdHistorialTarifa = 4,
                    IdEmpleado = 3,
                    TarifaHora = 2200.00m,
                    FechaInicio = new DateTime(2023, 2, 10),
                    FechaFin = new DateTime(2024, 8, 31),
                    Motivo = "Tarifa inicial - Barista certificado",
                    FechaRegistro = new DateTime(2023, 2, 10, 8, 0, 0)
                });
                historial.Add(new HistorialTarifa
                {
                    IdHistorialTarifa = 5,
                    IdEmpleado = 3,
                    TarifaHora = 2400.00m,
                    FechaInicio = new DateTime(2024, 9, 1),
                    FechaFin = null,
                    Motivo = "Aumento semestral por antigüedad",
                    FechaRegistro = new DateTime(2024, 8, 28, 14, 20, 0)
                });
            }
            else if (id == 4)
            {
                historial.Add(new HistorialTarifa
                {
                    IdHistorialTarifa = 6,
                    IdEmpleado = 4,
                    TarifaHora = 1800.00m,
                    FechaInicio = new DateTime(2023, 4, 5),
                    FechaFin = null,
                    Motivo = "Tarifa inicial - Cajera",
                    FechaRegistro = new DateTime(2023, 4, 5, 8, 0, 0)
                });
            }

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
            // Simulando el empleado actual (ID = 1 - Kenneth Salas)
            int idEmpleadoActual = 1;
            ViewBag.IdEmpleado = idEmpleadoActual;

            // DATOS QUEMADOS DEL EMPLEADO ACTUAL
            var empleadoActual = new EmpleadoInfo
            {
                IdEmpleado = 1,
                NombreCompleto = "Kenneth Salas",
                Identificacion = "1-2345-6789",
                Cargo = "Chef Principal",
                TiempoServicio = "1 año 10 meses",
                FechaContratacion = new DateTime(2023, 1, 15)
            };

            ViewBag.EmpleadoActual = empleadoActual;

            // MI HISTORIAL QUEMADO
            var miHistorial = new List<HistorialTarifa>
            {
                new HistorialTarifa
                {
                    IdHistorialTarifa = 1,
                    IdEmpleado = 1,
                    TarifaHora = 2500.00m,
                    FechaInicio = new DateTime(2023, 1, 15),
                    FechaFin = new DateTime(2024, 6, 30),
                    Motivo = "Tarifa inicial al contratar",
                    FechaRegistro = new DateTime(2023, 1, 15, 8, 0, 0)
                },
                new HistorialTarifa
                {
                    IdHistorialTarifa = 2,
                    IdEmpleado = 1,
                    TarifaHora = 2800.00m,
                    FechaInicio = new DateTime(2024, 7, 1),
                    FechaFin = null,
                    Motivo = "Aumento por desempeño excepcional",
                    FechaRegistro = new DateTime(2024, 6, 25, 10, 30, 0)
                }
            };

            return View(miHistorial);
        }
    }
}