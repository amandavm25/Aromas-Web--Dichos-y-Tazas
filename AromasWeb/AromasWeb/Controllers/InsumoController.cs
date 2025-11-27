using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class InsumoController : Controller
    {
        // GET: Insumo/ListadoInsumos
        public IActionResult ListadoInsumos(string buscar, int? categoria)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;

            // Categorías de ejemplo
            var categorias = new List<CategoriaInsumo>
            {
                new CategoriaInsumo { IdCategoria = 1, NombreCategoria = "Harinas", Estado = true },
                new CategoriaInsumo { IdCategoria = 2, NombreCategoria = "Endulzantes", Estado = true },
                new CategoriaInsumo { IdCategoria = 3, NombreCategoria = "Lácteos", Estado = true },
                new CategoriaInsumo { IdCategoria = 4, NombreCategoria = "Ingredientes Frescos", Estado = true },
                new CategoriaInsumo { IdCategoria = 5, NombreCategoria = "Chocolates", Estado = true },
                new CategoriaInsumo { IdCategoria = 6, NombreCategoria = "Esencias", Estado = true },
                new CategoriaInsumo { IdCategoria = 7, NombreCategoria = "Leudantes", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            // Insumos de ejemplo
            var insumos = new List<Insumo>
            {
                new Insumo
                {
                    IdInsumo = 1,
                    NombreInsumo = "Harina de Trigo",
                    IdCategoria = 1,
                    UnidadMedida = "Kilogramos",
                    CostoUnitario = 2500.00m,
                    CantidadDisponible = 50.00m,
                    StockMinimo = 10.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-30),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 2,
                    NombreInsumo = "Azúcar Blanca",
                    IdCategoria = 2,
                    UnidadMedida = "Kilogramos",
                    CostoUnitario = 1200.00m,
                    CantidadDisponible = 8.00m,
                    StockMinimo = 15.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-25),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 3,
                    NombreInsumo = "Mantequilla",
                    IdCategoria = 3,
                    UnidadMedida = "Kilogramos",
                    CostoUnitario = 4500.00m,
                    CantidadDisponible = 25.00m,
                    StockMinimo = 5.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-20),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 4,
                    NombreInsumo = "Huevos",
                    IdCategoria = 4,
                    UnidadMedida = "Unidades",
                    CostoUnitario = 200.00m,
                    CantidadDisponible = 150.00m,
                    StockMinimo = 50.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-15),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 5,
                    NombreInsumo = "Chocolate en Polvo",
                    IdCategoria = 5,
                    UnidadMedida = "Kilogramos",
                    CostoUnitario = 8000.00m,
                    CantidadDisponible = 3.00m,
                    StockMinimo = 5.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-10),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 6,
                    NombreInsumo = "Vainilla Líquida",
                    IdCategoria = 6,
                    UnidadMedida = "Mililitros",
                    CostoUnitario = 3.50m,
                    CantidadDisponible = 500.00m,
                    StockMinimo = 100.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-8),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 7,
                    NombreInsumo = "Leche Entera",
                    IdCategoria = 3,
                    UnidadMedida = "Litros",
                    CostoUnitario = 1500.00m,
                    CantidadDisponible = 2.00m,
                    StockMinimo = 10.00m,
                    Estado = true,
                    FechaCreacion = DateTime.Now.AddDays(-5),
                    FechaActualizacion = DateTime.Now
                },
                new Insumo
                {
                    IdInsumo = 8,
                    NombreInsumo = "Levadura en Polvo",
                    IdCategoria = 7,
                    UnidadMedida = "Gramos",
                    CostoUnitario = 2.80m,
                    CantidadDisponible = 1000.00m,
                    StockMinimo = 2000.00m,
                    Estado = false,
                    FechaCreacion = DateTime.Now.AddDays(-3),
                    FechaActualizacion = DateTime.Now
                }
            };

            return View(insumos);
        }

        // GET: Insumo/CrearInsumo
        public IActionResult CrearInsumo()
        {
            // Categorías de ejemplo
            var categorias = new List<CategoriaInsumo>
            {
                new CategoriaInsumo { IdCategoria = 1, NombreCategoria = "Harinas", Estado = true },
                new CategoriaInsumo { IdCategoria = 2, NombreCategoria = "Endulzantes", Estado = true },
                new CategoriaInsumo { IdCategoria = 3, NombreCategoria = "Lácteos", Estado = true },
                new CategoriaInsumo { IdCategoria = 4, NombreCategoria = "Ingredientes Frescos", Estado = true },
                new CategoriaInsumo { IdCategoria = 5, NombreCategoria = "Chocolates", Estado = true },
                new CategoriaInsumo { IdCategoria = 6, NombreCategoria = "Esencias", Estado = true },
                new CategoriaInsumo { IdCategoria = 7, NombreCategoria = "Leudantes", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            return View();
        }

        // POST: Insumo/CrearInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearInsumo(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Insumo registrado correctamente";
                return RedirectToAction(nameof(ListadoInsumos));
            }

            // Si hay errores, recargar las categorías
            var categorias = new List<CategoriaInsumo>
            {
                new CategoriaInsumo { IdCategoria = 1, NombreCategoria = "Harinas", Estado = true },
                new CategoriaInsumo { IdCategoria = 2, NombreCategoria = "Endulzantes", Estado = true },
                new CategoriaInsumo { IdCategoria = 3, NombreCategoria = "Lácteos", Estado = true },
                new CategoriaInsumo { IdCategoria = 4, NombreCategoria = "Ingredientes Frescos", Estado = true },
                new CategoriaInsumo { IdCategoria = 5, NombreCategoria = "Chocolates", Estado = true },
                new CategoriaInsumo { IdCategoria = 6, NombreCategoria = "Esencias", Estado = true },
                new CategoriaInsumo { IdCategoria = 7, NombreCategoria = "Leudantes", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            return View(insumo);
        }

        // GET: Insumo/EditarInsumo/5
        public IActionResult EditarInsumo(int id)
        {
            // Categorías de ejemplo
            var categorias = new List<CategoriaInsumo>
            {
                new CategoriaInsumo { IdCategoria = 1, NombreCategoria = "Harinas", Estado = true },
                new CategoriaInsumo { IdCategoria = 2, NombreCategoria = "Endulzantes", Estado = true },
                new CategoriaInsumo { IdCategoria = 3, NombreCategoria = "Lácteos", Estado = true },
                new CategoriaInsumo { IdCategoria = 4, NombreCategoria = "Ingredientes Frescos", Estado = true },
                new CategoriaInsumo { IdCategoria = 5, NombreCategoria = "Chocolates", Estado = true },
                new CategoriaInsumo { IdCategoria = 6, NombreCategoria = "Esencias", Estado = true },
                new CategoriaInsumo { IdCategoria = 7, NombreCategoria = "Leudantes", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            // Insumo de ejemplo
            var insumo = new Insumo
            {
                IdInsumo = id,
                NombreInsumo = "Harina de Trigo",
                IdCategoria = 1,
                UnidadMedida = "Kilogramos",
                CostoUnitario = 2500.00m,
                CantidadDisponible = 50.00m,
                StockMinimo = 10.00m,
                Estado = true,
                FechaCreacion = DateTime.Now.AddDays(-30),
                FechaActualizacion = DateTime.Now
            };

            return View(insumo);
        }

        // POST: Insumo/EditarInsumo
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarInsumo(Insumo insumo)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Insumo actualizado correctamente";
                return RedirectToAction(nameof(ListadoInsumos));
            }

            // Si hay errores, recargar las categorías
            var categorias = new List<CategoriaInsumo>
            {
                new CategoriaInsumo { IdCategoria = 1, NombreCategoria = "Harinas", Estado = true },
                new CategoriaInsumo { IdCategoria = 2, NombreCategoria = "Endulzantes", Estado = true },
                new CategoriaInsumo { IdCategoria = 3, NombreCategoria = "Lácteos", Estado = true },
                new CategoriaInsumo { IdCategoria = 4, NombreCategoria = "Ingredientes Frescos", Estado = true },
                new CategoriaInsumo { IdCategoria = 5, NombreCategoria = "Chocolates", Estado = true },
                new CategoriaInsumo { IdCategoria = 6, NombreCategoria = "Esencias", Estado = true },
                new CategoriaInsumo { IdCategoria = 7, NombreCategoria = "Leudantes", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            return View(insumo);
        }

        // GET: Insumo/RegistrarMovimiento/5
        public IActionResult RegistrarMovimiento(int id)
        {
            // Insumo de ejemplo para el ViewBag
            ViewBag.Insumo = new
            {
                IdInsumo = id,
                NombreInsumo = "Harina de Trigo",
                CantidadDisponible = 50.00m,
                StockMinimo = 10.00m,
                CostoUnitario = 2500.00m,
                UnidadMedida = "kg"
            };

            var movimiento = new MovimientoInsumo
            {
                IdInsumo = id
            };

            return View(movimiento);
        }

        // POST: Insumo/RegistrarMovimiento
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult RegistrarMovimiento(MovimientoInsumo movimiento)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para registrar el movimiento
                string mensaje = movimiento.TipoMovimiento == "E"
                    ? "Entrada registrada correctamente"
                    : "Salida registrada correctamente";

                TempData["Mensaje"] = mensaje;
                return RedirectToAction(nameof(ListadoInsumos));
            }

            // Si hay errores, recargar el insumo
            ViewBag.Insumo = new
            {
                IdInsumo = movimiento.IdInsumo,
                NombreInsumo = "Harina de Trigo",
                CantidadDisponible = 50.00m,
                StockMinimo = 10.00m,
                CostoUnitario = 2500.00m,
                UnidadMedida = "kg"
            };

            return View(movimiento);
        }

        // GET: Insumo/HistorialMovimientos
        public IActionResult HistorialMovimientos()
        {
            var movimientos = new List<MovimientoInsumo>
            {
                new MovimientoInsumo
                {
                    IdMovimiento = 48,
                    IdInsumo = 1,
                    TipoMovimiento = "S",
                    Cantidad = 5.00m,
                    Motivo = "Preparación de pasteles",
                    CostoUnitario = 2500.00m,
                    IdEmpleado = 1,
                    FechaMovimiento = DateTime.Now
                },
                new MovimientoInsumo
                {
                    IdMovimiento = 47,
                    IdInsumo = 2,
                    TipoMovimiento = "E",
                    Cantidad = 20.00m,
                    Motivo = "Compra a proveedor ABC",
                    CostoUnitario = 1200.00m,
                    IdEmpleado = 1,
                    FechaMovimiento = DateTime.Now.AddHours(-4)
                }
            };

            return View(movimientos);
        }

        // POST: Insumo/EliminarInsumo/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarInsumo(int id)
        {
            // Aquí iría la lógica para eliminar el insumo
            TempData["Mensaje"] = "Insumo eliminado correctamente";
            return RedirectToAction(nameof(ListadoInsumos));
        }
    }
}