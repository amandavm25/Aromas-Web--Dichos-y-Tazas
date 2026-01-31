using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Empleado;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class EmpleadoController : Controller
    {
        private IListarEmpleados _listarEmpleados;

        public EmpleadoController()
        {
            _listarEmpleados = new LogicaDeNegocio.Empleados.ListarEmpleados();
        }

        // GET: Empleado/ListadoEmpleados
        public IActionResult ListadoEmpleados(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Obtener empleados según los filtros
            List<Empleado> empleados;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                // Buscar por nombre y filtrar por estado
                bool estado = filtroEstado == "activo";
                empleados = _listarEmpleados.BuscarPorNombre(buscar)
                    .FindAll(e => e.Estado == estado);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre
                empleados = _listarEmpleados.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                // Solo filtrar por estado
                bool estado = filtroEstado == "activo";
                empleados = _listarEmpleados.BuscarPorEstado(estado);
            }
            else
            {
                // Obtener todos
                empleados = _listarEmpleados.Obtener();
            }

            return View(empleados);
        }

        // GET: Empleado/CrearEmpleado
        public IActionResult CrearEmpleado()
        {
            CargarRoles();
            return View();
        }

        // POST: Empleado/CrearEmpleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearEmpleado(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Empleado registrado correctamente";
                return RedirectToAction(nameof(ListadoEmpleados));
            }

            CargarRoles();
            return View(empleado);
        }

        // GET: Empleado/EditarEmpleado/5
        public IActionResult EditarEmpleado(int id)
        {
            var empleado = _listarEmpleados.ObtenerPorId(id);

            if (empleado == null)
            {
                TempData["Error"] = "Empleado no encontrado";
                return RedirectToAction(nameof(ListadoEmpleados));
            }

            CargarRoles();
            return View(empleado);
        }

        // POST: Empleado/EditarEmpleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarEmpleado(Empleado empleado)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Empleado actualizado correctamente";
                return RedirectToAction(nameof(ListadoEmpleados));
            }

            CargarRoles();
            return View(empleado);
        }

        // POST: Empleado/EliminarEmpleado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarEmpleado(int id)
        {
            // Aquí iría la lógica para eliminar el empleado
            TempData["Mensaje"] = "Empleado eliminado correctamente";
            return RedirectToAction(nameof(ListadoEmpleados));
        }

        // POST: Empleado/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id)
        {
            // Aquí iría la lógica para cambiar el estado del empleado
            TempData["Mensaje"] = "Estado del empleado actualizado correctamente";
            return RedirectToAction(nameof(ListadoEmpleados));
        }

        // POST: Empleado/CambiarContrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarContrasena(int idEmpleado, string contrasenaActual, string contrasenaNueva)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para cambiar la contraseña
                TempData["Mensaje"] = "Contraseña actualizada correctamente";
                return RedirectToAction(nameof(ListadoEmpleados));
            }

            return RedirectToAction(nameof(EditarEmpleado), new { id = idEmpleado });
        }

        // Método auxiliar para cargar roles
        private void CargarRoles()
        {
            var roles = new List<dynamic>
            {
                new { IdRol = 1, Nombre = "Administrador" },
                new { IdRol = 2, Nombre = "Empleado" }
            };

            ViewBag.Roles = roles;
        }
    }
}