using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using System;

namespace AromasWeb.Controllers
{
    public class CategoriaRecetaController : Controller
    {
        private IListarCategoriasReceta _listarCategoriasReceta;
        private ICrearCategoriaReceta _crearCategoriaReceta;
        private IActualizarCategoriaReceta _actualizarCategoriaReceta;
        private IEliminarCategoriaReceta _eliminarCategoriaReceta;
        private IObtenerCategoriaReceta _obtenerCategoriaReceta;
        private readonly CrearBitacora _crearBitacora;

        public CategoriaRecetaController()
        {
            _listarCategoriasReceta = new LogicaDeNegocio.CategoriasReceta.ListarCategoriasReceta();
            _crearCategoriaReceta = new LogicaDeNegocio.CategoriasReceta.CrearCategoriaReceta();
            _actualizarCategoriaReceta = new LogicaDeNegocio.CategoriasReceta.ActualizarCategoriaReceta();
            _eliminarCategoriaReceta = new LogicaDeNegocio.CategoriasReceta.EliminarCategoriaReceta();
            _obtenerCategoriaReceta = new LogicaDeNegocio.CategoriasReceta.ObtenerCategoriaReceta();
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

        // GET: CategoriaReceta/ListadoCategoriasRecetas
        public IActionResult ListadoCategoriasRecetas(string buscar)
        {
            ViewBag.Buscar = buscar;

            List<CategoriaReceta> categorias;

            try
            {
                if (!string.IsNullOrEmpty(buscar))
                {
                    categorias = _listarCategoriasReceta.BuscarPorNombre(buscar);
                }
                else
                {
                    categorias = _listarCategoriasReceta.Obtener();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las categorías: {ex.Message}";
                categorias = new List<CategoriaReceta>();
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
        public async Task<IActionResult> CrearCategoriaReceta(CategoriaReceta categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    categoria.Estado = true;

                    int resultado = await _crearCategoriaReceta.Crear(categoria);

                    if (resultado > 0)
                    {
                        _crearBitacora.RegistrarAccion(
                            idEmpleado: ObtenerIdEmpleadoSesion(),
                            idModulo: ObtenerModulo.ObtenerIdPorNombre("Categoría de recetas"),
                            accion: Bitacora.Acciones.Crear,
                            tablaAfectada: "CategoriaReceta",
                            descripcion: $"Se creó la categoría de receta: {categoria.Nombre}",
                            datosNuevos: JsonSerializer.Serialize(new
                            {
                                categoria.Nombre,
                                categoria.Descripcion,
                                categoria.Estado
                            })
                        );

                        TempData["Mensaje"] = "Categoría de receta registrada correctamente";
                        return RedirectToAction(nameof(ListadoCategoriasRecetas));
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo registrar la categoría";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al registrar la categoría: {ex.Message}";
                }
            }

            return View(categoria);
        }

        // GET: CategoriaReceta/EditarCategoriaReceta/5
        public IActionResult EditarCategoriaReceta(int id)
        {
            try
            {
                var categoria = _obtenerCategoriaReceta.Obtener(id);

                if (categoria == null)
                {
                    TempData["Error"] = "Categoría de receta no encontrada";
                    return RedirectToAction(nameof(ListadoCategoriasRecetas));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la categoría: {ex.Message}";
                return RedirectToAction(nameof(ListadoCategoriasRecetas));
            }
        }

        // POST: CategoriaReceta/EditarCategoriaReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCategoriaReceta(CategoriaReceta categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Capturar datos anteriores ANTES de actualizar
                    var anterior = _obtenerCategoriaReceta.Obtener(categoria.IdCategoriaReceta);

                    int resultado = _actualizarCategoriaReceta.Actualizar(categoria);

                    if (resultado > 0)
                    {
                        _crearBitacora.RegistrarAccion(
                            idEmpleado: ObtenerIdEmpleadoSesion(),
                            idModulo: ObtenerModulo.ObtenerIdPorNombre("Categoría de recetas"),
                            accion: Bitacora.Acciones.Actualizar,
                            tablaAfectada: "CategoriaReceta",
                            descripcion: $"Se actualizó la categoría de receta: {categoria.Nombre} (ID: {categoria.IdCategoriaReceta})",
                            datosAnteriores: anterior != null
                                ? JsonSerializer.Serialize(new
                                {
                                    anterior.Nombre,
                                    anterior.Descripcion,
                                    anterior.Estado
                                })
                                : null,
                            datosNuevos: JsonSerializer.Serialize(new
                            {
                                categoria.Nombre,
                                categoria.Descripcion,
                                categoria.Estado
                            })
                        );

                        TempData["Mensaje"] = "Categoría de receta actualizada correctamente";
                        return RedirectToAction(nameof(ListadoCategoriasRecetas));
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo actualizar la categoría";
                    }
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar la categoría: {ex.Message}";
                }
            }

            return View(categoria);
        }

        // POST: CategoriaReceta/EliminarCategoriaReceta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoriaReceta(int id)
        {
            try
            {
                // Capturar datos ANTES de eliminar
                var categoria = _obtenerCategoriaReceta.Obtener(id);

                int resultado = _eliminarCategoriaReceta.Eliminar(id);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Categoría de recetas"),
                        accion: Bitacora.Acciones.Eliminar,
                        tablaAfectada: "CategoriaReceta",
                        descripcion: $"Se eliminó la categoría de receta: {categoria?.Nombre ?? id.ToString()} (ID: {id})",
                        datosAnteriores: categoria != null
                            ? JsonSerializer.Serialize(new
                            {
                                categoria.Nombre,
                                categoria.Descripcion,
                                categoria.Estado
                            })
                            : null
                    );

                    TempData["Mensaje"] = "Categoría de receta eliminada correctamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar la categoría";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al eliminar la categoría: {ex.Message}";
            }

            return RedirectToAction(nameof(ListadoCategoriasRecetas));
        }
    }
}