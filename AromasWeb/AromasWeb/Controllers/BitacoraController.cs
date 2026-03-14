using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Bitacora;
using System;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class BitacoraController : Controller
    {
        private readonly IListarBitacora _listarBitacora;

        public BitacoraController()
        {
            _listarBitacora = new LogicaDeNegocio.Bitacoras.ListarBitacoras();
        }

        // GET: Bitacora/ListadoBitacoras
        public IActionResult ListadoBitacoras(
            string buscar = null,
            string filtroModulo = null,
            DateTime? fechaInicio = null,
            DateTime? fechaFin = null)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;
            ViewBag.FechaInicio = fechaInicio?.ToString("yyyy-MM-dd");
            ViewBag.FechaFin = fechaFin?.ToString("yyyy-MM-dd");

            bool hayFiltros = !string.IsNullOrEmpty(buscar)
                           || !string.IsNullOrEmpty(filtroModulo)
                           || fechaInicio.HasValue
                           || fechaFin.HasValue;

            List<Bitacora> bitacoras = hayFiltros
                ? _listarBitacora.BuscarPorFiltros(buscar, filtroModulo, fechaInicio, fechaFin)
                : _listarBitacora.Obtener();

            return View(bitacoras);
        }

        // GET: Bitacora/DetallesBitacora/5

        public IActionResult DetallesBitacora(int id)
        {
            if (id <= 0)
            {
                TempData["Error"] = "El registro de auditoría no se encuentra disponible";
                return RedirectToAction(nameof(ListadoBitacoras));
            }

            var bitacora = _listarBitacora.ObtenerPorId(id);

            if (bitacora == null)
            {
                // Registro inexistente
                TempData["Error"] = "El registro de auditoría no se encuentra disponible";
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