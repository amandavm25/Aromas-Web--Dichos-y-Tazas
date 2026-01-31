using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class TipoPromocionController : Controller
    {
        private IListarTiposPromociones _listarTiposPromociones;

        public TipoPromocionController()
        {
            _listarTiposPromociones = new LogicaDeNegocio.TiposPromociones.ListarTiposPromociones();
        }

        // GET: TipoPromocion/ListadoTiposPromociones
        public IActionResult ListadoTiposPromociones(string buscar)
        {
            ViewBag.Buscar = buscar;

            List<TipoPromocion> tiposPromociones;

            if (!string.IsNullOrEmpty(buscar))
            {
                tiposPromociones = _listarTiposPromociones.BuscarPorNombre(buscar);
            }
            else
            {
                tiposPromociones = _listarTiposPromociones.Obtener();
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
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Tipo de promoción registrado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }

        // GET: TipoPromocion/EditarTipoPromocion/5
        public IActionResult EditarTipoPromocion(int id)
        {
            var tipoPromocion = _listarTiposPromociones.ObtenerPorId(id);

            if (tipoPromocion == null)
            {
                TempData["Error"] = "Tipo de promoción no encontrado";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }

        // POST: TipoPromocion/EditarTipoPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarTipoPromocion(TipoPromocion tipoPromocion)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
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
            var tipoPromocion = _listarTiposPromociones.ObtenerPorId(id);

            if (tipoPromocion == null)
            {
                TempData["Error"] = "Tipo de promoción no encontrado";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }
    }
}