using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.Logica.Insumo;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System.Collections.Generic;
using System.Text.Json;
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
        private readonly CrearBitacora _crearBitacora;

        public RecetaController()
        {
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
            _listarCategoriasReceta = new LogicaDeNegocio.CategoriasReceta.ListarCategoriasReceta();
            _obtenerReceta = new LogicaDeNegocio.Recetas.ObtenerReceta();
            _actualizarReceta = new LogicaDeNegocio.Recetas.ActualizarReceta();
            _crearReceta = new LogicaDeNegocio.Recetas.CrearReceta();
            _eliminarReceta = new LogicaDeNegocio.Recetas.EliminarReceta();
            _listarInsumos = new LogicaDeNegocio.Insumos.ListarInsumos();
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

        // Helper para recargar ViewBags en caso de error
        private void CargarViewBags()
        {
            try { ViewBag.TodasCategorias = _listarCategoriasReceta.Obtener(); }
            catch { ViewBag.TodasCategorias = new List<CategoriaReceta>(); }

            try { ViewBag.TodosInsumos = _listarInsumos.Obtener(); }
            catch { ViewBag.TodosInsumos = new List<Insumo>(); }
        }

        // GET: Receta/ListadoRecetas
        public IActionResult ListadoRecetas(string buscar, int? categoria, string disponibilidad)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;
            ViewBag.Disponibilidad = disponibilidad;

            List<Receta> recetas;

            try
            {
                ViewBag.TodasCategorias = _listarCategoriasReceta.Obtener();

                recetas = !string.IsNullOrEmpty(buscar)
                    ? _listarRecetas.BuscarPorNombre(buscar)
                    : _listarRecetas.Obtener();

                if (categoria.HasValue)
                    recetas = recetas.FindAll(r => r.IdCategoriaReceta == categoria.Value);

                if (!string.IsNullOrEmpty(disponibilidad))
                {
                    if (disponibilidad == "disponible")
                        recetas = recetas.FindAll(r => r.Disponibilidad == true);
                    else if (disponibilidad == "no-disponible")
                        recetas = recetas.FindAll(r => r.Disponibilidad == false);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las recetas: {ex.Message}";
                recetas = new List<Receta>();
                ViewBag.TodasCategorias = new List<CategoriaReceta>();
            }

            return View(recetas);
        }

        // GET: Receta/DetalleReceta/5
        public IActionResult DetalleReceta(int id)
        {
            try
            {
                var receta = _obtenerReceta.Obtener(id);

                if (receta == null)
                {
                    TempData["Error"] = "Receta no encontrada";
                    return RedirectToAction(nameof(ListadoRecetas));
                }

                return View(receta);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la receta: {ex.Message}";
                return RedirectToAction(nameof(ListadoRecetas));
            }
        }

        // GET: Receta/ObtenerIngredientes/5
        [HttpGet]
        public IActionResult ObtenerIngredientes(int id)
        {
            var receta = _obtenerReceta.Obtener(id);

            if (receta == null || receta.Ingredientes == null)
                return Json(new List<object>());

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
            CargarViewBags();
            return View();
        }

        // POST: Receta/CrearReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearReceta(Receta receta, List<int> IngredientesIdInsumo, List<decimal> IngredientesCantidad)
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
                    CargarViewBags();
                    return View(receta);
                }

                receta.Ingredientes = new List<RecetaInsumo>();

                if (IngredientesIdInsumo != null && IngredientesCantidad != null)
                {
                    var todosInsumos = _listarInsumos.Obtener();

                    for (int i = 0; i < IngredientesIdInsumo.Count; i++)
                    {
                        if (i < IngredientesCantidad.Count && IngredientesCantidad[i] > 0)
                        {
                            var insumo = todosInsumos.FirstOrDefault(ins => ins.IdInsumo == IngredientesIdInsumo[i]);

                            if (insumo != null)
                            {
                                receta.Ingredientes.Add(new RecetaInsumo
                                {
                                    IdInsumo = IngredientesIdInsumo[i],
                                    CantidadUtilizada = IngredientesCantidad[i],
                                    CostoUnitario = insumo.CostoUnitario,
                                    CostoTotalIngrediente = IngredientesCantidad[i] * insumo.CostoUnitario
                                });
                            }
                        }
                    }
                }

                if (!receta.Ingredientes.Any())
                {
                    ModelState.AddModelError("", "Debes agregar al menos un ingrediente a la receta");
                    CargarViewBags();
                    return View(receta);
                }

                decimal costoTotal = receta.Ingredientes.Sum(i => i.CostoTotalIngrediente);
                receta.CostoTotal = costoTotal;
                receta.CostoPorcion = receta.CantidadPorciones > 0
                    ? costoTotal / receta.CantidadPorciones
                    : 0;

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

                int resultado = await _crearReceta.Crear(receta);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de recetas"),
                        accion: Bitacora.Acciones.Crear,
                        tablaAfectada: "Receta",
                        descripcion: $"Se creó la receta: {receta.Nombre}",
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            receta.Nombre,
                            receta.IdCategoriaReceta,
                            receta.CantidadPorciones,
                            receta.PrecioVenta,
                            receta.CostoTotal,
                            receta.Disponibilidad,
                            CantidadIngredientes = receta.Ingredientes.Count
                        })
                    );

                    TempData["Mensaje"] = "Receta registrada correctamente";
                    return RedirectToAction(nameof(ListadoRecetas));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo registrar la receta en la base de datos");
                    CargarViewBags();
                    return View(receta);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al registrar la receta: {ex.Message}");
                CargarViewBags();
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

                CargarViewBags();
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
                    CargarViewBags();
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

                if (!receta.Ingredientes.Any())
                {
                    ModelState.AddModelError("", "Debes agregar al menos un ingrediente a la receta");
                    CargarViewBags();
                    return View(receta);
                }

                // Capturar datos anteriores ANTES de actualizar
                var anterior = _obtenerReceta.Obtener(receta.IdReceta);

                int resultado = _actualizarReceta.Actualizar(receta);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de recetas"),
                        accion: Bitacora.Acciones.Actualizar,
                        tablaAfectada: "Receta",
                        descripcion: $"Se actualizó la receta: {receta.Nombre} (ID: {receta.IdReceta})",
                        datosAnteriores: anterior != null
                            ? JsonSerializer.Serialize(new
                            {
                                anterior.Nombre,
                                anterior.IdCategoriaReceta,
                                anterior.CantidadPorciones,
                                anterior.PrecioVenta,
                                anterior.CostoTotal,
                                anterior.Disponibilidad
                            })
                            : null,
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            receta.Nombre,
                            receta.IdCategoriaReceta,
                            receta.CantidadPorciones,
                            receta.PrecioVenta,
                            receta.CostoTotal,
                            receta.Disponibilidad,
                            CantidadIngredientes = receta.Ingredientes.Count
                        })
                    );

                    TempData["Mensaje"] = "Receta actualizada correctamente";
                    return RedirectToAction(nameof(ListadoRecetas));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar la receta en la base de datos");
                    CargarViewBags();
                    return View(receta);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar la receta: {ex.Message}");
                CargarViewBags();
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
                // Capturar datos ANTES de eliminar
                var receta = _obtenerReceta.Obtener(id);

                int resultado = _eliminarReceta.Eliminar(id);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de recetas"),
                        accion: Bitacora.Acciones.Eliminar,
                        tablaAfectada: "Receta",
                        descripcion: $"Se eliminó la receta: {receta?.Nombre ?? id.ToString()} (ID: {id})",
                        datosAnteriores: receta != null
                            ? JsonSerializer.Serialize(new
                            {
                                receta.Nombre,
                                receta.IdCategoriaReceta,
                                receta.CantidadPorciones,
                                receta.PrecioVenta,
                                receta.CostoTotal,
                                receta.Disponibilidad
                            })
                            : null
                    );

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

                bool disponibilidadAnterior = receta.Disponibilidad;
                receta.Disponibilidad = !receta.Disponibilidad;

                int resultado = _actualizarReceta.Actualizar(receta);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de recetas"),
                        accion: Bitacora.Acciones.Actualizar,
                        tablaAfectada: "Receta",
                        descripcion: $"Se cambió la disponibilidad de la receta: {receta.Nombre} (ID: {id})",
                        datosAnteriores: JsonSerializer.Serialize(new { Disponibilidad = disponibilidadAnterior }),
                        datosNuevos: JsonSerializer.Serialize(new { Disponibilidad = receta.Disponibilidad })
                    );

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