using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class CategoriaRecetaController : Controller
    {
        // GET: CategoriaReceta/ListadoCategoriasRecetas
        public IActionResult ListadoCategoriasRecetas(string buscar)
        {
            ViewBag.Buscar = buscar;

            // Categorías de ejemplo
            var categorias = new List<CategoriaReceta>
            {
                new CategoriaReceta
                {
                    IdCategoriaReceta = 1,
                    Nombre = "Postres",
                    Descripcion = "Recetas de postres y dulces",
                    Estado = true
                },
                new CategoriaReceta
                {
                    IdCategoriaReceta = 2,
                    Nombre = "Panes",
                    Descripcion = "Recetas de panes artesanales",
                    Estado = true
                },
                new CategoriaReceta
                {
                    IdCategoriaReceta = 3,
                    Nombre = "Pasteles",
                    Descripcion = "Recetas de pasteles y tortas",
                    Estado = true
                },
                new CategoriaReceta
                {
                    IdCategoriaReceta = 4,
                    Nombre = "Galletas",
                    Descripcion = "Recetas de galletas y cookies",
                    Estado = true
                },
                new CategoriaReceta
                {
                    IdCategoriaReceta = 5,
                    Nombre = "Bebidas",
                    Descripcion = "Bebidas frías y calientes",
                    Estado = true
                }
            };

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
            // Categoría de ejemplo
            var categoria = new CategoriaReceta
            {
                IdCategoriaReceta = id,
                Nombre = "Postres",
                Descripcion = "Recetas de postres y dulces",
                Estado = true
            };

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