using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Reserva;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class ReservaController : Controller
    {
        private IListarReservas _listarReservas;

        public ReservaController()
        {
            _listarReservas = new LogicaDeNegocio.Reservas.ListarReservas();
        }

        // GET: Reserva/ListadoReservas
        public IActionResult ListadoReservas(string buscar, string filtroEstado, string filtroFecha)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.FiltroFecha = filtroFecha;

            // Obtener reservas según los filtros
            List<Reserva> reservas;

            // Aplicar filtros de manera combinada
            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado) && !string.IsNullOrEmpty(filtroFecha))
            {
                // Buscar con todos los filtros
                reservas = AplicarFiltros(buscar, filtroEstado, filtroFecha);
            }
            else if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                // Buscar por nombre/teléfono y filtrar por estado
                reservas = BuscarPorNombreOTelefono(buscar)
                    .FindAll(r => r.Estado.Equals(filtroEstado, StringComparison.OrdinalIgnoreCase));
            }
            else if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroFecha))
            {
                // Buscar por nombre/teléfono y filtrar por fecha
                reservas = FiltrarPorFecha(BuscarPorNombreOTelefono(buscar), filtroFecha);
            }
            else if (!string.IsNullOrEmpty(filtroEstado) && !string.IsNullOrEmpty(filtroFecha))
            {
                // Filtrar por estado y fecha
                reservas = FiltrarPorFecha(_listarReservas.BuscarPorEstado(filtroEstado), filtroFecha);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre o teléfono
                reservas = BuscarPorNombreOTelefono(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                // Solo filtrar por estado
                reservas = _listarReservas.BuscarPorEstado(filtroEstado);
            }
            else if (!string.IsNullOrEmpty(filtroFecha))
            {
                // Solo filtrar por fecha
                reservas = FiltrarPorFecha(_listarReservas.Obtener(), filtroFecha);
            }
            else
            {
                // Obtener todas
                reservas = _listarReservas.Obtener();
            }

            // Ordenar por fecha y hora (más próximas primero)
            reservas = reservas.OrderBy(r => r.Fecha).ThenBy(r => r.Hora).ToList();

            return View(reservas);
        }

        // Métodos auxiliares privados
        private List<Reserva> BuscarPorNombreOTelefono(string buscar)
        {
            var porNombre = _listarReservas.BuscarPorNombre(buscar);
            var porTelefono = _listarReservas.BuscarPorTelefono(buscar);

            // Combinar y eliminar duplicados
            return porNombre.Union(porTelefono).Distinct().ToList();
        }

        private List<Reserva> FiltrarPorFecha(List<Reserva> reservas, string filtroFecha)
        {
            return filtroFecha?.ToLower() switch
            {
                "hoy" => reservas.Where(r => r.EsHoy).ToList(),
                "proximas" => reservas.Where(r => r.EsFutura || r.EsHoy).ToList(),
                "pasadas" => reservas.Where(r => r.EsPasada).ToList(),
                _ => reservas
            };
        }

        private List<Reserva> AplicarFiltros(string buscar, string filtroEstado, string filtroFecha)
        {
            var reservas = BuscarPorNombreOTelefono(buscar);

            if (!string.IsNullOrEmpty(filtroEstado))
            {
                reservas = reservas.FindAll(r => r.Estado.Equals(filtroEstado, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(filtroFecha))
            {
                reservas = FiltrarPorFecha(reservas, filtroFecha);
            }

            return reservas;
        }

        // GET: Reserva/CrearReserva (Para clientes)
        public IActionResult CrearReserva()
        {
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .Where(c => c.Estado == true)
                        .OrderBy(c => c.IdCliente)
                        .Select(c => new
                        {
                            IdCliente = c.IdCliente,
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            Identificacion = c.Identificacion,
                            Telefono = c.Telefono,
                            Correo = c.Correo
                        })
                        .FirstOrDefault();

                    ViewBag.IdCliente = cliente?.IdCliente;
                    ViewBag.NombreCliente = cliente?.NombreCompleto;
                    ViewBag.Identificacion = cliente?.Identificacion;
                    ViewBag.Telefono = cliente?.Telefono;
                    ViewBag.Correo = cliente?.Correo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar cliente: {ex.Message}");
                }
            }

            return View();
        }

        // POST: Reserva/CrearReserva (Para clientes)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
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
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Reserva registrada correctamente";
                return RedirectToAction(nameof(ListadoReservas));
            }

            return View(reserva);
        }

        // GET: Reserva/BuscarClientePorIdentificacion (AJAX)
        [HttpGet]
        public JsonResult BuscarClientePorIdentificacion(string identificacion)
        {
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .Where(c => c.Identificacion == identificacion && c.Estado == true)
                        .Select(c => new
                        {
                            Id = c.IdCliente,
                            Identificacion = c.Identificacion,
                            Nombre = c.Nombre + " " + c.Apellidos,
                            Telefono = c.Telefono
                        })
                        .FirstOrDefault();

                    if (cliente != null)
                    {
                        return Json(new { success = true, data = cliente });
                    }

                    return Json(new { success = false, message = "Cliente no encontrado" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar cliente: {ex.Message}");
                    return Json(new { success = false, message = "Error al buscar cliente" });
                }
            }
        }

        // GET: Reserva/EditarReserva/5
        public IActionResult EditarReserva(int id)
        {
            // Cargar clientes desde la base de datos
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var clientes = contexto.Cliente
                        .Where(c => c.Estado == true)
                        .OrderBy(c => c.Nombre)
                        .Select(c => new
                        {
                            Id = c.IdCliente,
                            Nombre = c.Nombre + " " + c.Apellidos,
                            Telefono = c.Telefono
                        })
                        .ToList();

                    ViewBag.Clientes = clientes;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar clientes: {ex.Message}");
                    ViewBag.Clientes = new List<dynamic>();
                }
            }

            var reserva = _listarReservas.ObtenerPorId(id);

            if (reserva == null)
            {
                TempData["Error"] = "Reserva no encontrada";
                return RedirectToAction(nameof(ListadoReservas));
            }

            return View(reserva);
        }

        // POST: Reserva/EditarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Reserva actualizada correctamente";
                return RedirectToAction(nameof(ListadoReservas));
            }

            // Recargar clientes si hay error
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var clientes = contexto.Cliente
                        .Where(c => c.Estado == true)
                        .OrderBy(c => c.Nombre)
                        .Select(c => new
                        {
                            Id = c.IdCliente,
                            Nombre = c.Nombre + " " + c.Apellidos,
                            Telefono = c.Telefono
                        })
                        .ToList();

                    ViewBag.Clientes = clientes;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar clientes: {ex.Message}");
                    ViewBag.Clientes = new List<dynamic>();
                }
            }

            return View(reserva);
        }

        // GET: Reserva/HistorialReservas/5 (Para empleados - ver historial de un cliente)
        public IActionResult HistorialReservas(int id)
        {
            // Obtener información del cliente desde la base de datos
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .Where(c => c.IdCliente == id)
                        .Select(c => new
                        {
                            IdCliente = c.IdCliente,
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            Identificacion = c.Identificacion,
                            Correo = c.Correo,
                            Telefono = c.Telefono
                        })
                        .FirstOrDefault();

                    if (cliente == null)
                    {
                        TempData["Error"] = "Cliente no encontrado";
                        return RedirectToAction("ListadoClientes", "Cliente");
                    }

                    ViewBag.Cliente = cliente;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener cliente: {ex.Message}");
                    TempData["Error"] = "Error al cargar información del cliente";
                    return RedirectToAction("ListadoClientes", "Cliente");
                }
            }

            // Reservas del cliente
            var reservas = _listarReservas.ObtenerPorCliente(id);

            // Ordenar por fecha descendente (más recientes primero)
            reservas = reservas.OrderByDescending(r => r.Fecha).ThenByDescending(r => r.Hora).ToList();

            ViewBag.Reservas = reservas;

            return View();
        }

        // GET: Reserva/MisReservas (Para clientes autenticados)
        public IActionResult MisReservas(string filtro)
        {
            ViewBag.Filtro = filtro;

            int idCliente = ObtenerIdClienteActual();

            // Cargar datos del cliente para la tarjeta de perfil
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .Where(c => c.IdCliente == idCliente && c.Estado == true)
                        .Select(c => new
                        {
                            IdCliente = c.IdCliente,
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            Identificacion = c.Identificacion,
                            Telefono = c.Telefono,
                            Correo = c.Correo
                        })
                        .FirstOrDefault();

                    ViewBag.NombreCliente = cliente?.NombreCompleto;
                    ViewBag.Identificacion = cliente?.Identificacion;
                    ViewBag.Telefono = cliente?.Telefono;
                    ViewBag.Correo = cliente?.Correo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al cargar cliente: {ex.Message}");
                }
            }

            var reservas = _listarReservas.ObtenerPorCliente(idCliente);

            // Aplicar filtro
            reservas = filtro?.ToLower() switch
            {
                "activas" => reservas.Where(r => r.EsFutura || r.EsHoy).ToList(),
                "pasadas" => reservas.Where(r => r.EsPasada).ToList(),
                _ => reservas
            };

            // Ordenar: próximas primero, luego pasadas
            reservas = reservas
                .OrderByDescending(r => r.EsFutura || r.EsHoy)
                .ThenBy(r => r.Fecha)
                .ThenBy(r => r.Hora)
                .ToList();

            return View(reservas);
        }

        // GET: Reserva/EditarMiReserva/5 (Para clientes autenticados)
        public IActionResult EditarMiReserva(int id)
        {
            int idCliente = ObtenerIdClienteActual();

            var reserva = _listarReservas.ObtenerPorId(id);

            if (reserva == null || reserva.IdCliente != idCliente)
            {
                TempData["Error"] = "Reserva no encontrada";
                return RedirectToAction(nameof(MisReservas));
            }

            if (!reserva.PuedeModificar)
            {
                TempData["Error"] = "Esta reserva no puede modificarse";
                return RedirectToAction(nameof(MisReservas));
            }

            return View(reserva);
        }

        // POST: Reserva/EditarMiReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarMiReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Reserva actualizada correctamente";
                return RedirectToAction(nameof(MisReservas));
            }

            return View(reserva);
        }

        // Método auxiliar para obtener el IdCliente del usuario autenticado
        private int ObtenerIdClienteActual()
        {
            // Obtener desde claims del usuario autenticado
            var claim = User.FindFirst("IdCliente");
            if (claim != null && int.TryParse(claim.Value, out int idCliente))
                return idCliente;

            // Alternativa: desde sesión
            var idSesion = HttpContext.Session.GetInt32("IdCliente");
            return idSesion ?? 0;
        }

        // POST: Reserva/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            // Aquí iría la lógica para cambiar el estado en la base de datos
            TempData["Mensaje"] = $"Estado de la reserva cambiado a {nuevoEstado}";
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/CancelarReserva/5 (Cambia el estado a "Cancelada" en lugar de eliminar)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarReserva(int id)
        {
            // Aquí iría la lógica para cambiar el estado a "Cancelada" en la base de datos
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