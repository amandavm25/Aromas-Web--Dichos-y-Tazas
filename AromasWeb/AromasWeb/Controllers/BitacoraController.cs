using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class BitacoraController : Controller
    {
        // GET: Bitacora/ListadoBitacoras
        public IActionResult ListadoBitacoras(string buscar, string filtroModulo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            // Registros de ejemplo
            var bitacoras = new List<Bitacora>
            {
                new Bitacora
                {
                    IdBitacora = 1,
                    IdEmpleado = 1,
                    NombreEmpleado = "Juan Pérez",
                    IdModulo = 1,
                    NombreModulo = "Empleados",
                    Accion = "Crear",
                    TablaAfectada = "Empleado",
                    Descripcion = "Se registró un nuevo empleado",
                    DatosAnteriores = null,
                    DatosNuevos = "{\"Nombre\":\"María López\",\"Identificacion\":\"123456789\"}",
                    Fecha = DateTime.Now.AddHours(-2)
                },
                new Bitacora
                {
                    IdBitacora = 2,
                    IdEmpleado = 1,
                    NombreEmpleado = "Juan Pérez",
                    IdModulo = 2,
                    NombreModulo = "Roles",
                    Accion = "Actualizar",
                    TablaAfectada = "Rol",
                    Descripcion = "Se actualizó el rol Administrador",
                    DatosAnteriores = "{\"Nombre\":\"Admin\",\"Estado\":true}",
                    DatosNuevos = "{\"Nombre\":\"Administrador\",\"Estado\":true}",
                    Fecha = DateTime.Now.AddHours(-5)
                },
                new Bitacora
                {
                    IdBitacora = 3,
                    IdEmpleado = 2,
                    NombreEmpleado = "Ana García",
                    IdModulo = 3,
                    NombreModulo = "Asistencia",
                    Accion = "Registrar Entrada",
                    TablaAfectada = "Asistencia",
                    Descripcion = "Registro de entrada de jornada",
                    DatosAnteriores = null,
                    DatosNuevos = "{\"HoraEntrada\":\"08:00:00\"}",
                    Fecha = DateTime.Now.AddDays(-1)
                }
            };

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                bitacoras = bitacoras.Where(b =>
                    b.NombreEmpleado.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    b.NombreModulo.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    b.Accion.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    b.Descripcion.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrWhiteSpace(filtroModulo))
            {
                bitacoras = bitacoras.Where(b =>
                    b.NombreModulo.Equals(filtroModulo, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (fechaInicio.HasValue)
            {
                bitacoras = bitacoras.Where(b => b.Fecha.Date >= fechaInicio.Value.Date).ToList();
            }

            if (fechaFin.HasValue)
            {
                bitacoras = bitacoras.Where(b => b.Fecha.Date <= fechaFin.Value.Date).ToList();
            }

            // Ordenar por fecha descendente
            bitacoras = bitacoras.OrderByDescending(b => b.Fecha).ToList();

            return View(bitacoras);
        }
    }
}