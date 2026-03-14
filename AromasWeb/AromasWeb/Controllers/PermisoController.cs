using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.Abstracciones.Logica.Modulo;
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
        private IListarModulos _listarModulos;
        private readonly CrearBitacora _crearBitacora;

        public PermisoController()
        {
            _listarPermisos = new LogicaDeNegocio.Permisos.ListarPermisos();
            _listarModulos = new LogicaDeNegocio.Modulos.ListarModulos();
            _crearBitacora = new CrearBitacora();
        }

        private int ObtenerIdEmpleadoSesion()
        {
            int? idSesion = HttpContext.Session.GetInt32("IdEmpleado");
            if (idSesion.HasValue && idSesion.Value > 0)
                return idSesion.Value;
            return 1;
        }

        // GET: Permiso/ListadoPermisos
        public IActionResult ListadoPermisos(string buscar, int? filtroModulo)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroModulo = filtroModulo;

            // Cargar módulos para filtro
            CargarModulos();

            // Obtener permisos
            List<Permiso> permisos;

            if (!string.IsNullOrEmpty(buscar) && filtroModulo.HasValue)
            {
                permisos = _listarPermisos.ObtenerPorModulo(filtroModulo.Value)
                    .Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()))
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                permisos = _listarPermisos.Obtener()
                    .Where(p => p.Nombre.ToLower().Contains(buscar.ToLower()))
                    .ToList();
            }
            else if (filtroModulo.HasValue)
            {
                permisos = _listarPermisos.ObtenerPorModulo(filtroModulo.Value);
            }
            else
            {
                permisos = _listarPermisos.Obtener();
            }

            return View(permisos);
        }

        // GET: Permiso/CrearPermiso
        public IActionResult CrearPermiso()
        {
            CargarModulosParaSelect();
            return View();
        }

        // POST: Permiso/CrearPermiso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPermiso(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                    accion: Bitacora.Acciones.Crear,
                    tablaAfectada: "Permiso",
                    descripcion: $"Se registró el permiso: {permiso.Nombre} (módulo ID: {permiso.IdModulo})",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        permiso.Nombre,
                        permiso.IdModulo
                    })
                );

                TempData["Mensaje"] = "Permiso registrado correctamente";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect();
            return View(permiso);
        }

        // GET: Permiso/EditarPermiso/5
        public IActionResult EditarPermiso(int id)
        {
            var permiso = _listarPermisos.ObtenerPorId(id);

            if (permiso == null)
            {
                TempData["Mensaje"] = "Permiso no encontrado";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect(permiso.IdModulo);
            return View(permiso);
        }

        // POST: Permiso/EditarPermiso
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPermiso(Permiso permiso)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                var anterior = _listarPermisos.ObtenerPorId(permiso.IdPermiso);

                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                    accion: Bitacora.Acciones.Actualizar,
                    tablaAfectada: "Permiso",
                    descripcion: $"Se actualizó el permiso: {permiso.Nombre} (ID: {permiso.IdPermiso})",
                    datosAnteriores: anterior != null
                        ? JsonSerializer.Serialize(new { anterior.Nombre, anterior.IdModulo })
                        : null,
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        permiso.Nombre,
                        permiso.IdModulo
                    })
                );

                TempData["Mensaje"] = "Permiso actualizado correctamente";
                return RedirectToAction(nameof(ListadoPermisos));
            }

            CargarModulosParaSelect(permiso.IdModulo);
            return View(permiso);
        }

        // POST: Permiso/EliminarPermiso/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPermiso(int id)
        {
            // Aquí iría la lógica para eliminar de la base de datos
            var permiso = _listarPermisos.ObtenerPorId(id);

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
            return RedirectToAction(nameof(ListadoPermisos));
        }

        // GET: Permiso/AsignarPermisos/5
        public IActionResult AsignarPermisos(int id)
        {
            // Aquí obtendrías el rol desde la BD (por ahora ejemplo)
            var rol = new Rol
            {
                IdRol = id,
                Nombre = "Administrador",
                Descripcion = "Acceso completo al sistema"
            };

            // Obtener módulos y permisos desde la BD
            var modulos = _listarModulos.Obtener();
            var permisos = _listarPermisos.Obtener();
            var permisosAsignados = _listarPermisos.ObtenerPermisosDeRol(id);

            ViewBag.Rol = rol;
            ViewBag.Modulos = modulos;
            ViewBag.Permisos = permisos;
            ViewBag.PermisosAsignados = permisosAsignados;

            return View();
        }

        // POST: Permiso/GuardarPermisos
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GuardarPermisos(int idRol, List<int> permisosSeleccionados)
        {
            if (permisosSeleccionados == null)
            {
                permisosSeleccionados = new List<int>();
            }

            bool exito = _listarPermisos.AsignarPermisosARol(idRol, permisosSeleccionados);

            if (exito)
            {
                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de permisos"),
                    accion: Bitacora.Acciones.AsignarPermisos,
                    tablaAfectada: "RolPermiso",
                    descripcion: $"Se asignaron {permisosSeleccionados.Count} permiso(s) al rol ID: {idRol}",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        IdRol = idRol,
                        Permisos = permisosSeleccionados
                    })
                );

                TempData["Mensaje"] = "Permisos asignados correctamente";
            }
            else
            {
                TempData["Mensaje"] = "Error al asignar permisos";
                TempData["TipoMensaje"] = "error";
            }

            return RedirectToAction("ListadoRoles", "Rol");
        }

        // Métodos auxiliares
        private void CargarModulos()
        {
            var modulos = _listarModulos.Obtener();
            ViewBag.Modulos = modulos;
        }

        private void CargarModulosParaSelect(int? idModuloSeleccionado = null)
        {
            var modulos = _listarModulos.BuscarPorEstado(true); // Solo activos
            ViewBag.Modulos = new SelectList(modulos, "IdModulo", "Nombre", idModuloSeleccionado);
        }
    }
}