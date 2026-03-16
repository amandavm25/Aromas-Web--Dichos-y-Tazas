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

        // Filtro por nombre
        if (!string.IsNullOrWhiteSpace(buscar))
        {
            promociones = promociones
                .Where(p =>
                    (!string.IsNullOrEmpty(p.Nombre) && p.Nombre.ToLower().Contains(buscar.ToLower())) ||
                    (!string.IsNullOrEmpty(p.Descripcion) && p.Descripcion.ToLower().Contains(buscar.ToLower())) ||
                    (!string.IsNullOrEmpty(p.NombreTipoPromocion) && p.NombreTipoPromocion.ToLower().Contains(buscar.ToLower()))
                )
                .ToList();
        }

        // Filtro por tipo
        if (tipo.HasValue)
        {
            promociones = promociones
                .Where(p => p.IdTipoPromocion == tipo.Value)
                .ToList();
        }

        // Filtro por vigencia
        if (!string.IsNullOrWhiteSpace(vigencia))
        {
            promociones = promociones
                .Where(p => !string.IsNullOrEmpty(p.VigenciaTexto) &&
                            p.VigenciaTexto.ToLower() == vigencia.ToLower())
                .ToList();
        }

        // Filtro por estado
        if (estado.HasValue)
        {
            promociones = promociones
                .Where(p => p.Estado == estado.Value)
                .ToList();
        }

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

        // GET: Promocion/ObtenerRecetasPromocion?id=5
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

        // GET: Promocion/CrearPromocion
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

        // POST: Promocion/CrearPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPromocion(Promocion promocion)

        {
            promocion.Recetas = promocion.Recetas ?? new List<PromocionReceta>();

            // limpiar recetas vacías
            promocion.Recetas = promocion.Recetas
                .Where(r => r != null && r.IdReceta > 0)
                .ToList();

            if (promocion.FechaFin.Date < promocion.FechaInicio.Date)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor que la fecha de inicio.");
            }

            if (!promocion.Recetas.Any())
            {
                ModelState.AddModelError("Recetas", "Debes agregar al menos una receta a la promoción.");
            }

            bool recetasDuplicadas = promocion.Recetas
                .GroupBy(r => r.IdReceta)
                .Any(g => g.Count() > 1);

            if (recetasDuplicadas)
            {
                ModelState.AddModelError("Recetas", "No puedes agregar la misma receta más de una vez.");
            }

            var promocionesExistentes = _listarPromociones.Obtener();

            bool nombreDuplicado = promocionesExistentes.Any(p =>
                p.Nombre != null &&
                promocion.Nombre != null &&
                p.Nombre.Trim().ToLower() == promocion.Nombre.Trim().ToLower());

            if (nombreDuplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe una promoción con ese nombre.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos obligatorios antes de guardar";
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                return View(promocion);
            }

            _crearPromociones.Ejecutar(promocion);

            TempData["Mensaje"] = "Promoción registrada correctamente";
            return RedirectToAction(nameof(ListadoPromociones));


        }

        // GET: Promocion/EditarPromocion/5
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

        // POST: Promocion/EditarPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPromocion(Promocion promocion)
        {
            promocion.Recetas = promocion.Recetas ?? new List<PromocionReceta>();

            promocion.Recetas = promocion.Recetas
                .Where(r => r != null && r.IdReceta > 0)
                .ToList();

            if (promocion.FechaFin.Date < promocion.FechaInicio.Date)
            {
                ModelState.AddModelError("FechaFin", "La fecha de fin no puede ser menor que la fecha de inicio.");
            }


            if (!promocion.Recetas.Any())
            {
                ModelState.AddModelError("Recetas", "Debes agregar al menos una receta a la promoción.");
            }

            bool recetasDuplicadas = promocion.Recetas
                .GroupBy(r => r.IdReceta)
                .Any(g => g.Count() > 1);

            if (recetasDuplicadas)
            {
                ModelState.AddModelError("Recetas", "No puedes agregar la misma receta más de una vez.");
            }

            var promocionesExistentes = _listarPromociones.Obtener();

            bool nombreDuplicado = promocionesExistentes.Any(p =>
                p.IdPromocion != promocion.IdPromocion &&
                p.Nombre != null &&
                promocion.Nombre != null &&
                p.Nombre.Trim().ToLower() == promocion.Nombre.Trim().ToLower());

            if (nombreDuplicado)
            {
                ModelState.AddModelError("Nombre", "Ya existe otra promoción con ese nombre.");
            }

            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Complete todos los campos obligatorios antes de guardar";
                ViewBag.TodosTipos = _listarTiposPromociones.Obtener();
                ViewBag.RecetasDisponibles = _listarRecetas.ObtenerDisponibles();
                return View(promocion);
            }

            _editarPromociones.Ejecutar(promocion);

            TempData["Mensaje"] = "Promoción actualizada correctamente";
            return RedirectToAction(nameof(ListadoPromociones));


        }

        // POST: Promocion/EliminarPromocion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPromocion(int id)
        {
            _eliminarPromociones.Ejecutar(id);
            
            TempData["Mensaje"] = "Promoción eliminada correctamente";
            return RedirectToAction(nameof(ListadoPromociones));
        }
    }
}