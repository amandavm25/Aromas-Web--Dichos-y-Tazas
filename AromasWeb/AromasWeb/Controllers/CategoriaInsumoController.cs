using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace AromasWeb.Controllers
{
    public class CategoriaInsumoController : Controller
    {
        private IListarCategoriasInsumo _listarCategoriasInsumo;
        private ICrearCategoriaInsumo _crearCategoriaInsumo;
        private IActualizarCategoriaInsumo _actualizarCategoriaInsumo;
        private IEliminarCategoriaInsumo _eliminarCategoriaInsumo;
        private IObtenerCategoriaInsumo _obtenerCategoriaInsumo;
        private readonly CrearBitacora _crearBitacora;

        public CategoriaInsumoController()
        {
            _listarCategoriasInsumo = new LogicaDeNegocio.CategoriasInsumo.ListarCategoriasInsumo();
            _crearCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.CrearCategoriaInsumo();
            _actualizarCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.ActualizarCategoriaInsumo();
            _eliminarCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.EliminarCategoriaInsumo();
            _obtenerCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.ObtenerCategoriaInsumo();
            _crearBitacora = new CrearBitacora();
        }

        // Helper de sesión
        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;

            return 1; // ← quitar el fallback del contexto temporalmente
        }

        // GET: CategoriaInsumo/ListadoCategoriasInsumos
        public IActionResult ListadoCategoriasInsumos(string buscar)
        {
            ViewBag.Buscar = buscar;

            List<CategoriaInsumo> categorias;

            try
            {
                if (!string.IsNullOrEmpty(buscar))
                {
                    categorias = _listarCategoriasInsumo.BuscarPorNombre(buscar);
                }
                else
                {
                    categorias = _listarCategoriasInsumo.Obtener();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las categorías: {ex.Message}";
                categorias = new List<CategoriaInsumo>();
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
        public async Task<IActionResult> CrearCategoriaInsumo(CategoriaInsumo categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Establecer valores por defecto
                    categoria.Estado = true;

                    int resultado = await _crearCategoriaInsumo.Crear(categoria);

                    if (resultado > 0)
                    {
                        // Registrar en bitácora
                        _crearBitacora.RegistrarAccion(
                            idEmpleado: ObtenerIdEmpleadoSesion(),
                            idModulo: ObtenerModulo.ObtenerIdPorNombre("Categoría de insumos"),
                            accion: Bitacora.Acciones.Crear,
                            tablaAfectada: "CategoriaInsumo",
                            descripcion: $"Se creó la categoría de insumo: {categoria.NombreCategoria}",
                            datosNuevos: JsonSerializer.Serialize(new
                            {
                                categoria.NombreCategoria,
                                categoria.Descripcion,
                                categoria.Estado
                            })
                        );

                        TempData["Mensaje"] = "Categoría registrada correctamente";
                        return RedirectToAction(nameof(ListadoCategoriasInsumos));
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

        // GET: CategoriaInsumo/EditarCategoriaInsumo/5
        public IActionResult EditarCategoriaInsumo(int id)
        {
            try
            {
                var categoria = _obtenerCategoriaInsumo.Obtener(id);

                if (categoria == null)
                {
                    TempData["Error"] = "Categoría no encontrada";
                    return RedirectToAction(nameof(ListadoCategoriasInsumos));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la categoría: {ex.Message}";
                return RedirectToAction(nameof(ListadoCategoriasInsumos));
            }
        }

        // POST: CategoriaInsumo/EditarCategoriaInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCategoriaInsumo(CategoriaInsumo categoria)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Capturar datos anteriores ANTES de actualizar
                    var anterior = _obtenerCategoriaInsumo.Obtener(categoria.IdCategoria);

                    int resultado = _actualizarCategoriaInsumo.Actualizar(categoria);

                    if (resultado > 0)
                    {
                        // Registrar en bitácora
                        _crearBitacora.RegistrarAccion(
                            idEmpleado: ObtenerIdEmpleadoSesion(),
                            idModulo: ObtenerModulo.ObtenerIdPorNombre("Categoría de insumos"),
                            accion: Bitacora.Acciones.Actualizar,
                            tablaAfectada: "CategoriaInsumo",
                            descripcion: $"Se actualizó la categoría de insumo: {categoria.NombreCategoria} (ID: {categoria.IdCategoria})",
                            datosAnteriores: anterior != null
                                ? JsonSerializer.Serialize(new
                                {
                                    anterior.NombreCategoria,
                                    anterior.Descripcion,
                                    anterior.Estado
                                })
                                : null,
                            datosNuevos: JsonSerializer.Serialize(new
                            {
                                categoria.NombreCategoria,
                                categoria.Descripcion,
                                categoria.Estado
                            })
                        );

                        TempData["Mensaje"] = "Categoría actualizada correctamente";
                        return RedirectToAction(nameof(ListadoCategoriasInsumos));
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

        // POST: CategoriaInsumo/EliminarCategoriaInsumo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoriaInsumo(int id)
        {
            try
            {
                // Capturar datos ANTES de eliminar
                var categoria = _obtenerCategoriaInsumo.Obtener(id);

                int resultado = _eliminarCategoriaInsumo.Eliminar(id);

                if (resultado > 0)
                {
                    // Registrar en bitácora
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Categoría de insumos"),
                        accion: Bitacora.Acciones.Eliminar,
                        tablaAfectada: "CategoriaInsumo",
                        descripcion: $"Se eliminó la categoría de insumo: {categoria?.NombreCategoria ?? id.ToString()} (ID: {id})",
                        datosAnteriores: categoria != null
                            ? JsonSerializer.Serialize(new
                            {
                                categoria.NombreCategoria,
                                categoria.Descripcion,
                                categoria.Estado
                            })
                            : null
                    );

                    TempData["Mensaje"] = "Categoría eliminada correctamente";
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

            return RedirectToAction(nameof(ListadoCategoriasInsumos));
        }
    }
}