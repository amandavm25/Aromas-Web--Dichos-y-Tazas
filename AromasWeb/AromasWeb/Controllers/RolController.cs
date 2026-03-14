using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AromasWeb.Controllers
{
    public class RolController : Controller
    {
        private readonly CrearBitacora _crearBitacora;

        public RolController()
        {
            _crearBitacora = new CrearBitacora();
        }

        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;
            return 1;
        }
        // GET: Rol/ListadoRoles
        public IActionResult ListadoRoles(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Roles de ejemplo
            var roles = new List<Rol>
            {
                new Rol
                {
                    IdRol = 1,
                    Nombre = "Administrador",
                    Descripcion = "Acceso completo al sistema",
                    Estado = true
                },
                new Rol
                {
                    IdRol = 2,
                    Nombre = "Empleado",
                    Descripcion = "Usuario estándar",
                    Estado = true
                }
            };

            return View(roles);
        }

        // GET: Rol/CrearRol
        public IActionResult CrearRol()
        {
            return View();
        }

        // POST: Rol/CrearRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearRol(Rol rol)
        {
            if (ModelState.IsValid)
            {
                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de roles"),
                    accion: Bitacora.Acciones.Crear,
                    tablaAfectada: "Rol",
                    descripcion: $"Se registró el rol: {rol.Nombre}",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        rol.Nombre,
                        rol.Descripcion,
                        rol.Estado
                    })
                );

                TempData["Mensaje"] = "Rol registrado correctamente";
                return RedirectToAction(nameof(ListadoRoles));
            }

            return View(rol);
        }

        // GET: Rol/EditarRol/5
        public IActionResult EditarRol(int id)
        {
            // Rol de ejemplo
            var rol = new Rol
            {
                IdRol = id,
                Nombre = "Administrador",
                Descripcion = "Acceso completo al sistema",
                Estado = true
            };

            return View(rol);
        }

        // POST: Rol/EditarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarRol(Rol rol)
        {
            if (ModelState.IsValid)
            {
                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de roles"),
                    accion: Bitacora.Acciones.Actualizar,
                    tablaAfectada: "Rol",
                    descripcion: $"Se actualizó el rol: {rol.Nombre} (ID: {rol.IdRol})",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        rol.IdRol,
                        rol.Nombre,
                        rol.Descripcion,
                        rol.Estado
                    })
                );

                TempData["Mensaje"] = "Rol actualizado correctamente";
                return RedirectToAction(nameof(ListadoRoles));
            }

            return View(rol);
        }

        // POST: Rol/EliminarRol/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarRol(int id)
        {
            _crearBitacora.RegistrarAccion(
                idEmpleado: ObtenerIdEmpleadoSesion(),
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de roles"),
                accion: Bitacora.Acciones.Eliminar,
                tablaAfectada: "Rol",
                descripcion: $"Se eliminó el rol ID: {id}",
                datosAnteriores: JsonSerializer.Serialize(new { IdRol = id })
            );

            TempData["Mensaje"] = "Rol eliminado correctamente";
            return RedirectToAction(nameof(ListadoRoles));
        }

        // POST: Rol/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id)
        {
            _crearBitacora.RegistrarAccion(
                idEmpleado: ObtenerIdEmpleadoSesion(),
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de roles"),
                accion: Bitacora.Acciones.CambiarEstado,
                tablaAfectada: "Rol",
                descripcion: $"Se cambió el estado del rol ID: {id}",
                datosNuevos: JsonSerializer.Serialize(new { IdRol = id })
            );

            TempData["Mensaje"] = "Estado del rol actualizado correctamente";
            return RedirectToAction(nameof(ListadoRoles));
        }
    }
}