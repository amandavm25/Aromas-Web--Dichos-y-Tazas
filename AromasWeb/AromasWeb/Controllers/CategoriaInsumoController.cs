using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class CategoriaInsumoController : Controller
    {
        // GET: CategoriaInsumo/ListadoCategoriasInsumos
        public IActionResult ListadoCategoriasInsumos(string buscar)
        {
            ViewBag.Buscar = buscar;

            // Categorías de ejemplo
            var categorias = new List<CategoriaInsumo>
            {
                new CategoriaInsumo
                {
                    IdCategoria = 1,
                    NombreCategoria = "Harinas",
                    Descripcion = "Harinas y derivados para panificación",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                },
                new CategoriaInsumo
                {
                    IdCategoria = 2,
                    NombreCategoria = "Endulzantes",
                    Descripcion = "Azúcares, mieles y edulcorantes",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                },
                new CategoriaInsumo
                {
                    IdCategoria = 3,
                    NombreCategoria = "Lácteos",
                    Descripcion = "Leche, mantequilla, crema y derivados",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                },
                new CategoriaInsumo
                {
                    IdCategoria = 4,
                    NombreCategoria = "Ingredientes Frescos",
                    Descripcion = "Frutas, huevos y productos frescos",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                },
                new CategoriaInsumo
                {
                    IdCategoria = 5,
                    NombreCategoria = "Chocolates",
                    Descripcion = "Chocolate en polvo, barra y coberturas",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                },
                new CategoriaInsumo
                {
                    IdCategoria = 6,
                    NombreCategoria = "Esencias",
                    Descripcion = "Vainilla, extractos y aromatizantes",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                },
                new CategoriaInsumo
                {
                    IdCategoria = 7,
                    NombreCategoria = "Leudantes",
                    Descripcion = "Levadura, polvo de hornear y bicarbonato",
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddMonths(-6),
                    FechaActualizacion = DateTime.Now
                }
            };

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
            // Categoría de ejemplo
            var categoria = new CategoriaInsumo
            {
                IdCategoria = id,
                NombreCategoria = "Harinas",
                Descripcion = "Harinas y derivados para panificación",
                Estado = true,
                FechaCreacion = DateTime.Now.AddMonths(-6),
                FechaActualizacion = DateTime.Now
            };

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