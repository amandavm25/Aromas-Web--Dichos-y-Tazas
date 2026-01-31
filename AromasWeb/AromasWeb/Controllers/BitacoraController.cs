using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Bitacora;
using System;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class BitacoraController : Controller
    {
        private IListarBitacora _listarBitacora;

        public BitacoraController()
        {
            _listarBitacora = new LogicaDeNegocio.Bitacoras.ListarBitacoras();
        }

        // GET: Bitacora/ListadoBitacoras
        public IActionResult ListadoBitacoras(string buscar, string filtroModulo, DateTime? fechaInicio, DateTime? fechaFin)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            List<Bitacora> bitacoras;

            // Si hay filtros, buscar con ellos
            if (!string.IsNullOrEmpty(buscar) || !string.IsNullOrEmpty(filtroModulo) || fechaInicio.HasValue || fechaFin.HasValue)
            {
                bitacoras = _listarBitacora.BuscarPorFiltros(buscar, filtroModulo, fechaInicio, fechaFin);
            }
            else
            {
                // Si no hay filtros, obtener todos
                bitacoras = _listarBitacora.Obtener();
            }

            return View(bitacoras);
        }

        // GET: Bitacora/DetallesBitacora/5
        public IActionResult DetallesBitacora(int id)
        {
            var bitacora = _listarBitacora.ObtenerPorId(id);

            if (bitacora == null)
            {
                TempData["Error"] = "Registro de bitácora no encontrado";
                return RedirectToAction(nameof(ListadoBitacoras));
            }

            return View(bitacora);
        }

        // GET: Bitacora/BitacorasPorEmpleado/5
        public IActionResult BitacorasPorEmpleado(int idEmpleado)
        {
            var bitacoras = _listarBitacora.ObtenerPorEmpleado(idEmpleado);
            ViewBag.IdEmpleado = idEmpleado;
            return View("ListadoBitacoras", bitacoras);
        }

        // GET: Bitacora/BitacorasPorModulo/5
        public IActionResult BitacorasPorModulo(int idModulo)
        {
            var bitacoras = _listarBitacora.ObtenerPorModulo(idModulo);
            ViewBag.IdModulo = idModulo;
            return View("ListadoBitacoras", bitacoras);
        }
    }
}