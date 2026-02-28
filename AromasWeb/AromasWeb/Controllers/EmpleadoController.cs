using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Empleado;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;
using AromasWeb.Helpers;

namespace AromasWeb.Controllers
{
    public class EmpleadoController : Controller
    {
        private IListarEmpleados _listarEmpleados;
        private IObtenerEmpleado _obtenerEmpleado;
        private IActualizarEmpleado _actualizarEmpleado;
        private ICrearEmpleado _crearEmpleado;

        public EmpleadoController()
        {
            _listarEmpleados = new LogicaDeNegocio.Empleados.ListarEmpleados();
            _obtenerEmpleado = new LogicaDeNegocio.Empleados.ObtenerEmpleado();
            _actualizarEmpleado = new LogicaDeNegocio.Empleados.ActualizarEmpleado();
            _crearEmpleado = new LogicaDeNegocio.Empleados.CrearEmpleado();
        }

        // GET: Empleado/ListadoEmpleados
        public IActionResult ListadoEmpleados(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            List<Empleado> empleados;

            if (!string.IsNullOrEmpty(buscar))
            {
                empleados = _listarEmpleados.BuscarPorNombre(buscar);
            }
            else
            {
                empleados = _listarEmpleados.Obtener();
            }

            if (!string.IsNullOrEmpty(filtroEstado))
            {
                if (filtroEstado == "activo")
                {
                    empleados = empleados.FindAll(e => e.Estado == true);
                }
                else if (filtroEstado == "inactivo")
                {
                    empleados = empleados.FindAll(e => e.Estado == false);
                }
            }

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
    public async Task<IActionResult> CrearEmpleado(Empleado empleado, string ConfirmarContrasena)
    {
        try
        {
            // Remover validaciones de campos calculados
            ModelState.Remove("NombreRol");
            ModelState.Remove("EstadoTexto");
            ModelState.Remove("FechaContratacionFormateada");
            ModelState.Remove("MesesTrabajados");
            ModelState.Remove("AnosTrabajados");
            ModelState.Remove("EsEmpleadoAntiguo");
            ModelState.Remove("TarifaActual");
            ModelState.Remove("ContactoEmergencia");
            ModelState.Remove("Alergias");
            ModelState.Remove("Medicamentos");

            // ⭐ Validación de contraseña con requisitos robustos
            if (string.IsNullOrWhiteSpace(empleado.Contrasena))
            {
                ModelState.AddModelError("Contrasena", "La contraseña es requerida");
            }
            else
            {
                if (!PasswordValidator.EsContrasenaValida(empleado.Contrasena, out string mensajeError))
                {
                    ModelState.AddModelError("Contrasena", mensajeError);
                }
            }

            if (string.IsNullOrWhiteSpace(ConfirmarContrasena))
            {
                ModelState.AddModelError("ConfirmarContrasena", "Debes confirmar la contraseña");
            }
            else if (empleado.Contrasena != ConfirmarContrasena)
            {
                ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas no coinciden");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.ErrorMessage = "Por favor, corrige los errores del formulario";
                CargarRoles();
                return View(empleado);
            }

            // Establecer estado activo por defecto
            empleado.Estado = true;

            int resultado = await _crearEmpleado.Crear(empleado);

            if (resultado > 0)
            {
                TempData["Mensaje"] = "Empleado registrado correctamente";
                return RedirectToAction(nameof(ListadoEmpleados));
            }
            else
            {
                ModelState.AddModelError("", "No se pudo registrar el empleado en la base de datos");
                CargarRoles();
                return View(empleado);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Excepción capturada: {ex.Message}");
            ModelState.AddModelError("", $"Error al registrar el empleado: {ex.Message}");
            CargarRoles();
            return View(empleado);
        }
    }

    // GET: Empleado/EditarEmpleado/5
    public IActionResult EditarEmpleado(int id)
        {
            try
            {
                var empleado = _obtenerEmpleado.Obtener(id);

                if (empleado == null)
                {
                    TempData["Error"] = "Empleado no encontrado";
                    return RedirectToAction(nameof(ListadoEmpleados));
                }

                CargarRoles();
                return View(empleado);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el empleado: {ex.Message}";
                return RedirectToAction(nameof(ListadoEmpleados));
            }
        }

        // POST: Empleado/EditarEmpleado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarEmpleado(Empleado empleado, string ContrasenaActual, string ContrasenaNueva, string ConfirmarContrasenaNueva)
        {
            try
            {
                // Remover validación de campos calculados
                ModelState.Remove("NombreRol");
                ModelState.Remove("EstadoTexto");
                ModelState.Remove("FechaContratacionFormateada");
                ModelState.Remove("MesesTrabajados");
                ModelState.Remove("AnosTrabajados");
                ModelState.Remove("EsEmpleadoAntiguo");
                ModelState.Remove("Contrasena");
                ModelState.Remove("TarifaActual");
                ModelState.Remove("ContrasenaActual");
                ModelState.Remove("ContrasenaNueva");
                ModelState.Remove("ConfirmarContrasenaNueva");
                ModelState.Remove("ContactoEmergencia");
                ModelState.Remove("Alergias");
                ModelState.Remove("Medicamentos");

                // Validación de contraseña: Solo si algún campo de contraseña tiene valor
                bool intentaCambiarContrasena = !string.IsNullOrWhiteSpace(ContrasenaNueva) ||
                                                 !string.IsNullOrWhiteSpace(ConfirmarContrasenaNueva) ||
                                                 !string.IsNullOrWhiteSpace(ContrasenaActual);

                if (intentaCambiarContrasena)
                {
                    var empleadoActual = _obtenerEmpleado.Obtener(empleado.IdEmpleado);

                    if (empleadoActual == null)
                    {
                        ModelState.AddModelError("", "No se pudo cargar la información del empleado");
                        CargarRoles();
                        return View(empleado);
                    }

                    if (string.IsNullOrWhiteSpace(ContrasenaActual))
                    {
                        ModelState.AddModelError("ContrasenaActual", "Debes ingresar la contraseña actual");
                    }
                    else
                    {
                        if (empleadoActual.Contrasena != ContrasenaActual)
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
                        // Validación robusta de la nueva contraseña
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
                        CargarRoles();
                        return View(empleado);
                    }

                    empleado.Contrasena = ContrasenaNueva;
                }
                else
                {
                    empleado.Contrasena = null;
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.ErrorMessage = "Por favor, corrige los errores del formulario";
                    CargarRoles();
                    return View(empleado);
                }

                int resultado = _actualizarEmpleado.Actualizar(empleado);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = "Empleado actualizado correctamente";
                    return RedirectToAction(nameof(ListadoEmpleados));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar el empleado en la base de datos");
                    CargarRoles();
                    return View(empleado);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Excepción capturada: {ex.Message}");
                ModelState.AddModelError("", $"Error al actualizar el empleado: {ex.Message}");
                CargarRoles();
                return View(empleado);
            }
        }

        // POST: Empleado/CambiarEstado/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CambiarEstado(int id)
        {
            try
            {
                var empleado = _obtenerEmpleado.Obtener(id);

                if (empleado == null)
                {
                    TempData["Error"] = "Empleado no encontrado";
                    return RedirectToAction(nameof(ListadoEmpleados));
                }

                empleado.Estado = !empleado.Estado;

                int resultado = _actualizarEmpleado.Actualizar(empleado);

                if (resultado > 0)
                {
                    TempData["Mensaje"] = $"Empleado marcado como {(empleado.Estado ? "activo" : "inactivo")}";
                }
                else
                {
                    TempData["Error"] = "No se pudo actualizar el estado";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }

            return RedirectToAction(nameof(ListadoEmpleados));
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