using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace AromasWeb.Controllers
{
    public class ModuloController : Controller
    {
        private IListarModulos _listarModulos;
        private ICrearModulo _crearModulo;
        private IActualizarModulo _actualizarModulo;
        private IObtenerModulo _obtenerModulo;
        private readonly CrearBitacora _crearBitacora;

        public ModuloController()
        {
            _listarModulos = new LogicaDeNegocio.Modulos.ListarModulos();
            _crearModulo = new LogicaDeNegocio.Modulos.CrearModulo();
            _actualizarModulo = new LogicaDeNegocio.Modulos.ActualizarModulo();
            _obtenerModulo = new LogicaDeNegocio.Modulos.ObtenerModulo();
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

        // GET: Modulo/ListadoModulos
        public IActionResult ListadoModulos(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            List<Modulo> modulos;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                bool estado = filtroEstado == "activo";
                modulos = _listarModulos.BuscarPorNombre(buscar)
                    .Where(m => m.Estado == estado)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                modulos = _listarModulos.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                bool estado = filtroEstado == "activo";
                modulos = _listarModulos.BuscarPorEstado(estado);
            }
            else
            {
                modulos = _listarModulos.Obtener();
            }

            return View(modulos);
        }

        // GET: Modulo/CrearModulo
        public IActionResult CrearModulo()
        {
            return View();
        }

        // POST: Modulo/CrearModulo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearModulo(Modulo modulo)
        {
            try
            {
                // Remover propiedades calculadas del ModelState
                ModelState.Remove("EstadoTexto");

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(modulo);
                }

                // Establecer estado por defecto si no viene
                modulo.Estado = true;

                int resultado = await _crearModulo.Crear(modulo);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de módulos"),
                        accion: Bitacora.Acciones.Crear,
                        tablaAfectada: "Modulo",
                        descripcion: $"Se creó el módulo: {modulo.Nombre}",
                        datosNuevos: JsonSerializer.Serialize(new
                        {
                            modulo.Nombre,
                            modulo.Descripcion,
                            modulo.Estado
                        })
                    );

                    TempData["Mensaje"] = "Módulo registrado correctamente";
                    return RedirectToAction(nameof(ListadoModulos));
                }
                else
                {
                    TempData["Error"] = "No se pudo registrar el módulo en la base de datos";
                    return View(modulo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en CrearModulo: {ex.Message}");

                if (ex.Message.Contains("Ya existe un módulo"))
                {
                    TempData["Error"] = "Ya existe un módulo con ese nombre";
                }
                else
                {
                    TempData["Error"] = $"Error al registrar el módulo: {ex.Message}";
                }

                return View(modulo);
            }
        }

        // GET: Modulo/EditarModulo/5
        public IActionResult EditarModulo(int id)
        {
            try
            {
                var modulo = _obtenerModulo.Obtener(id);

                if (modulo == null)
                {
                    TempData["Error"] = "Módulo no encontrado";
                    return RedirectToAction(nameof(ListadoModulos));
                }

                return View(modulo);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar el módulo: {ex.Message}";
                return RedirectToAction(nameof(ListadoModulos));
            }
        }

        // POST: Modulo/EditarModulo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarModulo(Modulo modulo)
        {
            try
            {
                // Remover propiedades calculadas del ModelState
                ModelState.Remove("EstadoTexto");

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Por favor, corrige los errores del formulario";
                    return View(modulo);
                }

                // Capturar datos anteriores ANTES de actualizar
                var anterior = _obtenerModulo.Obtener(modulo.IdModulo);

                int resultado = _actualizarModulo.Actualizar(modulo);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de módulos"),
                        accion: Bitacora.Acciones.Actualizar,
                        tablaAfectada: "Modulo",
                        descripcion: $"Se actualizó el módulo: {modulo.Nombre} (ID: {modulo.IdModulo})",
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
                            modulo.Nombre,
                            modulo.Descripcion,
                            modulo.Estado
                        })
                    );

                    TempData["Mensaje"] = "Módulo actualizado correctamente";
                    return RedirectToAction(nameof(ListadoModulos));
                }
                else
                {
                    TempData["Error"] = "No se pudo actualizar el módulo en la base de datos";
                    return View(modulo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error en EditarModulo: {ex.Message}");

                if (ex.Message.Contains("Ya existe otro módulo"))
                {
                    TempData["Error"] = "Ya existe otro módulo con ese nombre";
                }
                else
                {
                    TempData["Error"] = $"Error al actualizar el módulo: {ex.Message}";
                }

                return View(modulo);
            }
        }
    }
}