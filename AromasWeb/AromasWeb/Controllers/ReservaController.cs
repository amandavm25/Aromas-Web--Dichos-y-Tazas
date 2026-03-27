using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AromasWeb.Controllers
{
    public class ReservaController : Controller
    {
        private IListarReservas _listarReservas;
        private readonly CrearBitacora _crearBitacora;

        public ReservaController()
        {
            _listarReservas = new LogicaDeNegocio.Reservas.ListarReservas();
            _crearBitacora = new CrearBitacora();
        }

        // Devuelve el IdEmpleado de sesión, o null si no hay empleado logueado.
        private int? ObtenerIdEmpleadoSesion()
        {
            int? id = HttpContext.Session.GetInt32("IdEmpleado");
            return (id.HasValue && id.Value > 0) ? id : null;
        }

        // Lanza si no hay empleado en sesión (para acciones exclusivas de empleados).
        private int RequiereEmpleadoSesion()
        {
            int? id = ObtenerIdEmpleadoSesion();
            if (!id.HasValue)
                throw new InvalidOperationException("No hay empleado autenticado en sesión.");
            return id.Value;
        }

        // GET: Reserva/ListadoReservas
        public IActionResult ListadoReservas(string buscar, string filtroEstado, string filtroFecha)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;
            ViewBag.FiltroFecha = filtroFecha;

            List<Reserva> reservas;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado) && !string.IsNullOrEmpty(filtroFecha))
                reservas = AplicarFiltros(buscar, filtroEstado, filtroFecha);
            else if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
                reservas = BuscarPorNombreOTelefono(buscar)
                    .FindAll(r => r.Estado.Equals(filtroEstado, StringComparison.OrdinalIgnoreCase));
            else if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroFecha))
                reservas = FiltrarPorFecha(BuscarPorNombreOTelefono(buscar), filtroFecha);
            else if (!string.IsNullOrEmpty(filtroEstado) && !string.IsNullOrEmpty(filtroFecha))
                reservas = FiltrarPorFecha(_listarReservas.BuscarPorEstado(filtroEstado), filtroFecha);
            else if (!string.IsNullOrEmpty(buscar))
                reservas = BuscarPorNombreOTelefono(buscar);
            else if (!string.IsNullOrEmpty(filtroEstado))
                reservas = _listarReservas.BuscarPorEstado(filtroEstado);
            else if (!string.IsNullOrEmpty(filtroFecha))
                reservas = FiltrarPorFecha(_listarReservas.Obtener(), filtroFecha);
            else
                reservas = _listarReservas.Obtener();

            reservas = reservas.OrderBy(r => r.Fecha).ThenBy(r => r.Hora).ToList();

            return View(reservas);
        }

        private List<Reserva> BuscarPorNombreOTelefono(string buscar)
        {
            var porNombre = _listarReservas.BuscarPorNombre(buscar);
            var porTelefono = _listarReservas.BuscarPorTelefono(buscar);
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
                reservas = reservas.FindAll(r => r.Estado.Equals(filtroEstado, StringComparison.OrdinalIgnoreCase));

            if (!string.IsNullOrEmpty(filtroFecha))
                reservas = FiltrarPorFecha(reservas, filtroFecha);

            return reservas;
        }

        // ============================================================
        // ACCIONES DE CLIENTES — sin bitácora
        // ============================================================

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

        // POST: Reserva/CrearReserva (Para clientes) — sin bitácora
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

        // GET: Reserva/MisReservas (Para clientes autenticados)
        public IActionResult MisReservas(string filtro)
        {
            ViewBag.Filtro = filtro;

            int idCliente = ObtenerIdClienteActual();

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

            reservas = filtro?.ToLower() switch
            {
                "activas" => reservas.Where(r => r.EsFutura || r.EsHoy).ToList(),
                "pasadas" => reservas.Where(r => r.EsPasada).ToList(),
                _ => reservas
            };

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

        // POST: Reserva/EditarMiReserva (Para clientes autenticados) — sin bitácora
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

        // ============================================================
        // ACCIONES DE EMPLEADOS — con bitácora, requieren sesión válida
        // ============================================================

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
                int idEmpleado = RequiereEmpleadoSesion();

                _crearBitacora.RegistrarAccion(
                    idEmpleado: idEmpleado,
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                    accion: Bitacora.Acciones.Crear,
                    tablaAfectada: "Reserva",
                    descripcion: $"Empleado registró reserva para cliente ID: {reserva.IdCliente}, fecha: {reserva.FechaFormateada} {reserva.HoraFormateada}, {reserva.CantidadPersonas} persona(s)",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        reserva.IdCliente,
                        reserva.Fecha,
                        reserva.Hora,
                        reserva.CantidadPersonas,
                        reserva.Estado,
                        reserva.Observaciones
                    })
                );

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
                        return Json(new { success = true, data = cliente });

                    return Json(new { success = false, message = "Cliente no encontrado" });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar cliente: {ex.Message}");
                    return Json(new { success = false, message = "Error al buscar cliente" });
                }
            }
        }

        // GET: Reserva/EditarReserva/5 (empleados)
        public IActionResult EditarReserva(int id)
        {
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var clientes = contexto.Cliente
                        .Where(c => c.Estado == true)
                        .OrderBy(c => c.Nombre)
                        .Select(c => new { Id = c.IdCliente, Nombre = c.Nombre + " " + c.Apellidos, Telefono = c.Telefono })
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

        // POST: Reserva/EditarReserva (empleados)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReserva(Reserva reserva)
        {
            if (ModelState.IsValid)
            {
                int idEmpleado = RequiereEmpleadoSesion();

                _crearBitacora.RegistrarAccion(
                    idEmpleado: idEmpleado,
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                    accion: Bitacora.Acciones.Actualizar,
                    tablaAfectada: "Reserva",
                    descripcion: $"Se editó la reserva ID: {reserva.IdReserva} (cliente: {reserva.NombreCliente ?? reserva.IdCliente.ToString()})",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        reserva.IdReserva,
                        reserva.IdCliente,
                        reserva.Fecha,
                        reserva.Hora,
                        reserva.CantidadPersonas,
                        reserva.Estado,
                        reserva.Observaciones
                    })
                );

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
                        .Select(c => new { Id = c.IdCliente, Nombre = c.Nombre + " " + c.Apellidos, Telefono = c.Telefono })
                        .ToList();
                    ViewBag.Clientes = clientes;
                }
                catch { ViewBag.Clientes = new List<dynamic>(); }
            }

            return View(reserva);
        }

        // GET: Reserva/HistorialReservas/5 (Para empleados)
        public IActionResult HistorialReservas(int id)
        {
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

            var reservas = _listarReservas.ObtenerPorCliente(id);
            reservas = reservas.OrderByDescending(r => r.Fecha).ThenByDescending(r => r.Hora).ToList();
            ViewBag.Reservas = reservas;

            return View();
        }

        // POST: Reserva/CambiarEstado (empleados)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            int idEmpleado = RequiereEmpleadoSesion();

            _crearBitacora.RegistrarAccion(
                idEmpleado: idEmpleado,
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                accion: Bitacora.Acciones.CambiarEstado,
                tablaAfectada: "Reserva",
                descripcion: $"Se cambió el estado de la reserva ID: {id} a '{nuevoEstado}'",
                datosNuevos: JsonSerializer.Serialize(new { IdReserva = id, Estado = nuevoEstado })
            );

            TempData["Mensaje"] = $"Estado de la reserva cambiado a {nuevoEstado}";
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/CancelarReserva/5 (empleados)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarReserva(int id)
        {
            int idEmpleado = RequiereEmpleadoSesion();

            var reserva = _listarReservas.ObtenerPorId(id);

            _crearBitacora.RegistrarAccion(
                idEmpleado: idEmpleado,
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                accion: Bitacora.Acciones.CambiarEstado,
                tablaAfectada: "Reserva",
                descripcion: $"Se canceló la reserva ID: {id} (cliente: {reserva?.NombreCliente ?? id.ToString()})",
                datosAnteriores: reserva != null ? JsonSerializer.Serialize(new { reserva.Estado }) : null,
                datosNuevos: JsonSerializer.Serialize(new { Estado = "Cancelada" })
            );

            TempData["Mensaje"] = "Reserva cancelada correctamente";
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/EliminarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarReserva(int id)
        {
            return CancelarReserva(id);
        }

        // Método auxiliar para obtener el IdCliente del usuario autenticado
        private int ObtenerIdClienteActual()
        {
            var claim = User.FindFirst("IdCliente");
            if (claim != null && int.TryParse(claim.Value, out int idCliente))
                return idCliente;

            return HttpContext.Session.GetInt32("IdCliente") ?? 0;
        }
    }
}