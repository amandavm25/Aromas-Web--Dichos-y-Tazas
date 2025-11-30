using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class RolController : Controller
    {
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
            TempData["Mensaje"] = "Rol eliminado correctamente";
            return RedirectToAction(nameof(ListadoRoles));
        }

        // POST: Rol/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id)
        {
            TempData["Mensaje"] = "Estado del rol actualizado correctamente";
            return RedirectToAction(nameof(ListadoRoles));
        }
    }
}