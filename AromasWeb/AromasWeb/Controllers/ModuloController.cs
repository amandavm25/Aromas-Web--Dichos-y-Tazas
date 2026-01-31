using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Modulo;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class ModuloController : Controller
    {
        private IListarModulos _listarModulos;

        public ModuloController()
        {
            _listarModulos = new LogicaDeNegocio.Modulos.ListarModulos();
        }

        // GET: Modulo/ListadoModulos
        public IActionResult ListadoModulos(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Obtener módulos según los filtros
            List<Modulo> modulos;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                // Buscar por nombre y filtrar por estado
                bool estado = filtroEstado == "activo";
                modulos = _listarModulos.BuscarPorNombre(buscar)
                    .Where(m => m.Estado == estado)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre
                modulos = _listarModulos.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                // Solo filtrar por estado
                bool estado = filtroEstado == "activo";
                modulos = _listarModulos.BuscarPorEstado(estado);
            }
            else
            {
                // Obtener todos
                modulos = _listarModulos.Obtener();
            }

            return View(modulos);
        }

        // GET: Modulo/CrearModulo
        public IActionResult CrearModulo()
        {
            return View();
        }

        // POST: Modulo/CrearModulo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearModulo(Modulo modulo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Módulo registrado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoModulos));
            }

            return View(modulo);
        }

        // GET: Modulo/EditarModulo/5
        public IActionResult EditarModulo(int id)
        {
            var modulo = _listarModulos.ObtenerPorId(id);

            if (modulo == null)
            {
                TempData["Mensaje"] = "Módulo no encontrado";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(ListadoModulos));
            }

            return View(modulo);
        }

        // POST: Modulo/EditarModulo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarModulo(Modulo modulo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Módulo actualizado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoModulos));
            }

            return View(modulo);
        }

        // POST: Modulo/EliminarModulo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarModulo(int id)
        {
            // Aquí iría la lógica para eliminar de la base de datos
            // Validar que no tenga permisos asociados antes de eliminar

            TempData["Mensaje"] = "Módulo eliminado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(ListadoModulos));
        }
    }
}