using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.Abstracciones.Logica.Modulo;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class PermisoController : Controller
    {
        private IListarPermisos _listarPermisos;
        private IListarModulos _listarModulos;

        public PermisoController()
        {
            _listarPermisos = new LogicaDeNegocio.Permisos.ListarPermisos();
            _listarModulos = new LogicaDeNegocio.Modulos.ListarModulos();
        }

        // GET: Permiso/ListadoPermisos
        public IActionResult ListadoPermisos(string buscar, int? filtroModulo)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;

            // Cargar módulos para filtro
            CargarModulos();

            // Obtener permisos
            List<Permiso> permisos;

            if (!string.IsNullOrEmpty(buscar) && filtroModulo.HasValue)
            {
                permisos = _listarPermisos.ObtenerPorModulo(filtroModulo.Value)
                    .Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                permisos = _listarPermisos.Obtener()
                    .Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()))
                    .ToList();
            }
            else if (filtroModulo.HasValue)
            {
                permisos = _listarPermisos.ObtenerPorModulo(filtroModulo.Value);
            }
            else
            {
                permisos = _listarPermisos.Obtener();
            }

            return View(permisos);
        }

        // GET: Permiso/CrearPermiso
        public IActionResult CrearPermiso()
        {
            CargarModulosParaSelect();
            return View();
        }

        // POST: Permiso/CrearPermiso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPermiso(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Permiso registrado correctamente";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect();
            return View(permiso);
        }

        // GET: Permiso/EditarPermiso/5
        public IActionResult EditarPermiso(int id)
        {
            var permiso = _listarPermisos.ObtenerPorId(id);

            if (permiso == null)
            {
                TempData["Mensaje"] = "Permiso no encontrado";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect(permiso.IdModulo);
            return View(permiso);
        }

        // POST: Permiso/EditarPermiso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPermiso(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Permiso actualizado correctamente";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect(permiso.IdModulo);
            return View(permiso);
        }

        // POST: Permiso/EliminarPermiso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPermiso(int id)
        {
            // Aquí iría la lógica para eliminar de la base de datos
            TempData["Mensaje"] = "Permiso eliminado correctamente";
            return RedirectToAction(nameof(ListadoPermisos));
        }

        // GET: Permiso/AsignarPermisos/5
        public IActionResult AsignarPermisos(int id)
        {
            // Aquí obtendrías el rol desde la BD (por ahora ejemplo)
            var rol = new Rol
            {
                IdRol = id,
                Nombre = "Administrador",
                Descripcion = "Acceso completo al sistema"
            };

            // Obtener módulos y permisos desde la BD
            var modulos = _listarModulos.Obtener();
            var permisos = _listarPermisos.Obtener();
            var permisosAsignados = _listarPermisos.ObtenerPermisosDeRol(id);

            ViewBag.Rol = rol;
            ViewBag.Modulos = modulos;
            ViewBag.Permisos = permisos;
            ViewBag.PermisosAsignados = permisosAsignados;

            return View();
        }

        // POST: Permiso/GuardarPermisos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPermisos(int idRol, List<int> permisosSeleccionados)
        {
            if (permisosSeleccionados == null)
            {
                permisosSeleccionados = new List<int>();
            }

            bool exito = _listarPermisos.AsignarPermisosARol(idRol, permisosSeleccionados);

            if (exito)
            {
                TempData["Mensaje"] = "Permisos asignados correctamente";
            }
            else
            {
                TempData["Mensaje"] = "Error al asignar permisos";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction("ListadoRoles", "Rol");
        }

        // Métodos auxiliares
        private void CargarModulos()
        {
            var modulos = _listarModulos.Obtener();
            ViewBag.Modulos = modulos;
        }

        private void CargarModulosParaSelect(int? idModuloSeleccionado = null)
        {
            var modulos = _listarModulos.BuscarPorEstado(true); // Solo activos
            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre", idModuloSeleccionado);
        }
    }
}