using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modulos;
using AromasWeb.LogicaDeNegocio.Bitacoras;
using System.Collections.Generic;
using System.Text.Json;
using System.Linq;
using System;

namespace AromasWeb.Controllers
{
    public class PromocionController : Controller
    {
        private IListarPromociones _listarPromociones;
        private IListarTiposPromociones _listarTiposPromociones;
        private IListarRecetas _listarRecetas;
        private ICrearPromociones _crearPromociones;
        private IEditarPromociones _editarPromociones;
        private IEliminarPromociones _eliminarPromociones;
        private readonly CrearBitacora _crearBitacora;

        public PromocionController()
        {
            _listarPromociones = new LogicaDeNegocio.Promociones.ListarPromociones();
            _listarTiposPromociones = new LogicaDeNegocio.TiposPromociones.ListarTiposPromociones();
            _listarRecetas = new LogicaDeNegocio.Recetas.ListarRecetas();
            _crearPromociones = new LogicaDeNegocio.Promocion.CrearPromociones();
            _editarPromociones = new LogicaDeNegocio.Promocion.EditarPromociones();
            _eliminarPromociones = new LogicaDeNegocio.Promocion.EliminarPromociones();
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

        // Limpia del ModelState los campos de navegación que no vienen del formulario
        private void LimpiarModelStateRecetas(Promocion promocion)
        {
            ModelState.Remove("NombreTipoPromocion");

            if (promocion.Recetas == null) return;

            for (int i = 0; i < promocion.Recetas.Count; i++)
            {
                ModelState.Remove($"Recetas[{i}].NombreReceta");
                ModelState.Remove($"Recetas[{i}].CategoriaReceta");
                ModelState.Remove($"Recetas[{i}].NombrePromocion");
                ModelState.Remove($"Recetas[{i}].PrecioOriginal");
                ModelState.Remove($"Recetas[{i}].PrecioPromocional");
                ModelState.Remove($"Recetas[{i}].PorcentajeDescuento");
            }
        }

        // Recalcula PrecioPromocional y PorcentajeDescuento consultando el precio real desde la BD
        private void RecalcularPreciosDesdeDB(Promocion promocion)
        {
            if (promocion.Recetas == null) return;

            var todasLasRecetas = _listarRecetas.ObtenerDisponibles();

            foreach (var receta in promocion.Recetas)
            {
                var recetaDB = todasLasRecetas.FirstOrDefault(r => r.IdReceta == receta.IdReceta);
                decimal precioOriginal = recetaDB?.PrecioVenta ?? 0m;

                receta.PrecioOriginal = precioOriginal;
                receta.PorcentajeDescuento = promocion.PorcentajeDescuento;
                receta.PrecioPromocional = precioOriginal > 0
                    ? precioOriginal * (1 - promocion.PorcentajeDescuento / 100)
                    : 0;
            }
        }

        // ============================================================
        // GET: Promocion/ListadoPromociones
        // ============================================================
        public IActionResult ListadoPromociones(string buscar, int? tipo, string vigencia, bool? estado)
        {
            try
            {
                ViewBag.Buscar = buscar;
                ViewBag.Tipo = tipo;
                ViewBag.Vigencia = vigencia;
                ViewBag.Estado = estado;
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();

                List<Promocion> promociones = _listarPromociones.Obtener();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    promociones = promociones
                        .Where(p =>
                            (!string.IsNullOrEmpty(p.Nombre) && p.Nombre.ToLower().Contains(buscar.ToLower())) ||
                            (!string.IsNullOrEmpty(p.Descripcion) && p.Descripcion.ToLower().Contains(buscar.ToLower())) ||
                            (!string.IsNullOrEmpty(p.NombreTipoPromocion) && p.NombreTipoPromocion.ToLower().Contains(buscar.ToLower()))
                        ).ToList();
                }

                if (tipo.HasValue)
                    promociones = promociones.Where(p => p.IdTipoPromocion == tipo.Value).ToList();

                if (!string.IsNullOrWhiteSpace(vigencia))
                    promociones = promociones
                        .Where(p => !string.IsNullOrEmpty(p.VigenciaTexto) &&
                                    p.VigenciaTexto.ToLower() == vigencia.ToLower()).ToList();

                if (estado.HasValue)
                    promociones = promociones.Where(p => p.Estado == estado.Value).ToList();

                var todas = _listarPromociones.Obtener();
                ViewBag.TotalPromociones = todas.Count;
                ViewBag.PromocionesVigentes = todas.Count(p => p.VigenciaTexto == "Vigente");
                ViewBag.PromedioDescuento = todas.Count > 0 ? todas.Average(p => p.PorcentajeDescuento) : 0;
                ViewBag.TotalRecetasEnPromocion = todas.Sum(p => p.CantidadRecetas);
                ViewBag.PromocionesPorVencer = todas.Count(p => p.EstaVigente && p.DiasRestantes <= 7 && p.DiasRestantes > 0);

                return View(promociones);
            }
            catch
            {
                TempData["Error"] = "Error al cargar la información de promociones";
                ViewBag.Buscar = buscar;
                ViewBag.Tipo = tipo;
                ViewBag.Vigencia = vigencia;
                ViewBag.Estado = estado;
                ViewBag.TodosTipos = new List<TipoPromocion>();
                ViewBag.TotalPromociones = 0;
                ViewBag.PromocionesVigentes = 0;
                ViewBag.PromocionesVencidas = 0;
                ViewBag.PromedioDescuento = 0;
                ViewBag.TotalRecetasEnPromocion = 0;
                ViewBag.PromocionesPorVencer = 0;
                return View(new List<Promocion>());
            }
        }

        // ============================================================
        // GET: Promocion/ObtenerRecetasPromocion?id=5
        // ============================================================
        [HttpGet]
        public IActionResult ObtenerRecetasPromocion(int id)
        {
            var promocion = _listarPromociones.ObtenerPorId(id);

            if (promocion == null)
                return NotFound();

            var recetas = (promocion.Recetas ?? new List<PromocionReceta>())
                .Select(r => new
                {
                    r.IdReceta,
                    r.NombreReceta,
                    r.CategoriaReceta,
                    r.PrecioOriginal,
                    r.PrecioPromocional,
                    Ahorro = r.PrecioOriginal - r.PrecioPromocional
                });

            return Json(recetas);
        }

        // ============================================================
        // GET: Promocion/CrearPromocion
        // ============================================================
        public IActionResult CrearPromocion()
        {
            try
            {
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar los datos: {ex.Message}";
                ViewBag.TodosTipos = new List<TipoPromocion>();
                ViewBag.RecetasDisponibles = new List<Receta>();
            }

            return View();
        }

        // ============================================================
        // POST: Promocion/CrearPromocion
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPromocion(Promocion promocion)
        {
            try
            {
                promocion.Recetas = (promocion.Recetas ?? new List<PromocionReceta>())
                    .Where(r => r != null && r.IdReceta > 0)
                    .ToList();

                if (promocion.FechaFin.Date < promocion.FechaInicio.Date)
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor que la fecha de inicio.");

                if (!promocion.Recetas.Any())
                    ModelState.AddModelError("Recetas", "Debes agregar al menos una receta a la promoción.");

                if (promocion.Recetas.GroupBy(r => r.IdReceta).Any(g => g.Count() > 1))
                    ModelState.AddModelError("Recetas", "No puedes agregar la misma receta más de una vez.");

                if (_listarPromociones.Obtener().Any(p =>
                        p.Nombre != null && promocion.Nombre != null &&
                        p.Nombre.Trim().ToLower() == promocion.Nombre.Trim().ToLower()))
                    ModelState.AddModelError("Nombre", "Ya existe una promoción con ese nombre.");

                LimpiarModelStateRecetas(promocion);

                // Calcular precios desde la BD
                RecalcularPreciosDesdeDB(promocion);

                var errores = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = string.Join(" ", errores);
                    ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                    ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                    return View(promocion);
                }

                _crearPromociones.Ejecutar(promocion);

                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de promociones"),
                    accion: Bitacora.Acciones.Crear,
                    tablaAfectada: "Promocion",
                    descripcion: $"Se creó la promoción: {promocion.Nombre}",
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        promocion.Nombre,
                        promocion.Descripcion,
                        promocion.IdTipoPromocion,
                        promocion.PorcentajeDescuento,
                        FechaInicio = promocion.FechaInicio.ToString("yyyy-MM-dd"),
                        FechaFin = promocion.FechaFin.ToString("yyyy-MM-dd"),
                        promocion.Estado,
                        CantidadRecetas = promocion.Recetas.Count
                    })
                );

                TempData["Mensaje"] = "Promoción registrada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al registrar la promoción: {ex.Message}";
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                return View(promocion);
            }
        }

        // ============================================================
        // GET: Promocion/EditarPromocion/5
        // ============================================================
        public IActionResult EditarPromocion(int id)
        {
            try
            {
                var promocion = _listarPromociones.ObtenerPorId(id);

                if (promocion == null)
                {
                    TempData["Error"] = "Promoción no encontrada";
                    return RedirectToAction(nameof(ListadoPromociones));
                }

                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                return View(promocion);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar la promoción: {ex.Message}";
                return RedirectToAction(nameof(ListadoPromociones));
            }
        }

        // ============================================================
        // POST: Promocion/EditarPromocion
        // ============================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPromocion(Promocion promocion)
        {
            try
            {
                promocion.Recetas = (promocion.Recetas ?? new List<PromocionReceta>())
                    .Where(r => r != null && r.IdReceta > 0)
                    .ToList();

                if (promocion.FechaFin.Date < promocion.FechaInicio.Date)
                    ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor que la fecha de inicio.");

                if (!promocion.Recetas.Any())
                    ModelState.AddModelError("Recetas", "Debes agregar al menos una receta a la promoción.");

                if (promocion.Recetas.GroupBy(r => r.IdReceta).Any(g => g.Count() > 1))
                    ModelState.AddModelError("Recetas", "No puedes agregar la misma receta más de una vez.");

                if (_listarPromociones.Obtener().Any(p =>
                        p.IdPromocion != promocion.IdPromocion &&
                        p.Nombre != null && promocion.Nombre != null &&
                        p.Nombre.Trim().ToLower() == promocion.Nombre.Trim().ToLower()))
                    ModelState.AddModelError("Nombre", "Ya existe otra promoción con ese nombre.");

                LimpiarModelStateRecetas(promocion);

                // Capturar datos anteriores ANTES de actualizar (para bitácora)
                var anterior = _listarPromociones.ObtenerPorId(promocion.IdPromocion);

                // Calcular precios desde la BD — independiente del form
                RecalcularPreciosDesdeDB(promocion);

                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Complete todos los campos obligatorios antes de guardar";
                    ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                    ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                    return View(promocion);
                }

                _editarPromociones.Ejecutar(promocion);

                _crearBitacora.RegistrarAccion(
                    idEmpleado: ObtenerIdEmpleadoSesion(),
                    idModulo: ObtenerModulo.ObtenerIdPorNombre("Gestión de promociones"),
                    accion: Bitacora.Acciones.Actualizar,
                    tablaAfectada: "Promocion",
                    descripcion: $"Se actualizó la promoción: {promocion.Nombre} (ID: {promocion.IdPromocion})",
                    datosAnteriores: anterior != null
                        ? JsonSerializer.Serialize(new
                        {
                            anterior.Nombre,
                            anterior.Descripcion,
                            anterior.IdTipoPromocion,
                            anterior.PorcentajeDescuento,
                            FechaInicio = anterior.FechaInicio.ToString("yyyy-MM-dd"),
                            FechaFin = anterior.FechaFin.ToString("yyyy-MM-dd"),
                            anterior.Estado,
                            CantidadRecetas = anterior.CantidadRecetas
                        })
                        : null,
                    datosNuevos: JsonSerializer.Serialize(new
                    {
                        promocion.Nombre,
                        promocion.Descripcion,
                        promocion.IdTipoPromocion,
                        promocion.PorcentajeDescuento,
                        FechaInicio = promocion.FechaInicio.ToString("yyyy-MM-dd"),
                        FechaFin = promocion.FechaFin.ToString("yyyy-MM-dd"),
                        promocion.Estado,
                        CantidadRecetas = promocion.Recetas.Count
                    })
                );

                TempData["Mensaje"] = "Promoción actualizada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar la promoción: {ex.Message}";
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                return View(promocion);
            }
        }
    }

}

        
