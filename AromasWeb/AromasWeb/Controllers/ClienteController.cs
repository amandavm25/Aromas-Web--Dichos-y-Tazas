using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Cliente;
using System;
using System.Threading.Tasks;
using AromasWeb.Helpers;

namespace AromasWeb.Controllers
{
    public class ClienteController : Controller
    {
        private IListarClientes _listarClientes;
        private ICrearCliente _crearCliente;
        private IActualizarCliente _actualizarCliente;
        private IObtenerCliente _obtenerCliente;

        public ClienteController()
        {
            _listarClientes = new LogicaDeNegocio.Clientes.ListarClientes();
            _crearCliente = new LogicaDeNegocio.Clientes.CrearCliente();
            _actualizarCliente = new LogicaDeNegocio.Clientes.ActualizarCliente();
            _obtenerCliente = new LogicaDeNegocio.Clientes.ObtenerCliente();
        }

        // GET: Cliente/ListadoClientes
        public IActionResult ListadoClientes(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            List<Cliente> clientes;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                bool estado = filtroEstado == "activo";
                clientes = _listarClientes.BuscarPorNombre(buscar)
                    .FindAll(c => c.Estado == estado);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                clientes = _listarClientes.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                bool estado = filtroEstado == "activo";
                clientes = _listarClientes.BuscarPorEstado(estado);
            }
            else
            {
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
        public async Task<IActionResult> CrearCliente(Cliente cliente, string ConfirmarContrasena)
        {
            try
            {
                // Remover validación de campos calculados
                ModelState.Remove("EstadoTexto");
                ModelState.Remove("FechaRegistroFormateada");
                ModelState.Remove("UltimaReservaFormateada");
                ModelState.Remove("DiasDesdeUltimaReserva");
                ModelState.Remove("EsClienteFrecuente");
                ModelState.Remove("UltimaReserva");

                // Validación robusta de contraseña
                if (string.IsNullOrWhiteSpace(cliente.Contrasena))
                {
                    ModelState.AddModelError("Contrasena", "La contraseña es requerida");
                }
                else
                {
                    if (!PasswordValidator.EsContrasenaValida(cliente.Contrasena, out string mensajeError))
                    {
                        ModelState.AddModelError("Contrasena", mensajeError);
                    }
                }

                if (string.IsNullOrWhiteSpace(ConfirmarContrasena))
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Debes confirmar la contraseña");
                }
                else if (cliente.Contrasena != ConfirmarContrasena)
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas no coinciden");
                }

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(cliente);
                }

                cliente.Estado = true;
                cliente.FechaRegistro = DateTime.Now;

                int resultado = await _crearCliente.Crear(cliente);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Cliente registrado correctamente";
                    return RedirectToAction(nameof(ListadoClientes));
                }
                else
                {
                    TempData["Error"] = "No se pudo registrar el cliente en la base de datos";
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearCliente: {ex.Message}");

                if (ex.Message.Contains("identificación"))
                {
                    TempData["Error"] = "Ya existe un cliente registrado con esa identificación";
                }
                else if (ex.Message.Contains("correo"))
                {
                    TempData["Error"] = "Ya existe un cliente registrado con ese correo electrónico";
                }
                else
                {
                    TempData["Error"] = $"Error al registrar el cliente: {ex.Message}";
                }

                return View(cliente);
            }
        }

        // GET: Cliente/EditarCliente/5
        public IActionResult EditarCliente(int id)
        {
            try
            {
                var cliente = _obtenerCliente.Obtener(id);

                if (cliente == null)
                {
                    TempData["Error"] = "Cliente no encontrado";
                    return RedirectToAction(nameof(ListadoClientes));
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el cliente: {ex.Message}";
                return RedirectToAction(nameof(ListadoClientes));
            }
        }

        // POST: Cliente/EditarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(Cliente cliente, string ContrasenaActual, string ContrasenaNueva, string ConfirmarContrasenaNueva)
        {
            try
            {
                // Remover validación de campos calculados
                ModelState.Remove("EstadoTexto");
                ModelState.Remove("FechaRegistroFormateada");
                ModelState.Remove("UltimaReservaFormateada");
                ModelState.Remove("DiasDesdeUltimaReserva");
                ModelState.Remove("EsClienteFrecuente");
                ModelState.Remove("Contrasena");
                ModelState.Remove("UltimaReserva");
                ModelState.Remove("ContrasenaActual");
                ModelState.Remove("ContrasenaNueva");
                ModelState.Remove("ConfirmarContrasenaNueva");

                bool intentaCambiarContrasena = !string.IsNullOrWhiteSpace(ContrasenaNueva) ||
                                                 !string.IsNullOrWhiteSpace(ConfirmarContrasenaNueva) ||
                                                 !string.IsNullOrWhiteSpace(ContrasenaActual);

                if (intentaCambiarContrasena)
                {
                    var clienteActual = _obtenerCliente.Obtener(cliente.IdCliente);

                    if (clienteActual == null)
                    {
                        ModelState.AddModelError("", "No se pudo cargar la información del cliente");
                        return View(cliente);
                    }

                    if (string.IsNullOrWhiteSpace(ContrasenaActual))
                    {
                        ModelState.AddModelError("ContrasenaActual", "Debes ingresar la contraseña actual");
                    }
                    else
                    {
                        if (clienteActual.Contrasena != ContrasenaActual)
                        {
                            ModelState.AddModelError("ContrasenaActual", "La contraseña actual es incorrecta");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(ContrasenaNueva))
                    {
                        ModelState.AddModelError("ContrasenaNueva", "Debes ingresar la nueva contraseña");
                    }
                    else
                    {
                        // ⭐ Validación robusta de la nueva contraseña
                        if (!PasswordValidator.EsContrasenaValida(ContrasenaNueva, out string mensajeError))
                        {
                            ModelState.AddModelError("ContrasenaNueva", mensajeError);
                        }
                    }

                    if (string.IsNullOrWhiteSpace(ConfirmarContrasenaNueva))
                    {
                        ModelState.AddModelError("ConfirmarContrasenaNueva", "Debes confirmar la nueva contraseña");
                    }

                    if (!string.IsNullOrWhiteSpace(ContrasenaNueva) && !string.IsNullOrWhiteSpace(ConfirmarContrasenaNueva))
                    {
                        if (ContrasenaNueva != ConfirmarContrasenaNueva)
                        {
                            ModelState.AddModelError("ConfirmarContrasenaNueva", "Las contraseñas nuevas no coinciden");
                        }
                    }

                    if (!ModelState.IsValid)
                    {
                        ViewBag.ErrorMessage = "Por favor, corrige los errores en el cambio de contraseña";
                        return View(cliente);
                    }

                    cliente.Contrasena = ContrasenaNueva;
                }
                else
                {
                    cliente.Contrasena = null;
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Por favor, corrige los errores del formulario";
                    return View(cliente);
                }

                int resultado = _actualizarCliente.Actualizar(cliente);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Cliente actualizado correctamente";
                    return RedirectToAction(nameof(ListadoClientes));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar el cliente en la base de datos");
                    return View(cliente);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción capturada: {ex.Message}");
                ModelState.AddModelError("", $"Error al actualizar el cliente: {ex.Message}");
                return View(cliente);
            }
        }
    }
}