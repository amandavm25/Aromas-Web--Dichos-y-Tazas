using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.Logica.Insumo;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace AromasWeb.Controllers
{
    public class RecetaController : Controller
    {
        private IListarRecetas _listarRecetas;
        private IListarCategoriasReceta _listarCategoriasReceta;
        private IObtenerReceta _obtenerReceta;
        private IActualizarReceta _actualizarReceta;
        private ICrearReceta _crearReceta;
        private IEliminarReceta _eliminarReceta;
        private IListarInsumos _listarInsumos;

        public RecetaController()
        {
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
            _listarCategoriasReceta = new LogicaDeNegocio.CategoriasReceta.ListarCategoriasReceta();
            _obtenerReceta = new LogicaDeNegocio.Recetas.ObtenerReceta();
            _actualizarReceta = new LogicaDeNegocio.Recetas.ActualizarReceta();
            _crearReceta = new LogicaDeNegocio.Recetas.CrearReceta();
            _eliminarReceta = new LogicaDeNegocio.Recetas.EliminarReceta();
            _listarInsumos = new LogicaDeNegocio.Insumos.ListarInsumos();
        }

        // GET: Receta/ListadoRecetas
        public IActionResult ListadoRecetas(string buscar, int? categoria, string disponibilidad)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;
            ViewBag.Disponibilidad = disponibilidad;

            var categorias = _listarCategoriasReceta.Obtener();
            ViewBag.TodasCategorias = categorias;

            List<Receta> recetas;

            if (!string.IsNullOrEmpty(buscar))
            {
                recetas = _listarRecetas.BuscarPorNombre(buscar);
            }
            else
            {
                recetas = _listarRecetas.Obtener();
            }

            if (categoria.HasValue)
            {
                recetas = recetas.FindAll(r => r.IdCategoriaReceta == categoria.Value);
            }

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

        // GET: Receta/ObtenerIngredientes/5
        [HttpGet]
        public IActionResult ObtenerIngredientes(int id)
        {
            var receta = _obtenerReceta.Obtener(id);

            if (receta == null || receta.Ingredientes == null)
            {
                return Json(new List<object>());
            }

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
        public async Task<IActionResult> CrearReceta(Receta receta, List<int> IngredientesIdInsumo, List<decimal> IngredientesCantidad)
        {
            try
            {
                // ⭐ Remover validaciones de campos calculados
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

                // ⭐ Procesar ingredientes del formulario
                receta.Ingredientes = new List<RecetaInsumo>();

                if (IngredientesIdInsumo != null && IngredientesCantidad != null)
                {
                    // Obtener todos los insumos para calcular costos
                    var todosInsumos = _listarInsumos.Obtener();

                    for (int i = 0; i < IngredientesIdInsumo.Count; i++)
                    {
                        if (i < IngredientesCantidad.Count && IngredientesCantidad[i] > 0)
                        {
                            var insumo = todosInsumos.FirstOrDefault(ins => ins.IdInsumo == IngredientesIdInsumo[i]);

                            if (insumo != null)
                            {
                                decimal costoIngrediente = IngredientesCantidad[i] * insumo.CostoUnitario;

                                receta.Ingredientes.Add(new RecetaInsumo
                                {
                                    IdInsumo = IngredientesIdInsumo[i],
                                    CantidadUtilizada = IngredientesCantidad[i],
                                    CostoUnitario = insumo.CostoUnitario,
                                    CostoTotalIngrediente = costoIngrediente
                                });
                            }
                        }
                    }
                }

                // ⭐ Validar que tenga al menos un ingrediente
                if (receta.Ingredientes == null || !receta.Ingredientes.Any())
                {
                    ModelState.AddModelError("", "Debes agregar al menos un ingrediente a la receta");

                    var categorias = _listarCategoriasReceta.Obtener();
                    ViewBag.TodasCategorias = categorias;

                    var insumos = _listarInsumos.Obtener();
                    ViewBag.TodosInsumos = insumos;

                    return View(receta);
                }

                // ⭐ Calcular costos totales
                decimal costoTotal = receta.Ingredientes.Sum(i => i.CostoTotalIngrediente);
                receta.CostoTotal = costoTotal;
                receta.CostoPorcion = receta.CantidadPorciones > 0
                    ? costoTotal / receta.CantidadPorciones
                    : 0;

                // ⭐ Calcular ganancia y margen si hay precio de venta
                if (receta.PrecioVenta.HasValue && receta.PrecioVenta.Value > 0)
                {
                    receta.GananciaNeta = receta.PrecioVenta.Value - costoTotal;
                    receta.PorcentajeMargen = costoTotal > 0
                        ? ((receta.PrecioVenta.Value - costoTotal) / costoTotal) * 100
                        : 0;
                }
                else
                {
                    receta.GananciaNeta = 0;
                    receta.PorcentajeMargen = 0;
                }

                // ⭐ Guardar la receta
                int resultado = await _crearReceta.Crear(receta);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Receta registrada correctamente";
                    return RedirectToAction(nameof(ListadoRecetas));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo registrar la receta en la base de datos");

                    var categorias = _listarCategoriasReceta.Obtener();
                    ViewBag.TodasCategorias = categorias;

                    var insumos = _listarInsumos.Obtener();
                    ViewBag.TodosInsumos = insumos;

                    return View(receta);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al registrar la receta: {ex.Message}");

                var categorias = _listarCategoriasReceta.Obtener();
                ViewBag.TodasCategorias = categorias;

                var insumos = _listarInsumos.Obtener();
                ViewBag.TodosInsumos = insumos;

                return View(receta);
            }
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

                if (receta.Ingredientes == null || !receta.Ingredientes.Any())
                {
                    ModelState.AddModelError("", "Debes agregar al menos un ingrediente a la receta");

                    var categorias = _listarCategoriasReceta.Obtener();
                    ViewBag.TodasCategorias = categorias;

                    var insumos = _listarInsumos.Obtener();
                    ViewBag.TodosInsumos = insumos;

                    return View(receta);
                }

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
            try
            {
                int resultado = _eliminarReceta.Eliminar(id);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Receta eliminada correctamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar la receta";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

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