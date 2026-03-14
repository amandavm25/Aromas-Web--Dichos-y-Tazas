using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace AromasWeb.Controllers
{
    public class PromocionController : Controller
    {
        private IListarPromociones _listarPromociones;
        private IListarTiposPromociones _listarTiposPromociones;
        private IListarRecetas _listarRecetas;
        private readonly CrearBitacora _crearBitacora;

        public PromocionController()
        {
            _listarPromociones = new LogicaDeNegocio.Promociones.ListarPromociones();
            _listarTiposPromociones = new LogicaDeNegocio.TiposPromociones.ListarTiposPromociones();
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
            _crearBitacora = new CrearBitacora();
        }

        // Helper de sesión
        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;

            return 1;
        }

        // GET: Promocion/ListadoPromociones
        public IActionResult ListadoPromociones(string buscar, int? tipo, string vigencia)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Tipo = tipo;
            ViewBag.Vigencia = vigencia;

            List<Promocion> promociones;

            try
            {
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();

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
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las promociones: {ex.Message}";
                promociones = new List<Promocion>();
                ViewBag.TodosTipos = new List<TipoPromocion>();
            }

            return View(promociones);
        }

        // GET: Promocion/ObtenerRecetasPromocion?id=5
        [HttpGet]
        public IActionResult ObtenerRecetasPromocion(int id)
        {
            var promocion = _listarPromociones.ObtenerPorId(id);

            if (promocion == null)
                return NotFound();

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

        // GET: Promocion/CrearPromocion
        public IActionResult CrearPromocion()
        {
            try
            {
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar los datos: {ex.Message}";
                ViewBag.TodosTipos = new List<TipoPromocion>();
                ViewBag.RecetasDisponibles = new List<Receta>();
            }

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

            try
            {
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            }
            catch
            {
                ViewBag.TodosTipos = new List<TipoPromocion>();
                ViewBag.RecetasDisponibles = new List<Receta>();
            }

            return View(promocion);
        }

        // GET: Promocion/EditarPromocion/5
        public IActionResult EditarPromocion(int id)
        {
            try
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
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la promoción: {ex.Message}";
                return RedirectToAction(nameof(ListadoPromociones));
            }
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

            try
            {
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            }
            catch
            {
                ViewBag.TodosTipos = new List<TipoPromocion>();
                ViewBag.RecetasDisponibles = new List<Receta>();
            }

            return View(promocion);
        }

        // POST: Promocion/EliminarPromocion/5
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