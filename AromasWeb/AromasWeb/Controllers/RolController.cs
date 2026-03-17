using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.AccesoADatos.Roles;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AromasWeb.Controllers
{
    public class RolController : Controller
    {
        private IListarRoles _listarRoles;
        private ICrearRol _crearRol;
        private IActualizarRol _actualizarRol;
        private IObtenerRol _obtenerRol;
        private readonly CrearBitacora _crearBitacora;

        public RolController()
        {
            _listarRoles = new LogicaDeNegocio.Roles.ListarRoles();
            _crearRol = new LogicaDeNegocio.Roles.CrearRol();
            _actualizarRol = new LogicaDeNegocio.Roles.ActualizarRol();
            _obtenerRol = new LogicaDeNegocio.Roles.ObtenerRol();
            _crearBitacora = new CrearBitacora();
        }

        // Helper de sesión
        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;
            return 1;
        }

        // GET: Rol/ListadoRoles
        public IActionResult ListadoRoles(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            List<Rol> roles;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                bool estado = filtroEstado == "activo";
                roles = _listarRoles.BuscarPorNombre(buscar)
                    .Where(r => r.Estado == estado)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                roles = _listarRoles.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                bool estado = filtroEstado == "activo";
                roles = _listarRoles.BuscarPorEstado(estado);
            }
            else
            {
                roles = _listarRoles.Obtener();
            }

            return View(roles);
        }

        // GET: Rol/CrearRol
        public IActionResult CrearRol()
        {
            return View();
        }

        // POST: Rol/CrearRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRol(Rol rol)
        {
            try
            {
                // Remover propiedades calculadas del ModelState
                ModelState.Remove("EstadoTexto");

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(rol);
                }

                // Establecer estado por defecto si no viene
                rol.Estado = true;

                int resultado = await _crearRol.Crear(rol);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: AccesoADatos.Modulos.ObtenerModulo.ObtenerIdPorNombre("Gestión de roles"),
                        accion: Bitacora.Acciones.Crear,
                        tablaAfectada: "Rol",
                        descripcion: $"Se creó el rol: {rol.Nombre}",
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            rol.Nombre,
                            rol.Descripcion,
                            rol.Estado
                        })
                    );

                    TempData["Mensaje"] = "Rol registrado correctamente";
                    return RedirectToAction(nameof(ListadoRoles));
                }
                else
                {
                    TempData["Error"] = "No se pudo registrar el rol en la base de datos";
                    return View(rol);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearRol: {ex.Message}");

                if (ex.Message.Contains("Ya existe un rol"))
                {
                    TempData["Error"] = "Ya existe un rol con ese nombre";
                }
                else
                {
                    TempData["Error"] = $"Error al registrar el rol: {ex.Message}";
                }

                return View(rol);
            }
        }

        // GET: Rol/EditarRol/5
        public IActionResult EditarRol(int id)
        {
            try
            {
                var rol = _obtenerRol.Obtener(id);

                if (rol == null)
                {
                    TempData["Error"] = "Rol no encontrado";
                    return RedirectToAction(nameof(ListadoRoles));
                }

                return View(rol);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el rol: {ex.Message}";
                return RedirectToAction(nameof(ListadoRoles));
            }
        }

        // POST: Rol/EditarRol
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarRol(Rol rol)
        {
            try
            {
                // Remover propiedades calculadas del ModelState
                ModelState.Remove("EstadoTexto");

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(rol);
                }

                // Capturar datos anteriores ANTES de actualizar
                var anterior = _obtenerRol.Obtener(rol.IdRol);

                int resultado = _actualizarRol.Actualizar(rol);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: AccesoADatos.Modulos.ObtenerModulo.ObtenerIdPorNombre("Gestión de roles"),
                        accion: Bitacora.Acciones.Actualizar,
                        tablaAfectada: "Rol",
                        descripcion: $"Se actualizó el rol: {rol.Nombre} (ID: {rol.IdRol})",
                        datosAnteriores: anterior != null
                            ? JsonSerializer.Serialize(new
                            {
                                anterior.Nombre,
                                anterior.Descripcion,
                                anterior.Estado
                            })
                            : null,
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            rol.Nombre,
                            rol.Descripcion,
                            rol.Estado
                        })
                    );

                    TempData["Mensaje"] = "Rol actualizado correctamente";
                    return RedirectToAction(nameof(ListadoRoles));
                }
                else
                {
                    TempData["Error"] = "No se pudo actualizar el rol en la base de datos";
                    return View(rol);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en EditarRol: {ex.Message}");

                if (ex.Message.Contains("Ya existe otro rol"))
                {
                    TempData["Error"] = "Ya existe otro rol con ese nombre";
                }
                else
                {
                    TempData["Error"] = $"Error al actualizar el rol: {ex.Message}";
                }

                return View(rol);
            }
        }
    }
}