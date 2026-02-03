using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AromasWeb.Controllers
{
    public class CategoriaInsumoController : Controller
    {
        private IListarCategoriasInsumo _listarCategoriasInsumo;
        private ICrearCategoriaInsumo _crearCategoriaInsumo;
        private IActualizarCategoriaInsumo _actualizarCategoriaInsumo;
        private IEliminarCategoriaInsumo _eliminarCategoriaInsumo;
        private IObtenerCategoriaInsumo _obtenerCategoriaInsumo;

        public CategoriaInsumoController()
        {
            _listarCategoriasInsumo = new LogicaDeNegocio.CategoriasInsumo.ListarCategoriasInsumo();
            _crearCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.CrearCategoriaInsumo();
            _actualizarCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.ActualizarCategoriaInsumo();
            _eliminarCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.EliminarCategoriaInsumo();
            _obtenerCategoriaInsumo = new LogicaDeNegocio.CategoriasInsumo.ObtenerCategoriaInsumo();
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
                    int resultado = _actualizarCategoriaInsumo.Actualizar(categoria);

                    if (resultado > 0)
                    {
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
                int resultado = _eliminarCategoriaInsumo.Eliminar(id);

                if (resultado > 0)
                {
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