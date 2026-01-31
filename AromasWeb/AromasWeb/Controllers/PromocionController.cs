using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class PromocionController : Controller
    {
        private IListarPromociones _listarPromociones;
        private IListarTiposPromociones _listarTiposPromociones;

        public PromocionController()
        {
            _listarPromociones = new LogicaDeNegocio.Promociones.ListarPromociones();
            _listarTiposPromociones = new LogicaDeNegocio.TiposPromociones.ListarTiposPromociones();
        }

        // GET: Promocion/ListadoPromociones
        public IActionResult ListadoPromociones(string buscar, int? tipo, string vigencia)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Tipo = tipo;
            ViewBag.Vigencia = vigencia;

            // Obtener todos los tipos de promoción para el filtro
            var tiposPromocion = _listarTiposPromociones.Obtener();
            ViewBag.TodosTipos = tiposPromocion;

            // Obtener promociones según los filtros
            List<Promocion> promociones;

            if (!string.IsNullOrEmpty(buscar) && tipo.HasValue && !string.IsNullOrEmpty(vigencia))
            {
                // Buscar por nombre, tipo y vigencia
                promociones = _listarPromociones.BuscarPorNombre(buscar)
                    .FindAll(p => p.IdTipoPromocion == tipo.Value && p.VigenciaTexto == vigencia);
            }
            else if (!string.IsNullOrEmpty(buscar) && tipo.HasValue)
            {
                // Buscar por nombre y tipo
                promociones = _listarPromociones.BuscarPorNombre(buscar)
                    .FindAll(p => p.IdTipoPromocion == tipo.Value);
            }
            else if (!string.IsNullOrEmpty(buscar) && !string.IsNullOrEmpty(vigencia))
            {
                // Buscar por nombre y vigencia
                promociones = _listarPromociones.BuscarPorNombre(buscar)
                    .FindAll(p => p.VigenciaTexto == vigencia);
            }
            else if (tipo.HasValue && !string.IsNullOrEmpty(vigencia))
            {
                // Filtrar por tipo y vigencia
                promociones = _listarPromociones.BuscarPorTipo(tipo.Value)
                    .FindAll(p => p.VigenciaTexto == vigencia);
            }
            else if (!string.IsNullOrEmpty(buscar))
            {
                // Solo buscar por nombre
                promociones = _listarPromociones.BuscarPorNombre(buscar);
            }
            else if (tipo.HasValue)
            {
                // Solo filtrar por tipo
                promociones = _listarPromociones.BuscarPorTipo(tipo.Value);
            }
            else if (!string.IsNullOrEmpty(vigencia))
            {
                // Solo filtrar por vigencia
                promociones = _listarPromociones.BuscarPorVigencia(vigencia);
            }
            else
            {
                // Obtener todas
                promociones = _listarPromociones.Obtener();
            }

            return View(promociones);
        }

        // GET: Promocion/CrearPromocion
        public IActionResult CrearPromocion()
        {
            var tiposPromocion = _listarTiposPromociones.Obtener();
            ViewBag.TodosTipos = tiposPromocion;

            // Aquí deberías obtener las recetas disponibles de tu base de datos
            // Por ahora, usaré datos de ejemplo
            var recetasDisponibles = new List<dynamic>
            {
                new { IdReceta = 1, Nombre = "Torta de Chocolate", Categoria = "Pasteles", PrecioVenta = 28000m },
                new { IdReceta = 2, Nombre = "Café Capuchino", Categoria = "Bebidas Calientes", PrecioVenta = 2500m },
                new { IdReceta = 3, Nombre = "Croissant de Mantequilla", Categoria = "Panadería", PrecioVenta = 12000m },
                new { IdReceta = 4, Nombre = "Galletas de Avena", Categoria = "Galletas", PrecioVenta = 9000m },
                new { IdReceta = 5, Nombre = "Muffin de Arándanos", Categoria = "Panadería", PrecioVenta = 13200m },
                new { IdReceta = 6, Nombre = "Brownie de Chocolate", Categoria = "Postres", PrecioVenta = 16000m },
                new { IdReceta = 7, Nombre = "Smoothie de Fresa", Categoria = "Bebidas Frías", PrecioVenta = 4000m },
                new { IdReceta = 8, Nombre = "Cheesecake de Vainilla", Categoria = "Pasteles", PrecioVenta = 25000m }
            };

            ViewBag.RecetasDisponibles = recetasDisponibles;

            return View();
        }

        // POST: Promocion/CrearPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPromocion(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Promoción registrada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            var tiposPromocion = _listarTiposPromociones.Obtener();
            ViewBag.TodosTipos = tiposPromocion;

            var recetasDisponibles = new List<dynamic>
            {
                new { IdReceta = 1, Nombre = "Torta de Chocolate", Categoria = "Pasteles", PrecioVenta = 28000m },
                new { IdReceta = 2, Nombre = "Café Capuchino", Categoria = "Bebidas Calientes", PrecioVenta = 2500m },
                new { IdReceta = 3, Nombre = "Croissant de Mantequilla", Categoria = "Panadería", PrecioVenta = 12000m },
                new { IdReceta = 4, Nombre = "Galletas de Avena", Categoria = "Galletas", PrecioVenta = 9000m }
            };

            ViewBag.RecetasDisponibles = recetasDisponibles;

            return View(promocion);
        }

        // GET: Promocion/EditarPromocion/5
        public IActionResult EditarPromocion(int id)
        {
            var tiposPromocion = _listarTiposPromociones.Obtener();
            ViewBag.TodosTipos = tiposPromocion;

            var recetasDisponibles = new List<dynamic>
            {
                new { IdReceta = 1, Nombre = "Torta de Chocolate", Categoria = "Pasteles", PrecioVenta = 28000m },
                new { IdReceta = 2, Nombre = "Café Capuchino", Categoria = "Bebidas Calientes", PrecioVenta = 2500m },
                new { IdReceta = 3, Nombre = "Croissant de Mantequilla", Categoria = "Panadería", PrecioVenta = 12000m },
                new { IdReceta = 4, Nombre = "Galletas de Avena", Categoria = "Galletas", PrecioVenta = 9000m },
                new { IdReceta = 5, Nombre = "Muffin de Arándanos", Categoria = "Panadería", PrecioVenta = 13200m },
                new { IdReceta = 6, Nombre = "Brownie de Chocolate", Categoria = "Postres", PrecioVenta = 16000m },
                new { IdReceta = 7, Nombre = "Smoothie de Fresa", Categoria = "Bebidas Frías", PrecioVenta = 4000m },
                new { IdReceta = 8, Nombre = "Cheesecake de Vainilla", Categoria = "Pasteles", PrecioVenta = 25000m }
            };

            ViewBag.RecetasDisponibles = recetasDisponibles;

            var promocion = _listarPromociones.ObtenerPorId(id);

            if (promocion == null)
            {
                TempData["Error"] = "Promoción no encontrada";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            return View(promocion);
        }

        // POST: Promocion/EditarPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPromocion(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Promoción actualizada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            var tiposPromocion = _listarTiposPromociones.Obtener();
            ViewBag.TodosTipos = tiposPromocion;

            var recetasDisponibles = new List<dynamic>
            {
                new { IdReceta = 1, Nombre = "Torta de Chocolate", Categoria = "Pasteles", PrecioVenta = 28000m },
                new { IdReceta = 2, Nombre = "Café Capuchino", Categoria = "Bebidas Calientes", PrecioVenta = 2500m },
                new { IdReceta = 3, Nombre = "Croissant de Mantequilla", Categoria = "Panadería", PrecioVenta = 12000m },
                new { IdReceta = 4, Nombre = "Galletas de Avena", Categoria = "Galletas", PrecioVenta = 9000m }
            };

            ViewBag.RecetasDisponibles = recetasDisponibles;

            return View(promocion);
        }

        // POST: Promocion/EliminarPromocion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPromocion(int id)
        {
            // Aquí iría la lógica para eliminar la promoción
            TempData["Mensaje"] = "Promoción eliminada correctamente";
            return RedirectToAction(nameof(ListadoPromociones));
        }
    }
}