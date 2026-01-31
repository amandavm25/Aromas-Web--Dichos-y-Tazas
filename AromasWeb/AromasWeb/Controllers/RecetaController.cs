using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class RecetaController : Controller
    {
        private IListarRecetas _listarRecetas;
        private IListarCategoriasReceta _listarCategoriasReceta;

        public RecetaController()
        {
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
            _listarCategoriasReceta = new LogicaDeNegocio.CategoriasReceta.ListarCategoriasReceta();
        }

        // GET: Receta/ListadoRecetas
        public IActionResult ListadoRecetas(string buscar, int? categoria, string disponibilidad)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;
            ViewBag.Disponibilidad = disponibilidad;

            // Obtener todas las categorías para el filtro
            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            // Obtener recetas según los filtros
            List<Receta> recetas;

            // Aplicar filtro de búsqueda por nombre
            if (!string.IsNullOrEmpty(buscar))
            {
                recetas = _listarRecetas.BuscarPorNombre(buscar);
            }
            else
            {
                recetas = _listarRecetas.Obtener();
            }

            // Aplicar filtro de categoría
            if (categoria.HasValue)
            {
                recetas = recetas.FindAll(r => r.IdCategoriaReceta == categoria.Value);
            }

            // Aplicar filtro de disponibilidad
            if (!string.IsNullOrEmpty(disponibilidad))
            {
                if (disponibilidad == "disponible")
                {
                    recetas = recetas.FindAll(r => r.Disponibilidad == true);
                }
                else if (disponibilidad == "no-disponible")
                {
                    recetas = recetas.FindAll(r => r.Disponibilidad == false);
                }
            }

            return View(recetas);
        }

        // GET: Receta/DetalleReceta/5
        public IActionResult DetalleReceta(int id)
        {
            var receta = _listarRecetas.ObtenerPorId(id);

            if (receta == null)
            {
                TempData["Error"] = "Receta no encontrada";
                return RedirectToAction(nameof(ListadoRecetas));
            }

            return View(receta);
        }

        // GET: Receta/ObtenerIngredientes/5 - Para carga dinámica en modal
        [HttpGet]
        public IActionResult ObtenerIngredientes(int id)
        {
            var receta = _listarRecetas.ObtenerPorId(id);

            if (receta == null || receta.Ingredientes == null)
            {
                return Json(new List<object>());
            }

            // Transformar los ingredientes al formato necesario para el modal
            var ingredientes = receta.Ingredientes.Select(i => new
            {
                nombreInsumo = i.NombreInsumo,
                cantidadUtilizada = i.CantidadUtilizada,
                unidadMedida = i.UnidadMedida,
                costoTotalIngrediente = i.CostoTotalIngrediente
            }).ToList();

            return Json(ingredientes);
        }

        // GET: Receta/CrearReceta
        public IActionResult CrearReceta()
        {
            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            return View();
        }

        // POST: Receta/CrearReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearReceta(Receta receta)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Receta registrada correctamente";
                return RedirectToAction(nameof(ListadoRecetas));
            }

            // Si hay errores, recargar las categorías
            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            return View(receta);
        }

        // GET: Receta/EditarReceta/5
        public IActionResult EditarReceta(int id)
        {
            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            var receta = _listarRecetas.ObtenerPorId(id);

            if (receta == null)
            {
                TempData["Error"] = "Receta no encontrada";
                return RedirectToAction(nameof(ListadoRecetas));
            }

            return View(receta);
        }

        // POST: Receta/EditarReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReceta(Receta receta)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Receta actualizada correctamente";
                return RedirectToAction(nameof(ListadoRecetas));
            }

            // Si hay errores, recargar las categorías
            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            return View(receta);
        }

        // POST: Receta/EliminarReceta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarReceta(int id)
        {
            // Aquí iría la lógica para eliminar la receta
            TempData["Mensaje"] = "Receta eliminada correctamente";
            return RedirectToAction(nameof(ListadoRecetas));
        }

        // POST: Receta/CambiarDisponibilidad/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarDisponibilidad(int id)
        {
            // Aquí iría la lógica para cambiar la disponibilidad
            TempData["Mensaje"] = "Disponibilidad actualizada correctamente";
            return RedirectToAction(nameof(ListadoRecetas));
        }
    }
}