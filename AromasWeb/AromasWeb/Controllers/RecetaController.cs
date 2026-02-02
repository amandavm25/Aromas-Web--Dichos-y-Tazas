using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.Logica.Insumo;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;

namespace AromasWeb.Controllers
{
    public class RecetaController : Controller
    {
        private IListarRecetas _listarRecetas;
        private IListarCategoriasReceta _listarCategoriasReceta;
        private IObtenerReceta _obtenerReceta;
        private IActualizarReceta _actualizarReceta;
        private IListarInsumos _listarInsumos; // Necesario para obtener los insumos disponibles

        public RecetaController()
        {
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
            _listarCategoriasReceta = new LogicaDeNegocio.CategoriasReceta.ListarCategoriasReceta();
            _obtenerReceta = new LogicaDeNegocio.Recetas.ObtenerReceta();
            _actualizarReceta = new LogicaDeNegocio.Recetas.ActualizarReceta();
            _listarInsumos = new LogicaDeNegocio.Insumos.ListarInsumos(); // Asumiendo que existe esta clase
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
            var receta = _obtenerReceta.Obtener(id);

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
            var receta = _obtenerReceta.Obtener(id);

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

            var insumos = _listarInsumos.Obtener();
            ViewBag.TodosInsumos = insumos;

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

            // Si hay errores, recargar las categorías e insumos
            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            var insumos = _listarInsumos.Obtener();
            ViewBag.TodosInsumos = insumos;

            return View(receta);
        }

        // GET: Receta/EditarReceta/5
        public IActionResult EditarReceta(int id)
        {
            try
            {
                var receta = _obtenerReceta.Obtener(id);

                if (receta == null)
                {
                    TempData["Error"] = "Receta no encontrada";
                    return RedirectToAction(nameof(ListadoRecetas));
                }

                // Cargar categorías e insumos para los dropdowns
                var categorias = _listarCategoriasReceta.Obtener();
                ViewBag.TodasCategorias = categorias;

                var insumos = _listarInsumos.Obtener();
                ViewBag.TodosInsumos = insumos;

                return View(receta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la receta: {ex.Message}";
                return RedirectToAction(nameof(ListadoRecetas));
            }
        }

        // POST: Receta/EditarReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReceta(Receta receta, List<int> IngredientesIdInsumo, List<decimal> IngredientesCantidad)
        {
            try
            {
                // Remover validaciones que se calculan automáticamente
                ModelState.Remove("CostoTotal");
                ModelState.Remove("CostoPorcion");
                ModelState.Remove("GananciaNeta");
                ModelState.Remove("PorcentajeMargen");
                ModelState.Remove("NombreCategoria");
                ModelState.Remove("Ingredientes");

                if (!ModelState.IsValid)
                {
                    var categorias = _listarCategoriasReceta.Obtener();
                    ViewBag.TodasCategorias = categorias;

                    var insumos = _listarInsumos.Obtener();
                    ViewBag.TodosInsumos = insumos;

                    return View(receta);
                }

                // Procesar los ingredientes recibidos del formulario
                receta.Ingredientes = new List<RecetaInsumo>();

                if (IngredientesIdInsumo != null && IngredientesCantidad != null)
                {
                    for (int i = 0; i < IngredientesIdInsumo.Count; i++)
                    {
                        if (i < IngredientesCantidad.Count && IngredientesCantidad[i] > 0)
                        {
                            receta.Ingredientes.Add(new RecetaInsumo
                            {
                                IdInsumo = IngredientesIdInsumo[i],
                                CantidadUtilizada = IngredientesCantidad[i]
                            });
                        }
                    }
                }

                // Validar que tenga al menos un ingrediente
                if (receta.Ingredientes == null || !receta.Ingredientes.Any())
                {
                    ModelState.AddModelError("", "Debes agregar al menos un ingrediente a la receta");

                    var categorias = _listarCategoriasReceta.Obtener();
                    ViewBag.TodasCategorias = categorias;

                    var insumos = _listarInsumos.Obtener();
                    ViewBag.TodosInsumos = insumos;

                    return View(receta);
                }

                // Actualizar la receta
                int resultado = _actualizarReceta.Actualizar(receta);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Receta actualizada correctamente";
                    return RedirectToAction(nameof(ListadoRecetas));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar la receta en la base de datos");

                    var categorias = _listarCategoriasReceta.Obtener();
                    ViewBag.TodasCategorias = categorias;

                    var insumos = _listarInsumos.Obtener();
                    ViewBag.TodosInsumos = insumos;

                    return View(receta);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar la receta: {ex.Message}");

                var categorias = _listarCategoriasReceta.Obtener();
                ViewBag.TodasCategorias = categorias;

                var insumos = _listarInsumos.Obtener();
                ViewBag.TodosInsumos = insumos;

                return View(receta);
            }
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
            try
            {
                var receta = _obtenerReceta.Obtener(id);

                if (receta == null)
                {
                    TempData["Error"] = "Receta no encontrada";
                    return RedirectToAction(nameof(ListadoRecetas));
                }

                // Cambiar la disponibilidad
                receta.Disponibilidad = !receta.Disponibilidad;

                int resultado = _actualizarReceta.Actualizar(receta);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = $"Receta marcada como {(receta.Disponibilidad ? "disponible" : "no disponible")}";
                }
                else
                {
                    TempData["Error"] = "No se pudo actualizar la disponibilidad";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(ListadoRecetas));
        }
    }
}