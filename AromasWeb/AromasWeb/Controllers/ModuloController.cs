using AromasWeb.Abstracciones.ModeloUI;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AromasWeb.Controllers
{
    public class ModuloController : Controller
    {
        // GET: Modulo/ListadoModulos
        public IActionResult ListadoModulos(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Módulos de ejemplo
            var modulos = new List<Modulo>
            {
                new Modulo
                {
                    IdModulo = 1,
                    Nombre = "Gestión de Empleados",
                    Descripcion = "Módulo para administrar empleados del sistema",
                    Estado = true
                },
                new Modulo
                {
                    IdModulo = 2,
                    Nombre = "Control de Asistencia",
                    Descripcion = "Registro y seguimiento de asistencias",
                    Estado = true
                },
                new Modulo
                {
                    IdModulo = 3,
                    Nombre = "Gestión de Planilla",
                    Descripcion = "Cálculo y procesamiento de planillas",
                    Estado = true
                },
                new Modulo
                {
                    IdModulo = 4,
                    Nombre = "Solicitud de Vacaciones",
                    Descripcion = "Sistema de solicitud y aprobación de vacaciones",
                    Estado = true
                },
                new Modulo
                {
                    IdModulo = 5,
                    Nombre = "Bitácora del Sistema",
                    Descripcion = "Registro de acciones y eventos del sistema",
                    Estado = true
                },
                new Modulo
                {
                    IdModulo = 6,
                    Nombre = "Reportes",
                    Descripcion = "Generación de reportes y estadísticas",
                    Estado = false
                },
                new Modulo
                {
                    IdModulo = 7,
                    Nombre = "Gestión de Roles",
                    Descripcion = "Administración de roles y permisos",
                    Estado = true
                },
                new Modulo
                {
                    IdModulo = 8,
                    Nombre = "Historial de Tarifas",
                    Descripcion = "Registro histórico de tarifas por hora",
                    Estado = true
                }
            };

            // Filtrar por búsqueda
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                modulos = modulos.Where(m =>
                    m.Nombre.ToLower().Contains(buscar.ToLower()) ||
                    (m.Descripcion != null && m.Descripcion.ToLower().Contains(buscar.ToLower()))
                ).ToList();
            }

            // Filtrar por estado
            if (!string.IsNullOrWhiteSpace(filtroEstado))
            {
                if (filtroEstado.ToLower() == "activo")
                {
                    modulos = modulos.Where(m => m.Estado).ToList();
                }
                else if (filtroEstado.ToLower() == "inactivo")
                {
                    modulos = modulos.Where(m => !m.Estado).ToList();
                }
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
            // Módulo de ejemplo
            var modulo = new Modulo
            {
                IdModulo = id,
                Nombre = "Gestión de Empleados",
                Descripcion = "Módulo para administrar empleados del sistema",
                Estado = true
            };

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