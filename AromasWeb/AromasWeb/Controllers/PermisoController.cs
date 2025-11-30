using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class PermisoController : Controller
    {
        // GET: Permiso/ListadoPermisos
        public IActionResult ListadoPermisos(string buscar, string filtroModulo)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;

            // Permisos de ejemplo
            var permisos = new List<Permiso>
            {
                new Permiso
                {
                    IdPermiso = 1,
                    IdModulo = 1,
                    Nombre = "Ver Empleados",
                    Modulo = new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" }
                },
                new Permiso
                {
                    IdPermiso = 2,
                    IdModulo = 1,
                    Nombre = "Crear Empleados",
                    Modulo = new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" }
                },
                new Permiso
                {
                    IdPermiso = 3,
                    IdModulo = 1,
                    Nombre = "Editar Empleados",
                    Modulo = new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" }
                },
                new Permiso
                {
                    IdPermiso = 4,
                    IdModulo = 1,
                    Nombre = "Eliminar Empleados",
                    Modulo = new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" }
                },
                new Permiso
                {
                    IdPermiso = 5,
                    IdModulo = 2,
                    Nombre = "Registrar Asistencia",
                    Modulo = new Modulo { IdModulo = 2, Nombre = "Control de Asistencia" }
                },
                new Permiso
                {
                    IdPermiso = 6,
                    IdModulo = 3,
                    Nombre = "Calcular Planilla",
                    Modulo = new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla" }
                }
            };

            // Lista de módulos para el filtro
            var modulos = new List<Modulo>
            {
                new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" },
                new Modulo { IdModulo = 2, Nombre = "Control de Asistencia" },
                new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla" }
            };

            ViewBag.Modulos = modulos;

            return View(permisos);
        }

        // GET: Permiso/CrearPermiso
        public IActionResult CrearPermiso()
        {
            // Lista de módulos para el dropdown
            var modulos = new List<Modulo>
            {
                new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" },
                new Modulo { IdModulo = 2, Nombre = "Control de Asistencia" },
                new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla" },
                new Modulo { IdModulo = 4, Nombre = "Solicitud de Vacaciones" },
                new Modulo { IdModulo = 5, Nombre = "Bitácora del Sistema" },
                new Modulo { IdModulo = 6, Nombre = "Reportes" }
            };

            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre");

            return View();
        }

        // POST: Permiso/CrearPermiso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPermiso(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Permiso registrado correctamente";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            // Recargar módulos si hay error
            var modulos = new List<Modulo>
            {
                new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" },
                new Modulo { IdModulo = 2, Nombre = "Control de Asistencia" },
                new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla" },
                new Modulo { IdModulo = 4, Nombre = "Solicitud de Vacaciones" },
                new Modulo { IdModulo = 5, Nombre = "Bitácora del Sistema" },
                new Modulo { IdModulo = 6, Nombre = "Reportes" }
            };

            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre");

            return View(permiso);
        }

        // GET: Permiso/EditarPermiso/5
        public IActionResult EditarPermiso(int id)
        {
            // Permiso de ejemplo
            var permiso = new Permiso
            {
                IdPermiso = id,
                IdModulo = 1,
                Nombre = "Ver Empleados"
            };

            // Lista de módulos para el dropdown
            var modulos = new List<Modulo>
            {
                new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" },
                new Modulo { IdModulo = 2, Nombre = "Control de Asistencia" },
                new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla" },
                new Modulo { IdModulo = 4, Nombre = "Solicitud de Vacaciones" },
                new Modulo { IdModulo = 5, Nombre = "Bitácora del Sistema" },
                new Modulo { IdModulo = 6, Nombre = "Reportes" }
            };

            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre", permiso.IdModulo);

            return View(permiso);
        }

        // POST: Permiso/EditarPermiso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPermiso(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Permiso actualizado correctamente";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            // Recargar módulos si hay error
            var modulos = new List<Modulo>
            {
                new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados" },
                new Modulo { IdModulo = 2, Nombre = "Control de Asistencia" },
                new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla" },
                new Modulo { IdModulo = 4, Nombre = "Solicitud de Vacaciones" },
                new Modulo { IdModulo = 5, Nombre = "Bitácora del Sistema" },
                new Modulo { IdModulo = 6, Nombre = "Reportes" }
            };

            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre", permiso.IdModulo);

            return View(permiso);
        }

        // POST: Permiso/EliminarPermiso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPermiso(int id)
        {
            TempData["Mensaje"] = "Permiso eliminado correctamente";
            return RedirectToAction(nameof(ListadoPermisos));
        }

        // GET: Permiso/AsignarPermisos/5
        public IActionResult AsignarPermisos(int id)
        {
            // Rol de ejemplo
            var rol = new Rol
            {
                IdRol = id,
                Nombre = "Administrador",
                Descripcion = "Acceso completo al sistema"
            };

            // Módulos con sus permisos
            var modulos = new List<Modulo>
            {
                new Modulo { IdModulo = 1, Nombre = "Gestión de Empleados", Estado = true },
                new Modulo { IdModulo = 2, Nombre = "Control de Asistencia", Estado = true },
                new Modulo { IdModulo = 3, Nombre = "Gestión de Planilla", Estado = true },
                new Modulo { IdModulo = 4, Nombre = "Solicitud de Vacaciones", Estado = true },
                new Modulo { IdModulo = 5, Nombre = "Bitácora del Sistema", Estado = true },
                new Modulo { IdModulo = 6, Nombre = "Reportes", Estado = false }
            };

            // Permisos agrupados por módulo
            var permisos = new List<Permiso>
            {
                new Permiso { IdPermiso = 1, IdModulo = 1, Nombre = "Ver Empleados" },
                new Permiso { IdPermiso = 2, IdModulo = 1, Nombre = "Crear Empleados" },
                new Permiso { IdPermiso = 3, IdModulo = 1, Nombre = "Editar Empleados" },
                new Permiso { IdPermiso = 4, IdModulo = 1, Nombre = "Eliminar Empleados" },
                new Permiso { IdPermiso = 5, IdModulo = 2, Nombre = "Registrar Asistencia" },
                new Permiso { IdPermiso = 6, IdModulo = 2, Nombre = "Ver Asistencias" },
                new Permiso { IdPermiso = 7, IdModulo = 3, Nombre = "Calcular Planilla" },
                new Permiso { IdPermiso = 8, IdModulo = 3, Nombre = "Ver Planilla" },
                new Permiso { IdPermiso = 9, IdModulo = 4, Nombre = "Solicitar Vacaciones" },
                new Permiso { IdPermiso = 10, IdModulo = 4, Nombre = "Aprobar Vacaciones" },
                new Permiso { IdPermiso = 11, IdModulo = 5, Nombre = "Ver Bitácora" },
                new Permiso { IdPermiso = 12, IdModulo = 6, Nombre = "Generar Reportes" }
            };

            // Permisos asignados (ejemplo - IDs 1, 2, 3, 5, 7)
            var permisosAsignados = new List<int> { 1, 2, 3, 5, 7 };

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
            TempData["Mensaje"] = "Permisos asignados correctamente";
            return RedirectToAction("ListadoRoles", "Rol");
        }
    }
}