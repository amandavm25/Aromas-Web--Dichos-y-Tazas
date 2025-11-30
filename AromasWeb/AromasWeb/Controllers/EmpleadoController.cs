using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class EmpleadoController : Controller
    {
        // GET: Empleado/ListadoEmpleados
        public IActionResult ListadoEmpleados(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Empleados de ejemplo
            var empleados = new List<Empleado>
            {
                new Empleado
                {
                    IdEmpleado = 1,
                    IdRol = 1,
                    NombreRol = "Administrador",
                    Identificacion = "1-1234-5678",
                    NombreCompleto = "María González Rodríguez",
                    Correo = "maria.gonzalez@entredichosytazas.com",
                    Telefono = "8888-1234",
                    Cargo = "Gerente General",
                    FechaContratacion = DateTime.Now.AddYears(-2),
                    Estado = true
                },
                new Empleado
                {
                    IdEmpleado = 2,
                    IdRol = 2,
                    NombreRol = "Empleado",
                    Identificacion = "2-2345-6789",
                    NombreCompleto = "Carlos Jiménez Mora",
                    Correo = "carlos.jimenez@entredichosytazas.com",
                    Telefono = "8888-2345",
                    Cargo = "Barista",
                    FechaContratacion = DateTime.Now.AddMonths(-8),
                    Estado = true
                },
                new Empleado
                {
                    IdEmpleado = 3,
                    IdRol = 2,
                    NombreRol = "Empleado",
                    Identificacion = "1-3456-7890",
                    NombreCompleto = "Ana Patricia Vargas Solís",
                    Correo = "ana.vargas@entredichosytazas.com",
                    Telefono = "8888-3456",
                    Cargo = "Mesera",
                    FechaContratacion = DateTime.Now.AddMonths(-14),
                    Estado = true
                },
                new Empleado
                {
                    IdEmpleado = 4,
                    IdRol = 2,
                    NombreRol = "Empleado",
                    Identificacion = "1-4567-8901",
                    NombreCompleto = "Roberto Fernández Castro",
                    Correo = "roberto.fernandez@entredichosytazas.com",
                    Telefono = "8888-4567",
                    Cargo = "Chef",
                    FechaContratacion = DateTime.Now.AddYears(-3),
                    Estado = true
                },
                new Empleado
                {
                    IdEmpleado = 5,
                    IdRol = 2,
                    NombreRol = "Empleado",
                    Identificacion = "2-5678-9012",
                    NombreCompleto = "Laura Martínez Pérez",
                    Correo = "laura.martinez@entredichosytazas.com",
                    Telefono = "8888-5678",
                    Cargo = "Cajera",
                    FechaContratacion = DateTime.Now.AddMonths(-6),
                    Estado = false
                },
                new Empleado
                {
                    IdEmpleado = 6,
                    IdRol = 2,
                    NombreRol = "Empleado",
                    Identificacion = "1-6789-0123",
                    NombreCompleto = "José Luis Ramírez Quesada",
                    Correo = "jose.ramirez@entredichosytazas.com",
                    Telefono = "8888-6789",
                    Cargo = "Barista",
                    FechaContratacion = DateTime.Now.AddMonths(-18),
                    Estado = true
                }
            };

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
                TempData["Mensaje"] = "Empleado registrado correctamente";
                return RedirectToAction(nameof(ListadoEmpleados));
            }

            CargarRoles();
            return View(empleado);
        }

        // GET: Empleado/EditarEmpleado/5
        public IActionResult EditarEmpleado(int id)
        {
            // Empleado de ejemplo
            var empleado = new Empleado
            {
                IdEmpleado = id,
                IdRol = 2,
                NombreRol = "Empleado",
                Identificacion = "1-1234-5678",
                NombreCompleto = "María González Rodríguez",
                Correo = "maria.gonzalez@entredichosytazas.com",
                Telefono = "8888-1234",
                Cargo = "Gerente General",
                FechaContratacion = DateTime.Now.AddYears(-2),
                Estado = true
            };

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
            TempData["Mensaje"] = "Empleado eliminado correctamente";
            return RedirectToAction(nameof(ListadoEmpleados));
        }

        // POST: Empleado/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id)
        {
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