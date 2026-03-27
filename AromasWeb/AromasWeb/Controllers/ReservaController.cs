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
        private readonly IListarReservas _listarReservas;
        private readonly IObtenerReserva _obtenerReserva;
        private readonly ICrearReserva _crearReserva;
        private readonly IActualizarReserva _actualizarReserva;
        private readonly CrearBitacora _crearBitacora;

        public ReservaController()
        {
            _listarReservas = new LogicaDeNegocio.Reservas.ListarReservas();
            _obtenerReserva = new LogicaDeNegocio.Reservas.ObtenerReserva();
            _crearReserva = new LogicaDeNegocio.Reservas.CrearReserva();
            _actualizarReserva = new LogicaDeNegocio.Reservas.ActualizarReserva();
            _crearBitacora = new CrearBitacora();
        }

        // Helpers de sesión

        private int? ObtenerIdEmpleadoSesion()
        {
            int? id = HttpContext.Session.GetInt32("IdEmpleado");
            return (id.HasValue && id.Value > 0) ? id : null;
        }

        private int RequiereEmpleadoSesion()
        {
            int? id = ObtenerIdEmpleadoSesion();
            if (!id.HasValue)
                throw new InvalidOperationException("No hay empleado autenticado en sesión.");
            return id.Value;
        }

        private int ObtenerIdClienteActual()
        {
            var claim = User.FindFirst("IdCliente");
            if (claim != null && int.TryParse(claim.Value, out int idCliente))
                return idCliente;
            return HttpContext.Session.GetInt32("IdCliente") ?? 0;
        }

        // ════════════════════════════════════════════════════════════════
        // LISTADO (empleados)
        // ════════════════════════════════════════════════════════════════

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

        // ════════════════════════════════════════════════════════════════
        // ACCIONES DE CLIENTES
        // ════════════════════════════════════════════════════════════════

        // GET: Reserva/CrearReserva
        public IActionResult CrearReserva()
        {
            int idCliente = ObtenerIdClienteActual();
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .Where(c => c.IdCliente == idCliente && c.Estado == true)
                        .Select(c => new
                        {
                            c.IdCliente,
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            c.Identificacion,
                            c.Telefono,
                            c.Correo
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

        // POST: Reserva/CrearReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> CrearReserva(Reserva reserva, string Hora)
        {
            if (!string.IsNullOrEmpty(Hora) && TimeSpan.TryParse(Hora, out TimeSpan horaParseada))
            {
                reserva.Hora = horaParseada;
                ModelState.Remove("Hora");
            }

            ModelState.Remove("Estado");
            ModelState.Remove("Observaciones");

            // Forzar la fecha a UTC antes de guardar
            reserva.Fecha = DateTime.SpecifyKind(reserva.Fecha.Date, DateTimeKind.Utc);

            if (!ModelState.IsValid)
            {
                RecargarViewBagCliente();
                return View(reserva);
            }

            try
            {
                reserva.Estado = "Pendiente";
                await _crearReserva.Crear(reserva);
                TempData["Mensaje"] = "Reserva registrada correctamente. Recibirá una confirmación pronto.";
                return RedirectToAction(nameof(MisReservas));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                RecargarViewBagCliente();
                return View(reserva);
            }
        }

        // GET: Reserva/MisReservas
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
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            c.Identificacion,
                            c.Telefono,
                            c.Correo
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

        // GET: Reserva/EditarMiReserva/5
        public IActionResult EditarMiReserva(int id)
        {
            int idCliente = ObtenerIdClienteActual();
            var reserva = _obtenerReserva.Obtener(id);

            if (reserva == null || reserva.IdCliente != idCliente)
            {
                TempData["Error"] = "Reserva no encontrada.";
                return RedirectToAction(nameof(MisReservas));
            }
            if (!reserva.PuedeModificar)
            {
                TempData["Error"] = "Esta reserva no puede modificarse.";
                return RedirectToAction(nameof(MisReservas));
            }
            return View(reserva);
        }

        // POST: Reserva/EditarMiReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarMiReserva(Reserva reserva, string Hora)
        {
            if (!string.IsNullOrEmpty(Hora) && TimeSpan.TryParse(Hora, out TimeSpan horaParseada))
            {
                reserva.Hora = horaParseada;
                ModelState.Remove("Hora");
            }

            ModelState.Remove("Estado");
            ModelState.Remove("Observaciones");
            ModelState.Remove("NombreCliente");
            ModelState.Remove("TelefonoCliente");

            // Forzar la fecha a UTC antes de guardar
            reserva.Fecha = DateTime.SpecifyKind(reserva.Fecha.Date, DateTimeKind.Utc);

            if (!ModelState.IsValid)
                return View(reserva);

            try
            {
                _actualizarReserva.Actualizar(reserva);
                TempData["Mensaje"] = "Reserva actualizada correctamente.";
                return RedirectToAction(nameof(MisReservas));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(reserva);
            }
        }

        // POST: Reserva/CancelarMiReserva/5  (exclusivo del cliente)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarMiReserva(int id)
        {
            try
            {
                int idCliente = ObtenerIdClienteActual();
                var reserva = _obtenerReserva.Obtener(id);

                if (reserva == null || reserva.IdCliente != idCliente)
                {
                    TempData["Error"] = "No tienes permiso para cancelar esta reserva.";
                    return RedirectToAction(nameof(MisReservas));
                }

                if (!reserva.PuedeCancelar)
                {
                    TempData["Error"] = "Esta reserva ya no puede cancelarse.";
                    return RedirectToAction(nameof(MisReservas));
                }

                _actualizarReserva.ActualizarEstado(id, "Cancelada");
                TempData["Mensaje"] = "Reserva cancelada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(MisReservas));
        }

        // ════════════════════════════════════════════════════════════════
        // ACCIONES DE EMPLEADOS
        // ════════════════════════════════════════════════════════════════

        // GET: Reserva/RegistrarReservaCliente
        public IActionResult RegistrarReservaCliente()
        {
            return View();
        }

        // POST: Reserva/RegistrarReservaCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async System.Threading.Tasks.Task<IActionResult> RegistrarReservaCliente(Reserva reserva, string Hora)
        {
            if (!string.IsNullOrEmpty(Hora) && TimeSpan.TryParse(Hora, out TimeSpan horaParseada))
            {
                reserva.Hora = horaParseada;
                ModelState.Remove("Hora");
            }

            ModelState.Remove("Estado");
            ModelState.Remove("Observaciones");

            // Forzar la fecha a UTC antes de guardar
            reserva.Fecha = DateTime.SpecifyKind(reserva.Fecha.Date, DateTimeKind.Utc);

            if (!ModelState.IsValid)
                return View(reserva);

            try
            {
                int idEmpleado = RequiereEmpleadoSesion();
                reserva.Estado = "Pendiente";
                await _crearReserva.Crear(reserva);

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

                TempData["Mensaje"] = "Reserva registrada correctamente.";
                return RedirectToAction(nameof(ListadoReservas));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(reserva);
            }
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
                            c.Identificacion,
                            Nombre = c.Nombre + " " + c.Apellidos,
                            c.Telefono
                        })
                        .FirstOrDefault();

                    if (cliente != null)
                        return Json(new { success = true, data = cliente });

                    return Json(new { success = false, message = "Cliente no encontrado." });
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar cliente: {ex.Message}");
                    return Json(new { success = false, message = "Error al buscar cliente." });
                }
            }
        }

        // GET: Reserva/EditarReserva/5
        public IActionResult EditarReserva(int id)
        {
            CargarClientesEnViewBag();
            var reserva = _obtenerReserva.Obtener(id);
            if (reserva == null)
            {
                TempData["Error"] = "Reserva no encontrada.";
                return RedirectToAction(nameof(ListadoReservas));
            }
            return View(reserva);
        }

        // POST: Reserva/EditarReserva
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReserva(Reserva reserva, string Hora)
        {
            if (!string.IsNullOrEmpty(Hora) && TimeSpan.TryParse(Hora, out TimeSpan horaParseada))
            {
                reserva.Hora = horaParseada;
                ModelState.Remove("Hora");
            }

            ModelState.Remove("Estado");
            ModelState.Remove("Observaciones");

            // Forzar la fecha a UTC antes de guardar
            reserva.Fecha = DateTime.SpecifyKind(reserva.Fecha.Date, DateTimeKind.Utc);

            if (!ModelState.IsValid)
            {
                CargarClientesEnViewBag();
                return View(reserva);
            }

            try
            {
                int idEmpleado = RequiereEmpleadoSesion();
                _actualizarReserva.Actualizar(reserva);

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

                TempData["Mensaje"] = "Reserva actualizada correctamente.";
                return RedirectToAction(nameof(ListadoReservas));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                CargarClientesEnViewBag();
                return View(reserva);
            }
        }

        // GET: Reserva/HistorialReservas/5
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
                            c.IdCliente,
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            c.Identificacion,
                            c.Correo,
                            c.Telefono
                        })
                        .FirstOrDefault();

                    if (cliente == null)
                    {
                        TempData["Error"] = "Cliente no encontrado.";
                        return RedirectToAction("ListadoClientes", "Cliente");
                    }
                    ViewBag.Cliente = cliente;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener cliente: {ex.Message}");
                    TempData["Error"] = "Error al cargar información del cliente.";
                    return RedirectToAction("ListadoClientes", "Cliente");
                }
            }

            var reservas = _listarReservas.ObtenerPorCliente(id)
                .OrderByDescending(r => r.Fecha)
                .ThenByDescending(r => r.Hora)
                .ToList();

            ViewBag.Reservas = reservas;
            return View();
        }

        // POST: Reserva/CambiarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id, string nuevoEstado)
        {
            try
            {
                int idEmpleado = RequiereEmpleadoSesion();
                _actualizarReserva.ActualizarEstado(id, nuevoEstado);

                _crearBitacora.RegistrarAccion(
                    idEmpleado: idEmpleado,
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                    accion: Bitacora.Acciones.CambiarEstado,
                    tablaAfectada: "Reserva",
                    descripcion: $"Se cambió el estado de la reserva ID: {id} a '{nuevoEstado}'",
                    datosNuevos: JsonSerializer.Serialize(new { IdReserva = id, Estado = nuevoEstado })
                );

                TempData["Mensaje"] = $"Estado de la reserva cambiado a {nuevoEstado}.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/CancelarReserva/5  (exclusivo del empleado)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarReserva(int id)
        {
            try
            {
                int idEmpleado = RequiereEmpleadoSesion();
                var reserva = _obtenerReserva.Obtener(id);

                _actualizarReserva.ActualizarEstado(id, "Cancelada");

                _crearBitacora.RegistrarAccion(
                    idEmpleado: idEmpleado,
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                    accion: Bitacora.Acciones.CambiarEstado,
                    tablaAfectada: "Reserva",
                    descripcion: $"Se canceló la reserva ID: {id} (cliente: {reserva?.NombreCliente ?? id.ToString()})",
                    datosAnteriores: reserva != null ? JsonSerializer.Serialize(new { reserva.Estado }) : null,
                    datosNuevos: JsonSerializer.Serialize(new { Estado = "Cancelada" })
                );

                TempData["Mensaje"] = "Reserva cancelada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListadoReservas));
        }

        // POST: Reserva/ConfirmarReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmarReserva(int id)
        {
            try
            {
                int idEmpleado = RequiereEmpleadoSesion();
                var reserva = _obtenerReserva.Obtener(id);

                if (reserva == null)
                {
                    TempData["Error"] = "Reserva no encontrada.";
                    return RedirectToAction(nameof(ListadoReservas));
                }

                if (reserva.Estado != "Pendiente")
                {
                    TempData["Error"] = "Solo se pueden confirmar reservas en estado Pendiente.";
                    return RedirectToAction(nameof(ListadoReservas));
                }

                _actualizarReserva.ActualizarEstado(id, "Confirmada");

                _crearBitacora.RegistrarAccion(
                    idEmpleado: idEmpleado,
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de reservas"),
                    accion: Bitacora.Acciones.CambiarEstado,
                    tablaAfectada: "Reserva",
                    descripcion: $"Se confirmó la reserva ID: {id} (cliente: {reserva.NombreCliente})",
                    datosAnteriores: JsonSerializer.Serialize(new { reserva.Estado }),
                    datosNuevos: JsonSerializer.Serialize(new { Estado = "Confirmada" })
                );

                TempData["Mensaje"] = "Reserva confirmada correctamente.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return RedirectToAction(nameof(ListadoReservas));
        }

        // Privados

        private void CargarClientesEnViewBag()
        {
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    ViewBag.Clientes = contexto.Cliente
                        .Where(c => c.Estado == true)
                        .OrderBy(c => c.Nombre)
                        .Select(c => new { Id = c.IdCliente, Nombre = c.Nombre + " " + c.Apellidos, c.Telefono })
                        .ToList();
                }
                catch
                {
                    ViewBag.Clientes = new List<dynamic>();
                }
            }
        }

        private void RecargarViewBagCliente()
        {
            int idCliente = ObtenerIdClienteActual();
            using (var contexto = new AromasWeb.AccesoADatos.Contexto())
            {
                try
                {
                    var cliente = contexto.Cliente
                        .Where(c => c.IdCliente == idCliente && c.Estado == true)
                        .Select(c => new {
                            c.IdCliente,
                            NombreCompleto = c.Nombre + " " + c.Apellidos,
                            c.Identificacion,
                            c.Telefono,
                            c.Correo
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
                    Console.WriteLine($"Error al recargar cliente: {ex.Message}");
                }
            }
        }
    }
}