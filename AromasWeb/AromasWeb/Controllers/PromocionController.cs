using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class PromocionController : Controller
    {
        // GET: Promocion/ListadoPromociones
        public IActionResult ListadoPromociones(string buscar, int? tipo, string vigencia)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Tipo = tipo;
            ViewBag.Vigencia = vigencia;

            // Tipos de promoción de ejemplo
            var tiposPromocion = new List<TipoPromocion>
            {
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Descuento Directo", Descripcion = "Descuento porcentual directo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "2x1", Descripcion = "Dos productos por el precio de uno", Estado = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Combo", Descripcion = "Combo de productos con descuento", Estado = true },
                new TipoPromocion { IdTipoPromocion = 4, Nombre = "Happy Hour", Descripcion = "Descuento en horarios específicos", Estado = true },
                new TipoPromocion { IdTipoPromocion = 5, Nombre = "Día Especial", Descripcion = "Promoción para días especiales", Estado = true }
            };

            ViewBag.TodosTipos = tiposPromocion;

            // Promociones de ejemplo
            var promociones = new List<Promocion>
            {
                new Promocion
                {
                    IdPromocion = 1,
                    IdTipoPromocion = 1,
                    Nombre = "Descuento Navideño",
                    Descripcion = "20% de descuento en pasteles durante diciembre",
                    PorcentajeDescuento = 20m,
                    FechaInicio = new DateTime(2024, 12, 1),
                    FechaFin = new DateTime(2024, 12, 31),
                    Estado = true,
                    NombreTipoPromocion = "Descuento Directo",
                    Recetas = new List<PromocionReceta>
                    {
                        new PromocionReceta { IdReceta = 1 },
                        new PromocionReceta { IdReceta = 8 }
                    }
                },
                new Promocion
                {
                    IdPromocion = 2,
                    IdTipoPromocion = 4,
                    Nombre = "Happy Hour Café",
                    Descripcion = "15% de descuento en bebidas calientes de 3pm a 5pm",
                    PorcentajeDescuento = 15m,
                    FechaInicio = new DateTime(2024, 11, 1),
                    FechaFin = new DateTime(2025, 1, 31),
                    Estado = true,
                    NombreTipoPromocion = "Happy Hour",
                    Recetas = new List<PromocionReceta>
                    {
                        new PromocionReceta { IdReceta = 2 }
                    }
                },
                new Promocion
                {
                    IdPromocion = 3,
                    IdTipoPromocion = 2,
                    Nombre = "2x1 Galletas",
                    Descripcion = "Compra dos paquetes de galletas y paga uno",
                    PorcentajeDescuento = 50m,
                    FechaInicio = new DateTime(2024, 11, 15),
                    FechaFin = new DateTime(2024, 11, 30),
                    Estado = true,
                    NombreTipoPromocion = "2x1",
                    Recetas = new List<PromocionReceta>
                    {
                        new PromocionReceta { IdReceta = 4 }
                    }
                },
                new Promocion
                {
                    IdPromocion = 4,
                    IdTipoPromocion = 3,
                    Nombre = "Combo Desayuno",
                    Descripcion = "Croissant + Café a precio especial",
                    PorcentajeDescuento = 25m,
                    FechaInicio = new DateTime(2024, 11, 1),
                    FechaFin = new DateTime(2024, 12, 31),
                    Estado = true,
                    NombreTipoPromocion = "Combo",
                    Recetas = new List<PromocionReceta>
                    {
                        new PromocionReceta { IdReceta = 2 },
                        new PromocionReceta { IdReceta = 3 }
                    }
                },
                new Promocion
                {
                    IdPromocion = 5,
                    IdTipoPromocion = 1,
                    Nombre = "Black Friday",
                    Descripcion = "30% en toda la repostería",
                    PorcentajeDescuento = 30m,
                    FechaInicio = new DateTime(2024, 11, 29),
                    FechaFin = new DateTime(2024, 11, 29),
                    Estado = false,
                    NombreTipoPromocion = "Descuento Directo",
                    Recetas = new List<PromocionReceta>
                    {
                        new PromocionReceta { IdReceta = 1 },
                        new PromocionReceta { IdReceta = 6 },
                        new PromocionReceta { IdReceta = 8 }
                    }
                },
                new Promocion
                {
                    IdPromocion = 6,
                    IdTipoPromocion = 5,
                    Nombre = "Día de San Valentín",
                    Descripcion = "15% en postres especiales",
                    PorcentajeDescuento = 15m,
                    FechaInicio = new DateTime(2025, 2, 14),
                    FechaFin = new DateTime(2025, 2, 14),
                    Estado = true,
                    NombreTipoPromocion = "Día Especial",
                    Recetas = new List<PromocionReceta>
                    {
                        new PromocionReceta { IdReceta = 1 },
                        new PromocionReceta { IdReceta = 8 }
                    }
                }
            };

            return View(promociones);
        }

        // GET: Promocion/CrearPromocion
        public IActionResult CrearPromocion()
        {
            var tiposPromocion = new List<TipoPromocion>
            {
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Descuento Directo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "2x1", Estado = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Combo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 4, Nombre = "Happy Hour", Estado = true },
                new TipoPromocion { IdTipoPromocion = 5, Nombre = "Día Especial", Estado = true }
            };

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

            return View();
        }

        // POST: Promocion/CrearPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearPromocion(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Promoción registrada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            var tiposPromocion = new List<TipoPromocion>
            {
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Descuento Directo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "2x1", Estado = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Combo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 4, Nombre = "Happy Hour", Estado = true },
                new TipoPromocion { IdTipoPromocion = 5, Nombre = "Día Especial", Estado = true }
            };

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
            var tiposPromocion = new List<TipoPromocion>
            {
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Descuento Directo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "2x1", Estado = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Combo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 4, Nombre = "Happy Hour", Estado = true },
                new TipoPromocion { IdTipoPromocion = 5, Nombre = "Día Especial", Estado = true }
            };

            ViewBag.TodosTipos = tiposPromocion;

            var recetasDisponibles = new List<dynamic>
            {
                new { IdReceta = 1, Nombre = "Torta de Chocolate", Categoria = "Pasteles", PrecioVenta = 28000m },
                new { IdReceta = 2, Nombre = "Café Capuchino", Categoria = "Bebidas Calientes", PrecioVenta = 2500m },
                new { IdReceta = 3, Nombre = "Croissant de Mantequilla", Categoria = "Panadería", PrecioVenta = 12000m },
                new { IdReceta = 4, Nombre = "Galletas de Avena", Categoria = "Galletas", PrecioVenta = 9000m }
            };

            ViewBag.RecetasDisponibles = recetasDisponibles;

            var promocion = new Promocion
            {
                IdPromocion = id,
                IdTipoPromocion = 1,
                Nombre = "Descuento Navideño",
                Descripcion = "20% de descuento en pasteles durante diciembre",
                PorcentajeDescuento = 20m,
                FechaInicio = new DateTime(2024, 12, 1),
                FechaFin = new DateTime(2024, 12, 31),
                Estado = true,
                Recetas = new List<PromocionReceta>
                {
                    new PromocionReceta { IdPromocionReceta = 1, IdPromocion = id, IdReceta = 1, PorcentajeDescuento = 20m },
                    new PromocionReceta { IdPromocionReceta = 2, IdPromocion = id, IdReceta = 8, PorcentajeDescuento = 20m }
                }
            };

            return View(promocion);
        }

        // POST: Promocion/EditarPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarPromocion(Promocion promocion)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Promoción actualizada correctamente";
                return RedirectToAction(nameof(ListadoPromociones));
            }

            var tiposPromocion = new List<TipoPromocion>
            {
                new TipoPromocion { IdTipoPromocion = 1, Nombre = "Descuento Directo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 2, Nombre = "2x1", Estado = true },
                new TipoPromocion { IdTipoPromocion = 3, Nombre = "Combo", Estado = true },
                new TipoPromocion { IdTipoPromocion = 4, Nombre = "Happy Hour", Estado = true },
                new TipoPromocion { IdTipoPromocion = 5, Nombre = "Día Especial", Estado = true }
            };

            ViewBag.TodosTipos = tiposPromocion;
            return View(promocion);
        }

        // POST: Promocion/EliminarPromocion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarPromocion(int id)
        {
            TempData["Mensaje"] = "Promoción eliminada correctamente";
            return RedirectToAction(nameof(ListadoPromociones));
        }
    }
}