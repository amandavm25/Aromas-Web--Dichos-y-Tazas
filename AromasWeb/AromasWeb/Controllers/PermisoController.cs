using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AromasWeb.Controllers
{
    public class PermisoController : Controller
    {
        private IListarPermisos _listarPermisos;
        private ICrearPermiso _crearPermiso;
        private IActualizarPermiso _actualizarPermiso;
        private IEliminarPermiso _eliminarPermiso;
        private IListarModulos _listarModulos;
        private IListarRoles _listarRoles;
        private readonly CrearBitacora _crearBitacora;

        public PermisoController()
        {
            _listarPermisos = new LogicaDeNegocio.Permisos.ListarPermisos();
            _crearPermiso = new LogicaDeNegocio.Permisos.CrearPermiso();
            _actualizarPermiso = new LogicaDeNegocio.Permisos.ActualizarPermiso();
            _eliminarPermiso = new LogicaDeNegocio.Permisos.EliminarPermiso();
            _listarModulos = new LogicaDeNegocio.Modulos.ListarModulos();
            _listarRoles = new LogicaDeNegocio.Roles.ListarRoles();
            _crearBitacora = new CrearBitacora();
        }

        private int ObtenerIdEmpleadoSesion()
        {
            int? id = HttpContext.Session.GetInt32("IdEmpleado");
            return (id.HasValue && id.Value > 0) ? id.Value : 1;
        }

        // ============================================================
        // LISTADO
        // ============================================================
        public IActionResult ListadoPermisos(string buscar, int? filtroModulo, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;
            ViewBag.FiltroEstado = filtroEstado;

            CargarModulos();

            var permisos = _listarPermisos.Obtener();

            // Filtro por módulo
            if (filtroModulo.HasValue)
                permisos = permisos.Where(p => p.IdModulo == filtroModulo.Value).ToList();

            // Filtro por estado
            if (!string.IsNullOrEmpty(filtroEstado))
            {
                bool activo = filtroEstado == "true";
                permisos = permisos.Where(p => p.Estado == activo).ToList();
            }

            // Filtro por nombre
            if (!string.IsNullOrEmpty(buscar))
                permisos = permisos.Where(p => p.Nombre.ToLower().Contains(buscar.ToLower())).ToList();

            return View(permisos);
        }

        // ============================================================
        // CREAR
        // ============================================================
        public IActionResult CrearPermiso()
        {
            CargarModulosParaSelect();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPermiso(Permiso permiso)
        {
            // Si el modelo no es válido, mostrar qué falta
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor completa todos los campos requeridos";
                CargarModulosParaSelect(permiso.IdModulo);
                return View(permiso);
            }

            try
            {
                int resultado = _crearPermiso.Crear(permiso);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                        accion: Bitacora.Acciones.Crear,
                        tablaAfectada: "Permiso",
                        descripcion: $"Se registró el permiso: {permiso.Nombre} (módulo ID: {permiso.IdModulo})",
                        datosNuevos: JsonSerializer.Serialize(new { permiso.Nombre, permiso.IdModulo, permiso.Descripcion, permiso.Estado })
                    );

                    TempData["Mensaje"] = "Permiso registrado correctamente";
                    return RedirectToAction(nameof(ListadoPermisos));
                }

                // SaveChanges devolvió 0 sin lanzar excepción
                TempData["Error"] = "No se pudo registrar el permiso";
                CargarModulosParaSelect(permiso.IdModulo);
                return View(permiso);
            }
            catch (Exception ex)
            {
                // Redirigir al GET para que TempData se muestre de forma segura
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(CrearPermiso));
            }
        }

        // ============================================================
        // EDITAR
        // ============================================================
        public IActionResult EditarPermiso(int id)
        {
            var permiso = _listarPermisos.ObtenerPorId(id);
            if (permiso == null)
            {
                TempData["Error"] = "Permiso no encontrado";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect(permiso.IdModulo);
            return View(permiso);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPermiso(Permiso permiso)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Por favor completa todos los campos requeridos";
                CargarModulosParaSelect(permiso.IdModulo);
                return View(permiso);
            }

            try
            {
                var anterior = _listarPermisos.ObtenerPorId(permiso.IdPermiso);
                int resultado = _actualizarPermiso.Actualizar(permiso);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                        accion: Bitacora.Acciones.Actualizar,
                        tablaAfectada: "Permiso",
                        descripcion: $"Se actualizó el permiso: {permiso.Nombre} (ID: {permiso.IdPermiso})",
                        datosAnteriores: anterior != null
                            ? JsonSerializer.Serialize(new { anterior.Nombre, anterior.IdModulo, anterior.Descripcion, anterior.Estado })
                            : null,
                        datosNuevos: JsonSerializer.Serialize(new { permiso.Nombre, permiso.IdModulo, permiso.Descripcion, permiso.Estado })
                    );

                    TempData["Mensaje"] = "Permiso actualizado correctamente";
                    return RedirectToAction(nameof(ListadoPermisos));
                }

                TempData["Error"] = "No se encontró el permiso a actualizar";
                CargarModulosParaSelect(permiso.IdModulo);
                return View(permiso);
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
                return RedirectToAction(nameof(EditarPermiso), new { id = permiso.IdPermiso });
            }
        }

        // ============================================================
        // ELIMINAR
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPermiso(int id)
        {
            try
            {
                var permiso = _listarPermisos.ObtenerPorId(id);
                int resultado = _eliminarPermiso.Eliminar(id);

                if (resultado > 0)
                {
                    _crearBitacora.RegistrarAccion(
                        idEmpleado: ObtenerIdEmpleadoSesion(),
                        idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                        accion: Bitacora.Acciones.Eliminar,
                        tablaAfectada: "Permiso",
                        descripcion: $"Se eliminó el permiso: {permiso?.Nombre ?? id.ToString()} (ID: {id})",
                        datosAnteriores: permiso != null
                            ? JsonSerializer.Serialize(new { permiso.Nombre, permiso.IdModulo })
                            : null
                    );

                    TempData["Mensaje"] = "Permiso eliminado correctamente";
                }
                else
                {
                    TempData["Error"] = "No se encontró el permiso a eliminar";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(ListadoPermisos));
        }

        // ============================================================
        // ASIGNAR PERMISOS A ROL
        // ============================================================
        public IActionResult AsignarPermisos(int idRol)
        {
            try
            {
                var rol = _listarRoles.ObtenerPorId(idRol);
                if (rol == null)
                {
                    TempData["Error"] = "Rol no encontrado";
                    return RedirectToAction("ListadoRoles", "Rol");
                }

                var modulos = _listarModulos.BuscarPorEstado(true);
                var permisos = _listarPermisos.Obtener().Where(p => p.Estado).ToList();
                var permisosAsignados = _listarPermisos.ObtenerPermisosDeRol(idRol);

                ViewBag.Rol = rol;
                ViewBag.Modulos = modulos;
                ViewBag.Permisos = permisos;
                ViewBag.PermisosAsignados = permisosAsignados;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ASIGNAR PERMISOS ERROR] {ex.Message} | INNER: {ex.InnerException?.Message}");
                TempData["Error"] = "Error al cargar la pantalla de permisos";
                return RedirectToAction("ListadoRoles", "Rol");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPermisos(int idRol, List<int> permisosSeleccionados)
        {
            try
            {
                if (permisosSeleccionados == null)
                    permisosSeleccionados = new List<int>();

                bool exito = _listarPermisos.AsignarPermisosARol(idRol, permisosSeleccionados);

                if (exito)
                {
                    try
                    {
                        _crearBitacora.RegistrarAccion(
                            idEmpleado: ObtenerIdEmpleadoSesion(),
                            idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                            accion: Bitacora.Acciones.AsignarPermisos,
                            tablaAfectada: "RolPermiso",
                            descripcion: $"Se asignaron {permisosSeleccionados.Count} permiso(s) al rol ID: {idRol}",
                            datosNuevos: JsonSerializer.Serialize(new { IdRol = idRol, Permisos = permisosSeleccionados })
                        );
                    }
                    catch (Exception exBitacora)
                    {
                        // La bitácora falla pero la operación principal fue exitosa
                        Console.WriteLine($"[BITACORA ERROR] {exBitacora.Message}");
                    }

                    TempData["Mensaje"] = "Permisos asignados correctamente";
                }
                else
                {
                    TempData["Error"] = "Error al asignar permisos";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[GUARDAR PERMISOS ERROR] {ex.Message} | INNER: {ex.InnerException?.Message}");
                TempData["Error"] = "Error inesperado al asignar permisos";
            }

            return RedirectToAction("ListadoRoles", "Rol");
        }

        // ============================================================
        // AUXILIARES
        // ============================================================
        private void CargarModulos()
        {
            ViewBag.Modulos = _listarModulos.Obtener();
        }

        private void CargarModulosParaSelect(int? idModuloSeleccionado = null)
        {
            var modulos = _listarModulos.BuscarPorEstado(true);
            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre", idModuloSeleccionado);
        }
    }
}