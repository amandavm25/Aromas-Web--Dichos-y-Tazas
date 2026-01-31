using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class CategoriaRecetaController : Controller
    {
        private IListarCategoriasReceta _listarCategoriasReceta;

        public CategoriaRecetaController()
        {
            _listarCategoriasReceta = new LogicaDeNegocio.CategoriasReceta.ListarCategoriasReceta();
        }

        // GET: CategoriaReceta/ListadoCategoriasRecetas
        public IActionResult ListadoCategoriasRecetas(string buscar)
        {
            ViewBag.Buscar = buscar;

            List<CategoriaReceta> categorias;

            if (!string.IsNullOrEmpty(buscar))
            {
                categorias = _listarCategoriasReceta.BuscarPorNombre(buscar);
            }
            else
            {
                categorias = _listarCategoriasReceta.Obtener();
            }

            return View(categorias);
        }

        // GET: CategoriaReceta/CrearCategoriaReceta
        public IActionResult CrearCategoriaReceta()
        {
            return View();
        }

        // POST: CategoriaReceta/CrearCategoriaReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCategoriaReceta(CategoriaReceta categoria)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Categoría de receta registrada correctamente";
                return RedirectToAction(nameof(ListadoCategoriasRecetas));
            }

            return View(categoria);
        }

        // GET: CategoriaReceta/EditarCategoriaReceta/5
        public IActionResult EditarCategoriaReceta(int id)
        {
            var categoria = _listarCategoriasReceta.ObtenerPorId(id);

            if (categoria == null)
            {
                TempData["Error"] = "Categoría de receta no encontrada";
                return RedirectToAction(nameof(ListadoCategoriasRecetas));
            }

            return View(categoria);
        }

        // POST: CategoriaReceta/EditarCategoriaReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCategoriaReceta(CategoriaReceta categoria)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Categoría de receta actualizada correctamente";
                return RedirectToAction(nameof(ListadoCategoriasRecetas));
            }

            return View(categoria);
        }

        // POST: CategoriaReceta/EliminarCategoriaReceta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoriaReceta(int id)
        {
            // Aquí iría la lógica para eliminar la categoría
            TempData["Mensaje"] = "Categoría de receta eliminada correctamente";
            return RedirectToAction(nameof(ListadoCategoriasRecetas));
        }
    }
}