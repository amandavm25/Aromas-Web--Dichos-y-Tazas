using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class ReservaController : Controller
    {
        // GET: Reserva/ListadoReservas
        public IActionResult ListadoReservas(string buscar, string filtroEstado, string filtroFecha)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.FiltroFecha = filtroFecha;

            // Reservas de ejemplo
            var reservas = new List<Reserva>
            {
                new Reserva
                {
                    IdReserva = 1,
                    IdCliente = 1,
                    NombreCliente = "María González Rodríguez",
                    TelefonoCliente = "8888-1234",
                    CantidadPersonas = 4,
                    Fecha = DateTime.Now.AddDays(2),
                    Hora = new TimeSpan(19, 30, 0),
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Now.AddDays(-3)
                },
                new Reserva
                {
                    IdReserva = 2,
                    IdCliente = 2,
                    NombreCliente = "Carlos Jiménez Mora",
                    TelefonoCliente = "8888-2345",
                    CantidadPersonas = 2,
                    Fecha = DateTime.Now,
                    Hora = new TimeSpan(14, 0, 0),
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Now.AddDays(-1)
                },
                new Reserva
                {
                    IdReserva = 3,
                    IdCliente = 3,
                    NombreCliente = "Ana Patricia Vargas Solís",
                    TelefonoCliente = "8888-3456",
                    CantidadPersonas = 6,
                    Fecha = DateTime.Now.AddDays(5),
                    Hora = new TimeSpan(20, 0, 0),
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now.AddHours(-2)
                },
                new Reserva
                {
                    IdReserva = 4,
                    IdCliente = 4,
                    NombreCliente = "Roberto Fernández Castro",
                    TelefonoCliente = "8888-4567",
                    CantidadPersonas = 3,
                    Fecha = DateTime.Now.AddDays(1),
                    Hora = new TimeSpan(12, 30, 0),
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Now.AddDays(-5)
                },
                new Reserva
                {
                    IdReserva = 5,
                    IdCliente = 5,
                    NombreCliente = "Laura Martínez Pérez",
                    TelefonoCliente = "8888-5678",
                    CantidadPersonas = 2,
                    Fecha = DateTime.Now.AddDays(-2),
                    Hora = new TimeSpan(18, 0, 0),
                    Estado = "Completada",
                    FechaCreacion = DateTime.Now.AddDays(-8)
                },
                new Reserva
                {
                    IdReserva = 6,
                    IdCliente = 1,
                    NombreCliente = "María González Rodríguez",
                    TelefonoCliente = "8888-1234",
                    CantidadPersonas = 5,
                    Fecha = DateTime.Now.AddDays(-5),
                    Hora = new TimeSpan(19, 0, 0),
                    Estado = "Cancelada",
                    FechaCreacion = DateTime.Now.AddDays(-10)
                },
                new Reserva
                {
                    IdReserva = 7,
                    IdCliente = 6,
                    NombreCliente = "José Luis Ramírez Quesada",
                    TelefonoCliente = "8888-6789",
                    CantidadPersonas = 4,
                    Fecha = DateTime.Now.AddDays(3),
                    Hora = new TimeSpan(13, 0, 0),
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Now.AddDays(-2)
                },
                new Reserva
                {
                    IdReserva = 8,
                    IdCliente = 7,
                    NombreCliente = "Sofía Hernández Blanco",
                    TelefonoCliente = "8888-7890",
                    CantidadPersonas = 8,
                    Fecha = DateTime.Now.AddDays(7),
                    Hora = new TimeSpan(20, 30, 0),
                    Estado = "Pendiente",
                    FechaCreacion = DateTime.Now.AddHours(-5)
                }
            };

            // Aplicar filtros
            if (!string.IsNullOrEmpty(buscar))
            {
                reservas = reservas.Where(r =>
                    r.NombreCliente.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    r.TelefonoCliente.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                    r.FechaFormateada.Contains(buscar, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(filtroEstado))
            {
                reservas = reservas.Where(r =>
                    r.Estado.Equals(filtroEstado, StringComparison.OrdinalIgnoreCase)
                ).ToList();
            }

            if (!string.IsNullOrEmpty(filtroFecha))
            {
                switch (filtroFecha.ToLower())
                {
                    case "hoy":
                        reservas = reservas.Where(r => r.Fecha.Date == DateTime.Now.Date).ToList();
                        break;
                    case "proximas":
                        reservas = reservas.Where(r => r.Fecha.Date >= DateTime.Now.Date).ToList();
                        break;
                    case "pasadas":
                        reservas = reservas.Where(r => r.Fecha.Date < DateTime.Now.Date).ToList();
                        break;
                }
            }

            return View(reservas);
        }

        // GET: Reserva/CrearReserva (Para clientes)
        public IActionResult CrearReserva()
        {
            return View();
        }

        // POST: Reserva/CrearReserva (Para clientes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Reserva registrada correctamente. Recibirá una confirmación pronto.";
                return RedirectToAction("Index", "Home");
            }

            return View(reserva);
        }

        // GET: Reserva/RegistrarReservaCliente (Para empleados)
        public IActionResult RegistrarReservaCliente()
        {
            return View();
        }

        // POST: Reserva/RegistrarReservaCliente (Para empleados)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarReservaCliente(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Reserva registrada correctamente";
                return RedirectToAction(nameof(ListadoReservas));
            }

            return View(reserva);
        }

        // GET: Reserva/BuscarClientePorIdentificacion (AJAX)
        [HttpGet]
        public JsonResult BuscarClientePorIdentificacion(string identificacion)
        {
            // Simulación de búsqueda de cliente
            var clientes = new List<dynamic>
            {
                new { Id = 1, Identificacion = "1-1234-5678", Nombre = "María González Rodríguez", Telefono = "8888-1234" },
                new { Id = 2, Identificacion = "2-2345-6789", Nombre = "Carlos Jiménez Mora", Telefono = "8888-2345" },
                new { Id = 3, Identificacion = "1-3456-7890", Nombre = "Ana Patricia Vargas Solís", Telefono = "8888-3456" },
                new { Id = 4, Identificacion = "1-4567-8901", Nombre = "Roberto Fernández Castro", Telefono = "8888-4567" },
                new { Id = 5, Identificacion = "2-5678-9012", Nombre = "Laura Martínez Pérez", Telefono = "8888-5678" }
            };

            var cliente = clientes.FirstOrDefault(c => c.Identificacion == identificacion);

            if (cliente != null)
            {
                return Json(new
                {
                    success = true,
                    data = cliente
                });
            }

            return Json(new
            {
                success = false,
                message = "Cliente no encontrado"
            });
        }

        // GET: Reserva/EditarReserva/5
        public IActionResult EditarReserva(int id)
        {
            // Clientes de ejemplo para el dropdown
            var clientes = new List<dynamic>
            {
                new { Id = 1, Nombre = "María González Rodríguez", Telefono = "8888-1234" },
                new { Id = 2, Nombre = "Carlos Jiménez Mora", Telefono = "8888-2345" },
                new { Id = 3, Nombre = "Ana Patricia Vargas Solís", Telefono = "8888-3456" },
                new { Id = 4, Nombre = "Roberto Fernández Castro", Telefono = "8888-4567" },
                new { Id = 5, Nombre = "Laura Martínez Pérez", Telefono = "8888-5678" }
            };

            ViewBag.Clientes = clientes;

            // Reserva de ejemplo
            var reserva = new Reserva
            {
                IdReserva = id,
                IdCliente = 1,
                NombreCliente = "María González Rodríguez",
                TelefonoCliente = "8888-1234",
                CantidadPersonas = 4,
                Fecha = DateTime.Now.AddDays(2),
                Hora = new TimeSpan(19, 30, 0),
                Estado = "Confirmada",
                FechaCreacion = DateTime.Now.AddDays(-3)
            };

            return View(reserva);
        }

        // POST: Reserva/EditarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Reserva actualizada correctamente";
                return RedirectToAction(nameof(ListadoReservas));
            }

            // Recargar clientes si hay error
            var clientes = new List<dynamic>
            {
                new { Id = 1, Nombre = "María González Rodríguez", Telefono = "8888-1234" },
                new { Id = 2, Nombre = "Carlos Jiménez Mora", Telefono = "8888-2345" },
                new { Id = 3, Nombre = "Ana Patricia Vargas Solís", Telefono = "8888-3456" },
                new { Id = 4, Nombre = "Roberto Fernández Castro", Telefono = "8888-4567" },
                new { Id = 5, Nombre = "Laura Martínez Pérez", Telefono = "8888-5678" }
            };

            ViewBag.Clientes = clientes;

            return View(reserva);
        }

        // GET: Reserva/HistorialReservas/5 (Para empleados - ver historial de un cliente)
        public IActionResult HistorialReservas(int id)
        {
            // Información del cliente
            ViewBag.Cliente = new
            {
                IdCliente = id,
                NombreCompleto = "María González Rodríguez",
                Identificacion = "1-1234-5678",
                Correo = "maria.gonzalez@email.com",
                Telefono = "8888-1234"
            };

            // Reservas del cliente
            var reservas = new List<Reserva>
            {
                new Reserva
                {
                    IdReserva = 15,
                    IdCliente = id,
                    Fecha = DateTime.Now.AddDays(1),
                    Hora = new TimeSpan(14, 0, 0),
                    CantidadPersonas = 4,
                    Estado = "Confirmada",
                    FechaCreacion = DateTime.Now.AddDays(-5)
                },
                new Reserva
                {
                    IdReserva = 12,
                    IdCliente = id,
                    Fecha = DateTime.Now.AddDays(-7),
                    Hora = new TimeSpan(19, 30, 0),
                    CantidadPersonas = 2,
                    Estado = "Completada",
                    FechaCreacion = DateTime.Now.AddDays(-12)
                },
                new Reserva
                {
                    IdReserva = 8,
                    IdCliente = id,
                    Fecha = DateTime.Now.AddDays(-17),
                    Hora = new TimeSpan(12, 0, 0),
                    CantidadPersonas = 6,
                    Estado = "Completada",
                    FechaCreacion = DateTime.Now.AddDays(-20)
                },
                new Reserva
                {
                    IdReserva = 5,
                    IdCliente = id,
                    Fecha = DateTime.Now.AddDays(-33),
                    Hora = new TimeSpan(18, 0, 0),
                    CantidadPersonas = 3,
                    Estado = "Completada",
                    FechaCreacion = DateTime.Now.AddDays(-36)
                },
                new Reserva
                {
                    IdReserva = 2,
                    IdCliente = id,
                    Fecha = DateTime.Now.AddDays(-73),
                    Hora = new TimeSpan(20, 0, 0),
                    CantidadPersonas = 2,
                    Estado = "Cancelada",
                    FechaCreacion = DateTime.Now.AddDays(-76)
                }
            };

            ViewBag.Reservas = reservas;

            return View();
        }

        // POST: Reserva/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            TempData["Mensaje"] = $"Estado de la reserva cambiado a {nuevoEstado}";
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/CancelarReserva/5 (Cambia el estado a "Cancelada" en lugar de eliminar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarReserva(int id)
        {
            // En producción, aquí cambiarías el estado de la reserva a "Cancelada"
            // en lugar de eliminarla de la base de datos
            // Ejemplo: reserva.Estado = "Cancelada";

            TempData["Mensaje"] = "Reserva cancelada correctamente";
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/EliminarReserva/5 (Mantener por compatibilidad, pero redirigir a cancelar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarReserva(int id)
        {
            // Redirigir a cancelar en lugar de eliminar
            return CancelarReserva(id);
        }
    }
}