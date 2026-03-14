using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Insumo;
using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System.Collections.Generic;
using System.Text.Json;
using System;

namespace AromasWeb.Controllers
{
    public class InsumoController : Controller
    {
        private IListarInsumos _listarInsumos;
        private IListarCategoriasInsumo _listarCategoriasInsumo;
        private readonly CrearBitacora _crearBitacora;

        public InsumoController()
        {
            _listarInsumos = new LogicaDeNegocio.Insumos.ListarInsumos();
            _listarCategoriasInsumo = new LogicaDeNegocio.CategoriasInsumo.ListarCategoriasInsumo();
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

        // GET: Insumo/ListadoInsumos
        public IActionResult ListadoInsumos(string buscar, int? categoria)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;

            List<Insumo> insumos;

            try
            {
                var categorias = _listarCategoriasInsumo.Obtener();
                ViewBag.TodasCategorias = categorias;

                if (!string.IsNullOrEmpty(buscar) && categoria.HasValue)
                {
                    insumos = _listarInsumos.BuscarPorNombre(buscar)
                        .FindAll(i => i.IdCategoria == categoria.Value);
                }
                else if (!string.IsNullOrEmpty(buscar))
                {
                    insumos = _listarInsumos.BuscarPorNombre(buscar);
                }
                else if (categoria.HasValue)
                {
                    insumos = _listarInsumos.BuscarPorCategoria(categoria.Value);
                }
                else
                {
                    insumos = _listarInsumos.Obtener();
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar los insumos: {ex.Message}";
                insumos = new List<Insumo>();
                ViewBag.TodasCategorias = new List<CategoriaInsumo>();
            }

            return View(insumos);
        }

        // GET: Insumo/CrearInsumo
        public IActionResult CrearInsumo()
        {
            try
            {
                var categorias = _listarCategoriasInsumo.Obtener();
                ViewBag.TodasCategorias = categorias;
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar las categorías: {ex.Message}";
                ViewBag.TodasCategorias = new List<CategoriaInsumo>();
            }

            return View();
        }

        // POST: Insumo/CrearInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearInsumo(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _listarInsumos.Crear(insumo);

                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de insumos"),
                        accion: Bitacora.Acciones.Crear,
                        tablaAfectada: "Insumo",
                        descripcion: $"Se creó el insumo: {insumo.NombreInsumo}",
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            insumo.NombreInsumo,
                            insumo.UnidadMedida,
                            insumo.IdCategoria,
                            insumo.CostoUnitario,
                            insumo.CantidadDisponible,
                            insumo.StockMinimo,
                            insumo.Estado
                        })
                    );

                    TempData["Mensaje"] = "Insumo registrado correctamente";
                    return RedirectToAction(nameof(ListadoInsumos));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al registrar el insumo: {ex.Message}";
                }
            }

            try
            {
                ViewBag.TodasCategorias = _listarCategoriasInsumo.Obtener();
            }
            catch
            {
                ViewBag.TodasCategorias = new List<CategoriaInsumo>();
            }

            return View(insumo);
        }

        // GET: Insumo/EditarInsumo/5
        public IActionResult EditarInsumo(int id)
        {
            try
            {
                var categorias = _listarCategoriasInsumo.Obtener();
                ViewBag.TodasCategorias = categorias;

                var insumo = _listarInsumos.ObtenerPorId(id);

                if (insumo == null)
                {
                    TempData["Error"] = "Insumo no encontrado";
                    return RedirectToAction(nameof(ListadoInsumos));
                }

                return View(insumo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el insumo: {ex.Message}";
                return RedirectToAction(nameof(ListadoInsumos));
            }
        }

        // POST: Insumo/EditarInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarInsumo(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Capturar datos anteriores ANTES de actualizar
                    var anterior = _listarInsumos.ObtenerPorId(insumo.IdInsumo);

                    _listarInsumos.Actualizar(insumo);

                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de insumos"),
                        accion: Bitacora.Acciones.Actualizar,
                        tablaAfectada: "Insumo",
                        descripcion: $"Se actualizó el insumo: {insumo.NombreInsumo} (ID: {insumo.IdInsumo})",
                        datosAnteriores: anterior != null
                            ? JsonSerializer.Serialize(new
                            {
                                anterior.NombreInsumo,
                                anterior.UnidadMedida,
                                anterior.IdCategoria,
                                anterior.CostoUnitario,
                                anterior.CantidadDisponible,
                                anterior.StockMinimo,
                                anterior.Estado
                            })
                            : null,
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            insumo.NombreInsumo,
                            insumo.UnidadMedida,
                            insumo.IdCategoria,
                            insumo.CostoUnitario,
                            insumo.CantidadDisponible,
                            insumo.StockMinimo,
                            insumo.Estado
                        })
                    );

                    TempData["Mensaje"] = "Insumo actualizado correctamente";
                    return RedirectToAction(nameof(ListadoInsumos));
                }
                catch (Exception ex)
                {
                    TempData["Error"] = $"Error al actualizar el insumo: {ex.Message}";
                }
            }

            try
            {
                ViewBag.TodasCategorias = _listarCategoriasInsumo.Obtener();
            }
            catch
            {
                ViewBag.TodasCategorias = new List<CategoriaInsumo>();
            }

            return View(insumo);
        }
    }
}