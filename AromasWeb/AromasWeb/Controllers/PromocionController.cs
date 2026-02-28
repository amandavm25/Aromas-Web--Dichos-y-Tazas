using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.Abstracciones.Logica.Receta;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class PromocionController : Controller
    {
        private IListarPromociones _listarPromociones;
        private IListarTiposPromociones _listarTiposPromociones;
        private IListarRecetas _listarRecetas;

        public PromocionController()
        {
            _listarPromociones = new LogicaDeNegocio.Promociones.ListarPromociones();
            _listarTiposPromociones = new LogicaDeNegocio.TiposPromociones.ListarTiposPromociones();
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
        }

        // ============================================================
        // GET: Promocion/ListadoPromociones
        // ============================================================
        public IActionResult ListadoPromociones(string buscar, int? tipo, string vigencia)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Tipo = tipo;
            ViewBag.Vigencia = vigencia;
            ViewBag.TodosTipos = _listarTiposPromociones.Obtener();

            List<Promocion> promociones;

            if (!string.IsNullOrEmpty(buscar) && tipo.HasValue && !string.IsNullOrEmpty(vigencia))
                promociones = _listarPromociones.BuscarPorNombre(buscar)
                    .FindAll(p => p.IdTipoPromocion == tipo.Value && p.VigenciaTexto == vigencia);
            else if (!string.IsNullOrEmpty(buscar) && tipo.HasValue)
                promociones = _listarPromociones.BuscarPorNombre(buscar)
                    .FindAll(p => p.IdTipoPromocion == tipo.Value);
            else if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(vigencia))
                promociones = _listarPromociones.BuscarPorNombre(buscar)
                    .FindAll(p => p.VigenciaTexto == vigencia);
            else if (tipo.HasValue && !string.IsNullOrEmpty(vigencia))
                promociones = _listarPromociones.BuscarPorTipo(tipo.Value)
                    .FindAll(p => p.VigenciaTexto == vigencia);
            else if (!string.IsNullOrEmpty(buscar))
                promociones = _listarPromociones.BuscarPorNombre(buscar);
            else if (tipo.HasValue)
                promociones = _listarPromociones.BuscarPorTipo(tipo.Value);
            else if (!string.IsNullOrEmpty(vigencia))
                promociones = _listarPromociones.BuscarPorVigencia(vigencia);
            else
                promociones = _listarPromociones.Obtener();

            return View(promociones);
        }

        // ============================================================
        // GET: Promocion/ObtenerRecetasPromocion?id=5
        // Endpoint JSON para el modal de detalles
        // ============================================================
        [HttpGet]
        public IActionResult ObtenerRecetasPromocion(int id)
        {
            var promocion = _listarPromociones.ObtenerPorId(id);

            if (promocion == null)
                return NotFound();

            // Devolver solo los campos que necesita el modal
            var recetas = (promocion.Recetas ?? new List<PromocionReceta>())
                .Select(r => new
                {
                    r.IdReceta,
                    r.NombreReceta,
                    r.CategoriaReceta,
                    r.PrecioOriginal,
                    r.PrecioPromocional,
                    Ahorro = r.PrecioOriginal - r.PrecioPromocional
                });

            return Json(recetas);
        }

        // ============================================================
        // GET: Promocion/CrearPromocion
        // ============================================================
        public IActionResult CrearPromocion()
        {
            ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
            ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            return View();
        }

        // POST: Promocion/CrearPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPromocion(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Promoción registrada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
            ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            return View(promocion);
        }

        // ============================================================
        // GET: Promocion/EditarPromocion/5
        // ============================================================
        public IActionResult EditarPromocion(int id)
        {
            var promocion = _listarPromociones.ObtenerPorId(id);

            if (promocion == null)
            {
                TempData["Error"] = "Promoción no encontrada";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
            ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            return View(promocion);
        }

        // POST: Promocion/EditarPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPromocion(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Promoción actualizada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
            ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            return View(promocion);
        }

        // ============================================================
        // POST: Promocion/EliminarPromocion/5
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPromocion(int id)
        {
            // Aquí iría la lógica para eliminar la promoción
            TempData["Mensaje"] = "Promoción eliminada correctamente";
            return RedirectToAction(nameof(ListadoPromociones));
        }
    }
}