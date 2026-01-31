using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class CategoriaInsumoController : Controller
    {
        private IListarCategoriasInsumo _listarCategoriasInsumo;

        public CategoriaInsumoController()
        {
            _listarCategoriasInsumo = new LogicaDeNegocio.CategoriasInsumo.ListarCategoriasInsumo();
        }

        // GET: CategoriaInsumo/ListadoCategoriasInsumos
        public IActionResult ListadoCategoriasInsumos(string buscar)
        {
            ViewBag.Buscar = buscar;

            List<CategoriaInsumo> categorias;

            if (!string.IsNullOrEmpty(buscar))
            {
                categorias = _listarCategoriasInsumo.BuscarPorNombre(buscar);
            }
            else
            {
                categorias = _listarCategoriasInsumo.Obtener();
            }

            return View(categorias);
        }

        // GET: CategoriaInsumo/CrearCategoriaInsumo
        public IActionResult CrearCategoriaInsumo()
        {
            return View();
        }

        // POST: CategoriaInsumo/CrearCategoriaInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCategoriaInsumo(CategoriaInsumo categoria)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Categoría registrada correctamente";
                return RedirectToAction(nameof(ListadoCategoriasInsumos));
            }

            return View(categoria);
        }

        // GET: CategoriaInsumo/EditarCategoriaInsumo/5
        public IActionResult EditarCategoriaInsumo(int id)
        {
            var categoria = _listarCategoriasInsumo.ObtenerPorId(id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoría no encontrada";
                return RedirectToAction(nameof(ListadoCategoriasInsumos));
            }

            return View(categoria);
        }

        // POST: CategoriaInsumo/EditarCategoriaInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCategoriaInsumo(CategoriaInsumo categoria)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Categoría actualizada correctamente";
                return RedirectToAction(nameof(ListadoCategoriasInsumos));
            }

            return View(categoria);
        }

        // POST: CategoriaInsumo/EliminarCategoriaInsumo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoriaInsumo(int id)
        {
            // Aquí iría la lógica para eliminar la categoría
            TempData["Mensaje"] = "Categoría eliminada correctamente";
            return RedirectToAction(nameof(ListadoCategoriasInsumos));
        }
    }
}