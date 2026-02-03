using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.Abstracciones.Logica.Reserva;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class ClienteController : Controller
    {
        private IListarClientes _listarClientes;
        private IListarReservas _listarReservas;

        public ClienteController()
        {
            _listarClientes = new LogicaDeNegocio.Clientes.ListarClientes();
            _listarReservas = new LogicaDeNegocio.Reservas.ListarReservas();
        }

        // GET: Cliente/ListadoClientes
        public IActionResult ListadoClientes(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Obtener clientes según los filtros
            List<Cliente> clientes;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                // Buscar por nombre y filtrar por estado
                bool estado = filtroEstado == "activo";
                clientes = _listarClientes.BuscarPorNombre(buscar)
                    .FindAll(c => c.Estado == estado);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre (también busca en identificación y teléfono)
                clientes = _listarClientes.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                // Solo filtrar por estado
                bool estado = filtroEstado == "activo";
                clientes = _listarClientes.BuscarPorEstado(estado);
            }
            else
            {
                // Obtener todos
                clientes = _listarClientes.Obtener();
            }

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
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Cliente registrado correctamente";
                return RedirectToAction(nameof(ListadoClientes));
            }

            return View(cliente);
        }

        // GET: Cliente/EditarCliente/5
        public IActionResult EditarCliente(int id)
        {
            var cliente = _listarClientes.ObtenerPorId(id);

            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado";
                return RedirectToAction(nameof(ListadoClientes));
            }

            return View(cliente);
        }

        // POST: Cliente/EditarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(Cliente cliente)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Cliente actualizado correctamente";
                return RedirectToAction(nameof(ListadoClientes));
            }

            return View(cliente);
        }

        // POST: Cliente/EliminarCliente/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCliente(int id)
        {
            // Aquí iría la lógica para eliminar el cliente
            TempData["Mensaje"] = "Cliente eliminado correctamente";
            return RedirectToAction(nameof(ListadoClientes));
        }

        // GET: Cliente/HistorialReservas/5 (Para empleados)
        public IActionResult HistorialReservas(int id)
        {
            var cliente = _listarClientes.ObtenerPorId(id);

            if (cliente == null)
            {
                TempData["Error"] = "Cliente no encontrado";
                return RedirectToAction(nameof(ListadoClientes));
            }

            ViewBag.Cliente = cliente;

            // Obtener las reservas del cliente desde la base de datos
            var reservas = _listarReservas.ObtenerPorCliente(id);
            ViewBag.Reservas = reservas;

            return View();
        }

        // ============================================
        // MÉTODOS PARA CLIENTES (MIS RESERVAS)
        // ============================================

        // GET: Cliente/MisReservas
        public IActionResult MisReservas(string filtro)
        {
            // Obtener el ID del cliente desde la sesión
            // Por ahora usamos un ID de ejemplo, pero deberías obtenerlo de:
            // HttpContext.Session.GetInt32("IdCliente")
            int idClienteActual = 1; // CAMBIAR: Obtener de la sesión

            ViewBag.IdCliente = idClienteActual;
            ViewBag.Filtro = filtro;

            // Obtener todas las reservas del cliente
            var misReservas = _listarReservas.ObtenerPorCliente(idClienteActual);

            // Aplicar filtros si se especifican
            if (!string.IsNullOrEmpty(filtro))
            {
                switch (filtro.ToLower())
                {
                    case "activas":
                        // Reservas futuras o de hoy (Pendiente o Confirmada)
                        misReservas = misReservas
                            .Where(r => (r.EsFutura || r.EsHoy) &&
                                       (r.Estado == "Pendiente" || r.Estado == "Confirmada"))
                            .ToList();
                        break;
                    case "pasadas":
                        // Reservas pasadas o completadas/canceladas
                        misReservas = misReservas
                            .Where(r => r.EsPasada || r.Estado == "Completada" || r.Estado == "Cancelada")
                            .ToList();
                        break;
                    default:
                        // Sin filtro - todas las reservas
                        break;
                }
            }

            // Ordenar: próximas primero, luego pasadas
            misReservas = misReservas
                .OrderByDescending(r => r.EsFutura)
                .ThenBy(r => r.Fecha)
                .ThenBy(r => r.Hora)
                .ToList();

            return View(misReservas);
        }

        // GET: Cliente/EditarMiReserva/5
        public IActionResult EditarMiReserva(int id)
        {
            // Obtener el ID del cliente desde la sesión
            int idClienteActual = 1; // CAMBIAR: Obtener de la sesión

            ViewBag.IdCliente = idClienteActual;

            var reserva = _listarReservas.ObtenerPorId(id);

            if (reserva == null)
            {
                TempData["Error"] = "Reserva no encontrada";
                return RedirectToAction(nameof(MisReservas));
            }

            // Verificar que la reserva pertenece al cliente actual
            if (reserva.IdCliente != idClienteActual)
            {
                TempData["Error"] = "No tienes permiso para editar esta reserva";
                return RedirectToAction(nameof(MisReservas));
            }

            // Verificar que la reserva puede ser editada
            if (!reserva.PuedeModificar)
            {
                TempData["Error"] = "Esta reserva no puede ser modificada";
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
                // Aquí iría la lógica para actualizar la reserva en la base de datos
                TempData["Mensaje"] = "Tu reserva ha sido actualizada correctamente";
                return RedirectToAction(nameof(MisReservas));
            }

            return View(reserva);
        }

        // POST: Cliente/CancelarMiReserva/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarMiReserva(int id)
        {
            // Obtener el ID del cliente desde la sesión
            int idClienteActual = 1; // CAMBIAR: Obtener de la sesión

            var reserva = _listarReservas.ObtenerPorId(id);

            if (reserva == null)
            {
                TempData["Error"] = "Reserva no encontrada";
                return RedirectToAction(nameof(MisReservas));
            }

            // Verificar que la reserva pertenece al cliente actual
            if (reserva.IdCliente != idClienteActual)
            {
                TempData["Error"] = "No tienes permiso para cancelar esta reserva";
                return RedirectToAction(nameof(MisReservas));
            }

            // Verificar que la reserva puede ser cancelada
            if (!reserva.PuedeCancelar)
            {
                TempData["Error"] = "Esta reserva no puede ser cancelada";
                return RedirectToAction(nameof(MisReservas));
            }

            // Aquí iría la lógica para cambiar el estado a "Cancelada" en la base de datos
            TempData["Mensaje"] = "Tu reserva ha sido cancelada correctamente";
            return RedirectToAction(nameof(MisReservas));
        }

        // GET: Cliente/MiPerfil
        public IActionResult MiPerfil()
        {
            // Obtener el ID del cliente desde la sesión
            int idClienteActual = 1; // CAMBIAR: Obtener de la sesión

            var cliente = _listarClientes.ObtenerPorId(idClienteActual);

            if (cliente == null)
            {
                TempData["Error"] = "No se pudo cargar tu perfil";
                return RedirectToAction("Index", "Home");
            }

            return View(cliente);
        }

        // GET: Cliente/EditarPerfil
        public IActionResult EditarPerfil()
        {
            // Obtener el ID del cliente desde la sesión
            int idClienteActual = 1; // CAMBIAR: Obtener de la sesión

            var cliente = _listarClientes.ObtenerPorId(idClienteActual);

            if (cliente == null)
            {
                TempData["Error"] = "No se pudo cargar tu perfil";
                return RedirectToAction("Index", "Home");
            }

            return View(cliente);
        }

        // POST: Cliente/EditarPerfil
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPerfil(Cliente cliente, string ContrasenaActual, string ContrasenaNueva, string ConfirmarContrasenaNueva)
        {
            // Obtener el ID del cliente desde la sesión
            int idClienteActual = 1; // CAMBIAR: Obtener de la sesión

            // Verificar que el cliente está editando su propio perfil
            if (cliente.IdCliente != idClienteActual)
            {
                TempData["Error"] = "No tienes permiso para editar este perfil";
                return RedirectToAction(nameof(MiPerfil));
            }

            // Validar cambio de contraseña si se proporcionó
            if (!string.IsNullOrEmpty(ContrasenaNueva) || !string.IsNullOrEmpty(ConfirmarContrasenaNueva))
            {
                // Validar que se proporcionó la contraseña actual
                if (string.IsNullOrEmpty(ContrasenaActual))
                {
                    ModelState.AddModelError("", "Debes ingresar tu contraseña actual para cambiarla");
                }
                // Validar que las nuevas contraseñas coinciden
                else if (ContrasenaNueva != ConfirmarContrasenaNueva)
                {
                    ModelState.AddModelError("", "Las contraseñas nuevas no coinciden");
                }
                // Validar longitud mínima
                else if (ContrasenaNueva.Length < 8)
                {
                    ModelState.AddModelError("", "La nueva contraseña debe tener al menos 8 caracteres");
                }
                else
                {
                    // Aquí deberías verificar que ContrasenaActual sea correcta
                    // comparándola con la contraseña en la base de datos
                    // Por ahora solo establecemos la nueva contraseña
                    cliente.Contrasena = ContrasenaNueva;
                }
            }

            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar el cliente en la base de datos
                // usando AccesoADatos para guardar los cambios

                TempData["Mensaje"] = "Tu perfil ha sido actualizado correctamente";
                return RedirectToAction(nameof(MiPerfil));
            }

            return View(cliente);
        }
    }
}