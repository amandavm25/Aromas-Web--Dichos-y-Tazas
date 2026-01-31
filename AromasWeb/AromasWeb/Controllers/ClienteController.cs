using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Cliente;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class ClienteController : Controller
    {
        private IListarClientes _listarClientes;

        public ClienteController()
        {
            _listarClientes = new LogicaDeNegocio.Clientes.ListarClientes();
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

            // Aquí se obtendrían las reservas del cliente desde la base de datos
            // Por ahora se deja vacío para que lo implementes
            var reservas = new List<Reserva>();
            ViewBag.Reservas = reservas;

            return View();
        }
    }
}