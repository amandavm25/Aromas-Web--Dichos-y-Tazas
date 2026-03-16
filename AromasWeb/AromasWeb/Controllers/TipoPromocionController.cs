using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace AromasWeb.Controllers
{
    public class TipoPromocionController : Controller
    {
        private IListarTiposPromociones _listarTiposPromociones;
        private readonly CrearBitacora _crearBitacora;
        private ICrearTiposPromociones _crearTiposPromociones;
        private IEditarTiposPromociones _editarTiposPromociones;
        

        public TipoPromocionController()
        {
            _listarTiposPromociones = new LogicaDeNegocio.TiposPromociones.ListarTiposPromociones();
            _crearBitacora = new CrearBitacora();
            _crearTiposPromociones = new LogicaDeNegocio.TiposPromociones.CrearTiposPromociones();
            _editarTiposPromociones = new LogicaDeNegocio.TiposPromociones.EditarTiposPromociones();
        }

        // Helper de sesión
        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;

            return 1;
        }

        // GET: TipoPromocion/ListadoTiposPromociones
        public IActionResult ListadoTiposPromociones(string buscar)
        {
            ViewBag.Buscar = buscar;

            List<TipoPromocion> tiposPromociones;

            try
            {
                tiposPromociones = !string.IsNullOrEmpty(buscar)
                    ? _listarTiposPromociones.BuscarPorNombre(buscar)
                    : _listarTiposPromociones.Obtener();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar los tipos de promoción: {ex.Message}";
                tiposPromociones = new List<TipoPromocion>();
            }

            return View(tiposPromociones);
        }

        // GET: TipoPromocion/CrearTipoPromocion
        public IActionResult CrearTipoPromocion()
        {
            return View();
        }

        // POST: TipoPromocion/CrearTipoPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearTipoPromocion(TipoPromocion tipoPromocion)
        {
            if (ModelState.IsValid)
            { 
                _crearTiposPromociones.Ejecutar(tipoPromocion);


            TempData["Mensaje"] = "Tipo de promoción registrado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }

        // GET: TipoPromocion/EditarTipoPromocion/5
        public IActionResult EditarTipoPromocion(int id)
        {
            try
            {
                var tipoPromocion = _listarTiposPromociones.ObtenerPorId(id);

                if (tipoPromocion == null)
                {
                    TempData["Error"] = "Tipo de promoción no encontrado";
                    return RedirectToAction(nameof(ListadoTiposPromociones));
                }

                return View(tipoPromocion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el tipo de promoción: {ex.Message}";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }
        }

        // POST: TipoPromocion/EditarTipoPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarTipoPromocion(TipoPromocion tipoPromocion)
        {
            if (ModelState.IsValid)

            {
                _editarTiposPromociones.Ejecutar(tipoPromocion);


                TempData["Mensaje"] = "Tipo de promoción actualizado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }

        // POST: TipoPromocion/EliminarTipoPromocion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarTipoPromocion(int id)
        {
            // Aquí iría la lógica para verificar si tiene promociones asociadas
            // Si tiene promociones asociadas, no se puede eliminar
            // Aquí iría la lógica para eliminar el tipo de promoción
            TempData["Mensaje"] = "Tipo de promoción eliminado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(ListadoTiposPromociones));
        }

        // GET: TipoPromocion/DetallesTipoPromocion/5
        public IActionResult DetallesTipoPromocion(int id)
        {
            try
            {
                var tipoPromocion = _listarTiposPromociones.ObtenerPorId(id);

                if (tipoPromocion == null)
                {
                    TempData["Error"] = "Tipo de promoción no encontrado";
                    return RedirectToAction(nameof(ListadoTiposPromociones));
                }

                return View(tipoPromocion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el tipo de promoción: {ex.Message}";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }
        }
    }
}