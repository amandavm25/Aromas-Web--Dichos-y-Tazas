using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.MovimientoInsumo;
using AromasWeb.Abstracciones.Logica.Insumo;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class MovimientoInsumoController : Controller
    {
        private IListarMovimientos _listarMovimientos;
        private IListarInsumos _listarInsumos;

        public MovimientoInsumoController()
        {
            _listarMovimientos = new LogicaDeNegocio.MovimientosInsumo.ListarMovimientos();
            _listarInsumos = new LogicaDeNegocio.Insumos.ListarInsumos();
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

        // GET: MovimientoInsumo/RegistrarMovimiento
        public IActionResult RegistrarMovimiento(int? idInsumo)
        {
            // Siempre cargar todos los insumos para el combobox
            ViewBag.TodosInsumos = _listarInsumos.Obtener();

            // Si viene idInsumo, precargar el insumo en el panel de info
            if (idInsumo.HasValue)
            {
                var insumo = _listarInsumos.ObtenerPorId(idInsumo.Value);
                ViewBag.Insumo = insumo;
            }

            var movimiento = new MovimientoInsumo
            {
                IdInsumo = idInsumo ?? 0,
                FechaMovimiento = DateTime.Now,
                TipoMovimiento = "E"
            };

            return View(movimiento);
        }

        // POST: MovimientoInsumo/RegistrarMovimiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarMovimiento(MovimientoInsumo movimiento)
        {
            if (ModelState.IsValid)
            {
                movimiento.FechaMovimiento = DateTime.Now;
                movimiento.IdEmpleado = 1; 
                TempData["Mensaje"] = "Movimiento registrado correctamente";
                return RedirectToAction(nameof(HistorialMovimientos));
            }

            // Si hay error, recargar combobox e info del insumo
            ViewBag.TodosInsumos = _listarInsumos.Obtener();
            if (movimiento.IdInsumo > 0)
                ViewBag.Insumo = _listarInsumos.ObtenerPorId(movimiento.IdInsumo);

            return View(movimiento);
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