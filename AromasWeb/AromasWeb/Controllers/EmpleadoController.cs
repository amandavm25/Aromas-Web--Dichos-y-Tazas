using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Empleado;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Threading.Tasks;

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
                // ⭐ DIAGNÓSTICO
                if (!ModelState.IsValid)
                {
                    foreach (var modelState in ModelState.Values)
                    {
                        foreach (var error in modelState.Errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error de validación: {error.ErrorMessage}");
                        }
                    }
                }

                // Remover validación de campos calculados
                ModelState.Remove("NombreRol");
                ModelState.Remove("EstadoTexto");
                ModelState.Remove("FechaContratacionFormateada");
                ModelState.Remove("MesesTrabajados");
                ModelState.Remove("AnosTrabajados");
                ModelState.Remove("EsEmpleadoAntiguo");
                ModelState.Remove("TarifaActual");

                // ⭐ VALIDACIÓN MANUAL DE CONTRASEÑA 
                if (string.IsNullOrWhiteSpace(empleado.Contrasena))
                {
                    ModelState.AddModelError("Contrasena", "La contraseña es requerida");
                }
                else if (empleado.Contrasena.Length < 8)
                {
                    ModelState.AddModelError("Contrasena", "La contraseña debe tener al menos 8 caracteres");
                }

                // Validar que las contraseñas coincidan
                if (!string.IsNullOrEmpty(empleado.Contrasena) && empleado.Contrasena != ConfirmarContrasena)
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Las contraseñas no coinciden");
                }

                if (string.IsNullOrWhiteSpace(ConfirmarContrasena))
                {
                    ModelState.AddModelError("ConfirmarContrasena", "Debes confirmar la contraseña");
                }

                if (!ModelState.IsValid)
                {
                    // ⭐ MOSTRAR ERRORES EN LA VISTA
                    ViewBag.ErrorMessage = "Por favor, corrige los errores del formulario";
                    CargarRoles();
                    return View(empleado);
                }

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

                // ⭐ VALIDACIÓN DE CONTRASEÑA: Solo si algún campo de contraseña tiene valor
                bool intentaCambiarContrasena = !string.IsNullOrWhiteSpace(ContrasenaNueva) ||
                                                 !string.IsNullOrWhiteSpace(ConfirmarContrasenaNueva) ||
                                                 !string.IsNullOrWhiteSpace(ContrasenaActual);

                if (intentaCambiarContrasena)
                {
                    // Si está intentando cambiar contraseña, validar todos los campos
                    if (string.IsNullOrWhiteSpace(ContrasenaActual))
                    {
                        ModelState.AddModelError("ContrasenaActual", "Debes ingresar la contraseña actual");
                    }

                    if (string.IsNullOrWhiteSpace(ContrasenaNueva))
                    {
                        ModelState.AddModelError("ContrasenaNueva", "Debes ingresar la nueva contraseña");
                    }
                    else if (ContrasenaNueva.Length < 8)
                    {
                        ModelState.AddModelError("ContrasenaNueva", "La nueva contraseña debe tener al menos 8 caracteres");
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

                    // Asignar la nueva contraseña si todo está bien
                    empleado.Contrasena = ContrasenaNueva;
                }
                else
                {
                    // No está cambiando contraseña
                    empleado.Contrasena = null;
                }

                // ⭐ AHORA SÍ validamos el ModelState
                if (!ModelState.IsValid)
                {
                    // DIAGNÓSTICO: Ver qué validaciones están fallando
                    System.Diagnostics.Debug.WriteLine("=== ERRORES DE VALIDACIÓN ===");
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        if (errors.Count > 0)
                        {
                            foreach (var error in errors)
                            {
                                System.Diagnostics.Debug.WriteLine($"Campo: {key} - Error: {error.ErrorMessage}");
                            }
                        }
                    }

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