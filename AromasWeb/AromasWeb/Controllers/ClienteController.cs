using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente/ListadoClientes
        public IActionResult ListadoClientes(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Clientes de ejemplo
            var clientes = new List<Cliente>
            {
                new Cliente
                {
                    IdCliente = 1,
                    Identificacion = "1-1234-5678",
                    NombreCompleto = "María González Rodríguez",
                    Correo = "maria.gonzalez@email.com",
                    Telefono = "8888-1234",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-6),
                    UltimaReserva = DateTime.Now.AddDays(-5)
                },
                new Cliente
                {
                    IdCliente = 2,
                    Identificacion = "2-2345-6789",
                    NombreCompleto = "Carlos Jiménez Mora",
                    Correo = "carlos.jimenez@email.com",
                    Telefono = "8888-2345",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-4),
                    UltimaReserva = DateTime.Now.AddDays(-2)
                },
                new Cliente
                {
                    IdCliente = 3,
                    Identificacion = "1-3456-7890",
                    NombreCompleto = "Ana Patricia Vargas Solís",
                    Correo = "ana.vargas@email.com",
                    Telefono = "8888-3456",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-3),
                    UltimaReserva = DateTime.Now.AddDays(-10)
                },
                new Cliente
                {
                    IdCliente = 4,
                    Identificacion = "1-4567-8901",
                    NombreCompleto = "Roberto Fernández Castro",
                    Correo = "roberto.fernandez@email.com",
                    Telefono = "8888-4567",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-8),
                    UltimaReserva = DateTime.Now.AddDays(-1)
                },
                new Cliente
                {
                    IdCliente = 5,
                    Identificacion = "2-5678-9012",
                    NombreCompleto = "Laura Martínez Pérez",
                    Correo = "laura.martinez@email.com",
                    Telefono = "8888-5678",
                    Estado = false,
                    FechaRegistro = DateTime.Now.AddMonths(-12),
                    UltimaReserva = DateTime.Now.AddMonths(-2)
                },
                new Cliente
                {
                    IdCliente = 6,
                    Identificacion = "1-6789-0123",
                    NombreCompleto = "José Luis Ramírez Quesada",
                    Correo = "jose.ramirez@email.com",
                    Telefono = "8888-6789",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-2),
                    UltimaReserva = DateTime.Now.AddDays(-7)
                },
                new Cliente
                {
                    IdCliente = 7,
                    Identificacion = "1-7890-1234",
                    NombreCompleto = "Sofía Hernández Blanco",
                    Correo = "sofia.hernandez@email.com",
                    Telefono = "8888-7890",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-5),
                    UltimaReserva = DateTime.Now.AddDays(-3)
                },
                new Cliente
                {
                    IdCliente = 8,
                    Identificacion = "2-8901-2345",
                    NombreCompleto = "Diego Alvarado Sánchez",
                    Correo = "diego.alvarado@email.com",
                    Telefono = "8888-8901",
                    Estado = true,
                    FechaRegistro = DateTime.Now.AddMonths(-1),
                    UltimaReserva = DateTime.Now.AddDays(-15)
                }
            };

            return View(clientes);
        }

        // GET: Cliente/CrearCliente
        public IActionResult CrearCliente()
        {
            return View();
        }

        // POST: Cliente/CrearCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Cliente registrado correctamente";
                return RedirectToAction(nameof(ListadoClientes));
            }

            return View(cliente);
        }

        // GET: Cliente/EditarCliente/5
        public IActionResult EditarCliente(int id)
        {
            // Cliente de ejemplo
            var cliente = new Cliente
            {
                IdCliente = id,
                Identificacion = "1-1234-5678",
                NombreCompleto = "María González Rodríguez",
                Correo = "maria.gonzalez@email.com",
                Telefono = "8888-1234",
                Estado = true,
                FechaRegistro = DateTime.Now.AddMonths(-6),
                UltimaReserva = DateTime.Now.AddDays(-5)
            };

            return View(cliente);
        }

        // POST: Cliente/EditarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Cliente actualizado correctamente";
                return RedirectToAction(nameof(ListadoClientes));
            }

            return View(cliente);
        }

        // GET: Cliente/HistorialReservas/5 (Para empleados)
        public IActionResult HistorialReservas(int id)
        {
            ViewBag.Cliente = new
            {
                IdCliente = id,
                NombreCompleto = "María González Rodríguez",
                Identificacion = "1-1234-5678",
                Correo = "maria.gonzalez@email.com",
                Telefono = "8888-1234"
            };

            var reservas = new List<Reserva>
            {
                new Reserva { IdReserva = 15, Fecha = DateTime.Parse("28/11/2025"), Hora = new TimeSpan(14, 0, 0), CantidadPersonas = 4, Estado = "Confirmada" },
                new Reserva { IdReserva = 12, Fecha = DateTime.Parse("20/11/2025"), Hora = new TimeSpan(19, 30, 0), CantidadPersonas = 2, Estado = "Completada" },
                new Reserva { IdReserva = 8, Fecha = DateTime.Parse("10/11/2025"), Hora = new TimeSpan(12, 0, 0), CantidadPersonas = 6, Estado = "Completada" },
                new Reserva { IdReserva = 5, Fecha = DateTime.Parse("25/10/2025"), Hora = new TimeSpan(18, 0, 0), CantidadPersonas = 3, Estado = "Completada" },
                new Reserva { IdReserva = 2, Fecha = DateTime.Parse("15/09/2025"), Hora = new TimeSpan(20, 0, 0), CantidadPersonas = 2, Estado = "Cancelada" }
            };

            ViewBag.Reservas = reservas;

            return View();
        }

        // GET: Cliente/MisReservas (Para clientes autenticados)
        public IActionResult MisReservas(string filtro)
        {
            ViewBag.Filtro = filtro;

            // Simulación de reservas del cliente autenticado
            var reservas = new List<Reserva>
            {
                new Reserva
                {
                    IdReserva = 1,
                    IdCliente = 1,
                    CantidadPersonas = 4,
                    Fecha = DateTime.Now.AddDays(2),
                    Hora = new TimeSpan(19, 30, 0),
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Now.AddDays(-3)
                },
                new Reserva
                {
                    IdReserva = 2,
                    IdCliente = 1,
                    CantidadPersonas = 2,
                    Fecha = DateTime.Now.AddDays(7),
                    Hora = new TimeSpan(14, 0, 0),
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now.AddDays(-1)
                },
                new Reserva
                {
                    IdReserva = 3,
                    IdCliente = 1,
                    CantidadPersonas = 6,
                    Fecha = DateTime.Now.AddDays(-5),
                    Hora = new TimeSpan(20, 0, 0),
                    Estado = "Completada",
                    FechaCreacion = DateTime.Now.AddDays(-10)
                }
            };

            // Aplicar filtros
            if (!string.IsNullOrEmpty(filtro))
            {
                if (filtro == "activas")
                {
                    reservas = reservas.Where(r => r.Fecha >= DateTime.Now &&
                                                   (r.Estado == "Pendiente" || r.Estado == "Confirmada")).ToList();
                }
                else if (filtro == "pasadas")
                {
                    reservas = reservas.Where(r => r.Fecha < DateTime.Now ||
                                                   r.Estado == "Completada" ||
                                                   r.Estado == "Cancelada").ToList();
                }
            }

            return View(reservas);
        }

        // GET: Cliente/EditarMiReserva/5 (Para clientes editando sus propias reservas)
        public IActionResult EditarMiReserva(int id)
        {
            // Verificar que la reserva pertenezca al cliente autenticado
            // En producción, verificar con el IdCliente de la sesión

            var reserva = new Reserva
            {
                IdReserva = id,
                IdCliente = 1,
                NombreCliente = "María González Rodríguez",
                TelefonoCliente = "8888-1234",
                CantidadPersonas = 4,
                Fecha = DateTime.Now.AddDays(2),
                Hora = new TimeSpan(19, 30, 0),
                Estado = "Pendiente",
                FechaCreacion = DateTime.Now.AddDays(-3)
            };

            // Verificar que la reserva se pueda editar (no completada o cancelada)
            if (reserva.Estado == "Completada" || reserva.Estado == "Cancelada")
            {
                TempData["Error"] = "No puedes editar una reserva completada o cancelada";
                return RedirectToAction(nameof(MisReservas));
            }

            // Verificar que falten al menos 24 horas
            if ((reserva.Fecha - DateTime.Now).TotalHours < 24)
            {
                TempData["Error"] = "Debes modificar tu reserva con al menos 24 horas de anticipación";
                return RedirectToAction(nameof(MisReservas));
            }

            return View(reserva);
        }

        // POST: Cliente/EditarMiReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarMiReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Verificar nuevamente las 24 horas
                if ((reserva.Fecha - DateTime.Now).TotalHours < 24)
                {
                    TempData["Error"] = "Debes modificar tu reserva con al menos 24 horas de anticipación";
                    return View(reserva);
                }

                TempData["Mensaje"] = "Tu reserva ha sido actualizada correctamente";
                return RedirectToAction(nameof(MisReservas));
            }

            return View(reserva);
        }

        // POST: Cliente/CancelarMiReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarMiReserva(int idReserva)
        {
            // Verificar que la reserva pertenezca al cliente autenticado
            // Verificar que falten al menos 24 horas

            TempData["Mensaje"] = "Tu reserva ha sido cancelada correctamente";
            return RedirectToAction(nameof(MisReservas));
        }

        // POST: Cliente/EliminarCliente/5 (Para empleados)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCliente(int id)
        {
            TempData["Mensaje"] = "Cliente eliminado correctamente";
            return RedirectToAction(nameof(ListadoClientes));
        }

        // POST: Cliente/CambiarContrasena
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarContrasena(int idCliente, string contrasenaActual, string contrasenaNueva)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Contraseña actualizada correctamente";
                return RedirectToAction(nameof(ListadoClientes));
            }

            return RedirectToAction(nameof(EditarCliente), new { id = idCliente });
        }

        // GET: Cliente/MiPerfil
        public IActionResult MiPerfil()
        {
            // En producción, obtener el IdCliente de la sesión
            int idCliente = 1; // Simulación

            var cliente = new Cliente
            {
                IdCliente = idCliente,
                Identificacion = "1-1234-5678",
                NombreCompleto = "María González Rodríguez",
                Correo = "maria.gonzalez@email.com",
                Telefono = "8888-1234",
                Estado = true,
                FechaRegistro = DateTime.Now.AddMonths(-6),
                UltimaReserva = DateTime.Now.AddDays(-5)
            };

            return View(cliente);
        }

        // GET: Cliente/EditarPerfil
        public IActionResult EditarPerfil()
        {
            // En producción, obtener el IdCliente de la sesión
            int idCliente = 1; // Simulación

            var cliente = new Cliente
            {
                IdCliente = idCliente,
                Identificacion = "1-1234-5678",
                NombreCompleto = "María González Rodríguez",
                Correo = "maria.gonzalez@email.com",
                Telefono = "8888-1234",
                Estado = true,
                FechaRegistro = DateTime.Now.AddMonths(-6),
                UltimaReserva = DateTime.Now.AddDays(-5)
            };

            return View(cliente);
        }

        // POST: Cliente/EditarPerfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPerfil(Cliente cliente, string contrasenaActual, string contrasenaNueva, string confirmarContrasenaNueva)
        {
            if (ModelState.IsValid)
            {
                // Actualizar información básica del cliente en base de datos
                // ...

                // Si se proporcionaron campos de contraseña, cambiarla también
                if (!string.IsNullOrWhiteSpace(contrasenaActual) &&
                    !string.IsNullOrWhiteSpace(contrasenaNueva) &&
                    !string.IsNullOrWhiteSpace(confirmarContrasenaNueva))
                {
                    // Verificar contraseña actual
                    // En producción: verificar contra base de datos

                    // Validar que la nueva contraseña cumpla requisitos
                    if (contrasenaNueva != confirmarContrasenaNueva)
                    {
                        TempData["Error"] = "Las contraseñas nuevas no coinciden";
                        return View(cliente);
                    }

                    if (contrasenaNueva.Length < 8)
                    {
                        TempData["Error"] = "La contraseña debe tener al menos 8 caracteres";
                        return View(cliente);
                    }

                    // Actualizar contraseña en base de datos
                    // ...

                    TempData["Mensaje"] = "Tu perfil y contraseña han sido actualizados correctamente";
                }
                else
                {
                    TempData["Mensaje"] = "Tu perfil ha sido actualizado correctamente";
                }

                return RedirectToAction(nameof(MiPerfil));
            }

            return View(cliente);
        }
    }
}