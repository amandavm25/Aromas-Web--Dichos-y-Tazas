using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.MovimientoInsumo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class MovimientoInsumoController : Controller
    {
        private IListarMovimientos _listarMovimientos;

        public MovimientoInsumoController()
        {
            _listarMovimientos = new LogicaDeNegocio.MovimientosInsumo.ListarMovimientos();
        }

        // GET: MovimientoInsumo/HistorialMovimientos
        public IActionResult HistorialMovimientos(string buscar, string tipo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Tipo = tipo;
            ViewBag.FechaDesde = fechaDesde?.ToString("yyyy-MM-dd");
            ViewBag.FechaHasta = fechaHasta?.ToString("yyyy-MM-dd");

            List<MovimientoInsumo> movimientos;

            // Aplicar filtros según los parámetros recibidos
            if (!string.IsNullOrEmpty(buscar) || !string.IsNullOrEmpty(tipo) || fechaDesde.HasValue || fechaHasta.HasValue)
            {
                movimientos = _listarMovimientos.BuscarConFiltros(buscar, tipo, fechaDesde, fechaHasta);
            }
            else
            {
                movimientos = _listarMovimientos.Obtener();
            }

            return View(movimientos);
        }

        // GET: MovimientoInsumo/RegistrarEntrada
        public IActionResult RegistrarEntrada()
        {
            var movimiento = new MovimientoInsumo
            {
                TipoMovimiento = "E",
                FechaMovimiento = DateTime.Now
            };

            return View(movimiento);
        }

        // POST: MovimientoInsumo/RegistrarEntrada
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarEntrada(MovimientoInsumo movimiento)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Entrada registrada correctamente";
                return RedirectToAction(nameof(HistorialMovimientos));
            }

            return View(movimiento);
        }

        // GET: MovimientoInsumo/RegistrarSalida
        public IActionResult RegistrarSalida()
        {
            var movimiento = new MovimientoInsumo
            {
                TipoMovimiento = "S",
                FechaMovimiento = DateTime.Now
            };

            return View(movimiento);
        }

        // POST: MovimientoInsumo/RegistrarSalida
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarSalida(MovimientoInsumo movimiento)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Salida registrada correctamente";
                return RedirectToAction(nameof(HistorialMovimientos));
            }

            return View(movimiento);
        }

        // GET: MovimientoInsumo/DetalleMovimiento/5
        public IActionResult DetalleMovimiento(int id)
        {
            var movimiento = _listarMovimientos.ObtenerPorId(id);

            if (movimiento == null)
            {
                TempData["Error"] = "Movimiento no encontrado";
                return RedirectToAction(nameof(HistorialMovimientos));
            }

            return View(movimiento);
        }

        // API para obtener estadísticas (opcional, para usar con AJAX)
        [HttpGet]
        public JsonResult ObtenerEstadisticas(DateTime? fechaDesde, DateTime? fechaHasta)
        {
            List<MovimientoInsumo> movimientos;

            if (fechaDesde.HasValue && fechaHasta.HasValue)
            {
                movimientos = _listarMovimientos.BuscarPorRangoFechas(fechaDesde.Value, fechaHasta.Value);
            }
            else
            {
                // Por defecto, movimientos del mes actual
                var primerDiaMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
                var ultimoDiaMes = primerDiaMes.AddMonths(1).AddDays(-1);
                movimientos = _listarMovimientos.BuscarPorRangoFechas(primerDiaMes, ultimoDiaMes);
            }

            var totalMovimientos = movimientos.Count;
            var totalEntradas = movimientos.Count(m => m.TipoMovimiento == "E");
            var totalSalidas = movimientos.Count(m => m.TipoMovimiento == "S");
            var valorEntradas = movimientos.Where(m => m.TipoMovimiento == "E").Sum(m => m.ValorMovimiento);

            var estadisticas = new
            {
                totalMovimientos,
                totalEntradas,
                totalSalidas,
                valorEntradas,
                promedioDiario = totalMovimientos / DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month)
            };

            return Json(estadisticas);
        }
    }
}