using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;

namespace AromasWeb.Controllers
{
    public class ModuloController : Controller
    {
        private IListarModulos _listarModulos;
        private readonly CrearBitacora _crearBitacora;

        public ModuloController()
        {
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

        // GET: Modulo/ListadoModulos
        public IActionResult ListadoModulos(string buscar, string filtroEstado)
        {
            ViewBag.Buscar = buscar;
            ViewBag.FiltroEstado = filtroEstado;

            // Obtener módulos según los filtros
            List<Modulo> modulos;

            if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(filtroEstado))
            {
                // Buscar por nombre y filtrar por estado
                bool estado = filtroEstado == "activo";
                modulos = _listarModulos.BuscarPorNombre(buscar)
                    .Where(m => m.Estado == estado)
                    .ToList();
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre
                modulos = _listarModulos.BuscarPorNombre(buscar);
            }
            else if (!string.IsNullOrEmpty(filtroEstado))
            {
                // Solo filtrar por estado
                bool estado = filtroEstado == "activo";
                modulos = _listarModulos.BuscarPorEstado(estado);
            }
            else
            {
                // Obtener todos
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
        public IActionResult CrearModulo(Modulo modulo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de módulos"),
                    accion: Bitacora.Acciones.Crear,
                    tablaAfectada: "Modulo",
                    descripcion: $"Se registró el módulo: {modulo.Nombre}",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        modulo.Nombre,
                        modulo.Descripcion,
                        modulo.Estado
                    })
                );

                TempData["Mensaje"] = "Módulo registrado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoModulos));
            }

            return View(modulo);
        }

        // GET: Modulo/EditarModulo/5
        public IActionResult EditarModulo(int id)
        {
            var modulo = _listarModulos.ObtenerPorId(id);

            if (modulo == null)
            {
                TempData["Mensaje"] = "Módulo no encontrado";
                TempData["TipoMensaje"] = "error";
                return RedirectToAction(nameof(ListadoModulos));
            }

            return View(modulo);
        }

        // POST: Modulo/EditarModulo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarModulo(Modulo modulo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                var anterior = _listarModulos.ObtenerPorId(modulo.IdModulo);

                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de módulos"),
                    accion: Bitacora.Acciones.Actualizar,
                    tablaAfectada: "Modulo",
                    descripcion: $"Se actualizó el módulo: {modulo.Nombre} (ID: {modulo.IdModulo})",
                    datosAnteriores: anterior != null
                        ? JsonSerializer.Serialize(new { anterior.Nombre, anterior.Descripcion, anterior.Estado })
                        : null,
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        modulo.Nombre,
                        modulo.Descripcion,
                        modulo.Estado
                    })
                );

                TempData["Mensaje"] = "Módulo actualizado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoModulos));
            }

            return View(modulo);
        }

        // POST: Modulo/EliminarModulo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarModulo(int id)
        {
            // Aquí iría la lógica para eliminar de la base de datos
            // Validar que no tenga permisos asociados antes de eliminar
            var modulo = _listarModulos.ObtenerPorId(id);

            _crearBitacora.RegistrarAccion(
                idEmpleado: ObtenerIdEmpleadoSesion(),
                idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de módulos"),
                accion: Bitacora.Acciones.Eliminar,
                tablaAfectada: "Modulo",
                descripcion: $"Se eliminó el módulo: {modulo?.Nombre ?? id.ToString()} (ID: {id})",
                datosAnteriores: modulo != null
                    ? JsonSerializer.Serialize(new { modulo.Nombre, modulo.Descripcion, modulo.Estado })
                    : null
            );

            TempData["Mensaje"] = "Módulo eliminado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(ListadoModulos));
        }
    }
}