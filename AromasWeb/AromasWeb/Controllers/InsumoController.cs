using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Insumo;
using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class InsumoController : Controller
    {
        private IListarInsumos _listarInsumos;
        private IListarCategoriasInsumo _listarCategoriasInsumo;

        public InsumoController()
        {
            _listarInsumos = new LogicaDeNegocio.Insumos.ListarInsumos();
            _listarCategoriasInsumo = new LogicaDeNegocio.CategoriasInsumo.ListarCategoriasInsumo();
        }

        // GET: Insumo/ListadoInsumos
        public IActionResult ListadoInsumos(string buscar, int? categoria)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;

            // Obtener todas las categorías para el filtro
            var categorias = _listarCategoriasInsumo.Obtener();
            ViewBag.TodasCategorias = categorias;

            // Obtener insumos según los filtros
            List<Insumo> insumos;

            if (!string.IsNullOrEmpty(buscar) && categoria.HasValue)
            {
                // Buscar por nombre y filtrar por categoría
                insumos = _listarInsumos.BuscarPorNombre(buscar)
                    .FindAll(i => i.IdCategoria == categoria.Value);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre
                insumos = _listarInsumos.BuscarPorNombre(buscar);
            }
            else if (categoria.HasValue)
            {
                // Solo filtrar por categoría
                insumos = _listarInsumos.BuscarPorCategoria(categoria.Value);
            }
            else
            {
                // Obtener todos
                insumos = _listarInsumos.Obtener();
            }

            return View(insumos);
        }

        // GET: Insumo/CrearInsumo
        public IActionResult CrearInsumo()
        {
            var categorias = _listarCategoriasInsumo.Obtener();
            ViewBag.TodasCategorias = categorias;

            return View();
        }

        // POST: Insumo/CrearInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearInsumo(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Insumo registrado correctamente";
                return RedirectToAction(nameof(ListadoInsumos));
            }

            // Si hay errores, recargar las categorías
            var categorias = _listarCategoriasInsumo.Obtener();
            ViewBag.TodasCategorias = categorias;

            return View(insumo);
        }

        // GET: Insumo/EditarInsumo/5
        public IActionResult EditarInsumo(int id)
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

        // POST: Insumo/EditarInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarInsumo(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Insumo actualizado correctamente";
                return RedirectToAction(nameof(ListadoInsumos));
            }

            // Si hay errores, recargar las categorías
            var categorias = _listarCategoriasInsumo.Obtener();
            ViewBag.TodasCategorias = categorias;

            return View(insumo);
        }

        // POST: Insumo/EliminarInsumo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarInsumo(int id)
        {
            // Aquí iría la lógica para eliminar el insumo
            TempData["Mensaje"] = "Insumo eliminado correctamente";
            return RedirectToAction(nameof(ListadoInsumos));
        }
    }
}