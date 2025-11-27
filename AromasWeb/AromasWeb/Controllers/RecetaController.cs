using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.Controllers
{
    public class RecetaController : Controller
    {
        // GET: Receta/ListadoRecetas
        public IActionResult ListadoRecetas(string buscar, int? categoria)
        {
            ViewBag.Buscar = buscar;
            ViewBag.Categoria = categoria;

            // Categorías de ejemplo
            var categorias = new List<CategoriaReceta>
            {
                new CategoriaReceta { IdCategoriaReceta = 1, Nombre = "Pasteles", Descripcion = "Pasteles y tortas", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 2, Nombre = "Galletas", Descripcion = "Galletas y cookies", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 3, Nombre = "Panadería", Descripcion = "Productos de panadería", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 4, Nombre = "Bebidas Calientes", Descripcion = "Café, té y bebidas calientes", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 5, Nombre = "Bebidas Frías", Descripcion = "Bebidas frías y refrescantes", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 6, Nombre = "Postres", Descripcion = "Postres variados", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            // Recetas de ejemplo
            var recetas = new List<Receta>
            {
                new Receta
                {
                    IdReceta = 1,
                    IdCategoriaReceta = 1,
                    Nombre = "Torta de Chocolate",
                    Descripcion = "Deliciosa torta de chocolate con cobertura",
                    CantidadPorciones = 12,
                    PasosPreparacion = "1. Precalentar el horno a 180°C\n2. Mezclar ingredientes secos\n3. Agregar ingredientes húmedos\n4. Hornear por 35 minutos",
                    PrecioVenta = 28000m,
                    CostoTotal = 15000m,
                    CostoPorcion = 1250m,
                    GananciaNeta = 13000m,
                    PorcentajeMargen = 46.43m,
                    Disponibilidad = true,
                    NombreCategoria = "Pasteles"
                },
                new Receta
                {
                    IdReceta = 2,
                    IdCategoriaReceta = 4,
                    Nombre = "Café Capuchino",
                    Descripcion = "Café espresso con leche vaporizada y espuma",
                    CantidadPorciones = 1,
                    PasosPreparacion = "1. Preparar espresso\n2. Vaporizar la leche\n3. Verter leche sobre el espresso\n4. Decorar con cacao en polvo",
                    PrecioVenta = 2500m,
                    CostoTotal = 800m,
                    CostoPorcion = 800m,
                    GananciaNeta = 1700m,
                    PorcentajeMargen = 68.00m,
                    Disponibilidad = true,
                    NombreCategoria = "Bebidas Calientes"
                },
                new Receta
                {
                    IdReceta = 3,
                    IdCategoriaReceta = 3,
                    Nombre = "Croissant de Mantequilla",
                    Descripcion = "Croissant francés hojaldrado con mantequilla",
                    CantidadPorciones = 10,
                    PasosPreparacion = "1. Preparar masa madre\n2. Incorporar mantequilla en capas\n3. Plegar y refrigerar\n4. Cortar y formar\n5. Hornear a 200°C",
                    PrecioVenta = 12000m,
                    CostoTotal = 5500m,
                    CostoPorcion = 550m,
                    GananciaNeta = 6500m,
                    PorcentajeMargen = 54.17m,
                    Disponibilidad = true,
                    NombreCategoria = "Panadería"
                },
                new Receta
                {
                    IdReceta = 4,
                    IdCategoriaReceta = 2,
                    Nombre = "Galletas de Avena",
                    Descripcion = "Galletas caseras de avena con pasas",
                    CantidadPorciones = 24,
                    PasosPreparacion = "1. Mezclar mantequilla con azúcar\n2. Agregar avena y harina\n3. Formar bolitas\n4. Hornear 15 minutos a 180°C",
                    PrecioVenta = 9000m,
                    CostoTotal = 4000m,
                    CostoPorcion = 166.67m,
                    GananciaNeta = 5000m,
                    PorcentajeMargen = 55.56m,
                    Disponibilidad = true,
                    NombreCategoria = "Galletas"
                },
                new Receta
                {
                    IdReceta = 5,
                    IdCategoriaReceta = 3,
                    Nombre = "Muffin de Arándanos",
                    Descripcion = "Muffins esponjosos con arándanos frescos",
                    CantidadPorciones = 12,
                    PasosPreparacion = "1. Mezclar ingredientes secos\n2. Combinar con ingredientes líquidos\n3. Agregar arándanos\n4. Hornear 20 minutos",
                    PrecioVenta = 13200m,
                    CostoTotal = 6000m,
                    CostoPorcion = 500m,
                    GananciaNeta = 7200m,
                    PorcentajeMargen = 54.55m,
                    Disponibilidad = false,
                    NombreCategoria = "Panadería"
                },
                new Receta
                {
                    IdReceta = 6,
                    IdCategoriaReceta = 6,
                    Nombre = "Brownie de Chocolate",
                    Descripcion = "Brownie denso y chocolatoso",
                    CantidadPorciones = 16,
                    PasosPreparacion = "1. Derretir chocolate con mantequilla\n2. Mezclar con azúcar y huevos\n3. Agregar harina\n4. Hornear 25 minutos",
                    PrecioVenta = 16000m,
                    CostoTotal = 8500m,
                    CostoPorcion = 531.25m,
                    GananciaNeta = 7500m,
                    PorcentajeMargen = 46.88m,
                    Disponibilidad = true,
                    NombreCategoria = "Postres"
                },
                new Receta
                {
                    IdReceta = 7,
                    IdCategoriaReceta = 5,
                    Nombre = "Smoothie de Fresa",
                    Descripcion = "Batido refrescante de fresa con yogurt",
                    CantidadPorciones = 2,
                    PasosPreparacion = "1. Lavar y cortar fresas\n2. Licuar con yogurt y hielo\n3. Agregar miel al gusto\n4. Servir frío",
                    PrecioVenta = 4000m,
                    CostoTotal = 1500m,
                    CostoPorcion = 750m,
                    GananciaNeta = 2500m,
                    PorcentajeMargen = 62.50m,
                    Disponibilidad = true,
                    NombreCategoria = "Bebidas Frías"
                },
                new Receta
                {
                    IdReceta = 8,
                    IdCategoriaReceta = 1,
                    Nombre = "Cheesecake de Vainilla",
                    Descripcion = "Pastel de queso cremoso con base de galleta",
                    CantidadPorciones = 10,
                    PasosPreparacion = "1. Preparar base de galleta\n2. Batir queso crema con azúcar\n3. Agregar vainilla y huevos\n4. Hornear a baño maría\n5. Refrigerar 4 horas",
                    PrecioVenta = 25000m,
                    CostoTotal = 12000m,
                    CostoPorcion = 1200m,
                    GananciaNeta = 13000m,
                    PorcentajeMargen = 52.00m,
                    Disponibilidad = true,
                    NombreCategoria = "Pasteles"
                }
            };

            return View(recetas);
        }

        // GET: Receta/CrearReceta
        public IActionResult CrearReceta()
        {
            // Categorías de ejemplo
            var categorias = new List<CategoriaReceta>
            {
                new CategoriaReceta { IdCategoriaReceta = 1, Nombre = "Pasteles", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 2, Nombre = "Galletas", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 3, Nombre = "Panadería", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 4, Nombre = "Bebidas Calientes", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 5, Nombre = "Bebidas Frías", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 6, Nombre = "Postres", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            // Insumos disponibles
            var insumos = new List<Insumo>
            {
                new Insumo { IdInsumo = 1, NombreInsumo = "Harina de Trigo", UnidadMedida = "kg", CostoUnitario = 2500.00m, CantidadDisponible = 50.00m },
                new Insumo { IdInsumo = 2, NombreInsumo = "Azúcar Blanca", UnidadMedida = "kg", CostoUnitario = 1200.00m, CantidadDisponible = 8.00m },
                new Insumo { IdInsumo = 3, NombreInsumo = "Mantequilla", UnidadMedida = "kg", CostoUnitario = 4500.00m, CantidadDisponible = 25.00m },
                new Insumo { IdInsumo = 4, NombreInsumo = "Huevos", UnidadMedida = "Unid", CostoUnitario = 200.00m, CantidadDisponible = 150.00m },
                new Insumo { IdInsumo = 5, NombreInsumo = "Chocolate en Polvo", UnidadMedida = "g", CostoUnitario = 8.00m, CantidadDisponible = 3000.00m },
                new Insumo { IdInsumo = 6, NombreInsumo = "Vainilla Líquida", UnidadMedida = "mL", CostoUnitario = 7.00m, CantidadDisponible = 500.00m },
                new Insumo { IdInsumo = 7, NombreInsumo = "Leche Entera", UnidadMedida = "L", CostoUnitario = 1500.00m, CantidadDisponible = 20.00m },
                new Insumo { IdInsumo = 8, NombreInsumo = "Levadura en Polvo", UnidadMedida = "g", CostoUnitario = 2.80m, CantidadDisponible = 1000.00m }
            };

            ViewBag.InsumosDisponibles = insumos;

            return View();
        }

        // POST: Receta/CrearReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearReceta(Receta receta)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para guardar en la base de datos
                TempData["Mensaje"] = "Receta registrada correctamente";
                return RedirectToAction(nameof(ListadoRecetas));
            }

            // Si hay errores, recargar las categorías e insumos
            var categorias = new List<CategoriaReceta>
            {
                new CategoriaReceta { IdCategoriaReceta = 1, Nombre = "Pasteles", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 2, Nombre = "Galletas", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 3, Nombre = "Panadería", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 4, Nombre = "Bebidas Calientes", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 5, Nombre = "Bebidas Frías", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 6, Nombre = "Postres", Estado = true }
            };

            var insumos = new List<Insumo>
            {
                new Insumo { IdInsumo = 1, NombreInsumo = "Harina de Trigo", UnidadMedida = "kg", CostoUnitario = 2500.00m, CantidadDisponible = 50.00m },
                new Insumo { IdInsumo = 2, NombreInsumo = "Azúcar Blanca", UnidadMedida = "kg", CostoUnitario = 1200.00m, CantidadDisponible = 8.00m },
                new Insumo { IdInsumo = 3, NombreInsumo = "Mantequilla", UnidadMedida = "kg", CostoUnitario = 4500.00m, CantidadDisponible = 25.00m },
                new Insumo { IdInsumo = 4, NombreInsumo = "Huevos", UnidadMedida = "Unid", CostoUnitario = 200.00m, CantidadDisponible = 150.00m },
                new Insumo { IdInsumo = 5, NombreInsumo = "Chocolate en Polvo", UnidadMedida = "g", CostoUnitario = 8.00m, CantidadDisponible = 3000.00m },
                new Insumo { IdInsumo = 6, NombreInsumo = "Vainilla Líquida", UnidadMedida = "mL", CostoUnitario = 7.00m, CantidadDisponible = 500.00m },
                new Insumo { IdInsumo = 7, NombreInsumo = "Leche Entera", UnidadMedida = "L", CostoUnitario = 1500.00m, CantidadDisponible = 20.00m },
                new Insumo { IdInsumo = 8, NombreInsumo = "Levadura en Polvo", UnidadMedida = "g", CostoUnitario = 2.80m, CantidadDisponible = 1000.00m }
            };

            ViewBag.TodasCategorias = categorias;
            ViewBag.InsumosDisponibles = insumos;

            return View(receta);
        }

        // GET: Receta/EditarReceta/5
        public IActionResult EditarReceta(int id)
        {
            // Categorías de ejemplo
            var categorias = new List<CategoriaReceta>
            {
                new CategoriaReceta { IdCategoriaReceta = 1, Nombre = "Pasteles", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 2, Nombre = "Galletas", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 3, Nombre = "Panadería", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 4, Nombre = "Bebidas Calientes", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 5, Nombre = "Bebidas Frías", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 6, Nombre = "Postres", Estado = true }
            };

            ViewBag.TodasCategorias = categorias;

            // Insumos disponibles
            var insumos = new List<Insumo>
            {
                new Insumo { IdInsumo = 1, NombreInsumo = "Harina de Trigo", UnidadMedida = "kg", CostoUnitario = 2500.00m, CantidadDisponible = 50.00m },
                new Insumo { IdInsumo = 2, NombreInsumo = "Azúcar Blanca", UnidadMedida = "kg", CostoUnitario = 1200.00m, CantidadDisponible = 8.00m },
                new Insumo { IdInsumo = 3, NombreInsumo = "Mantequilla", UnidadMedida = "kg", CostoUnitario = 4500.00m, CantidadDisponible = 25.00m },
                new Insumo { IdInsumo = 4, NombreInsumo = "Huevos", UnidadMedida = "Unid", CostoUnitario = 200.00m, CantidadDisponible = 150.00m },
                new Insumo { IdInsumo = 5, NombreInsumo = "Chocolate en Polvo", UnidadMedida = "g", CostoUnitario = 8.00m, CantidadDisponible = 3000.00m },
                new Insumo { IdInsumo = 6, NombreInsumo = "Vainilla Líquida", UnidadMedida = "mL", CostoUnitario = 7.00m, CantidadDisponible = 500.00m },
                new Insumo { IdInsumo = 7, NombreInsumo = "Leche Entera", UnidadMedida = "L", CostoUnitario = 1500.00m, CantidadDisponible = 20.00m },
                new Insumo { IdInsumo = 8, NombreInsumo = "Levadura en Polvo", UnidadMedida = "g", CostoUnitario = 2.80m, CantidadDisponible = 1000.00m }
            };

            ViewBag.InsumosDisponibles = insumos;

            // Receta de ejemplo
            var receta = new Receta
            {
                IdReceta = id,
                IdCategoriaReceta = 1,
                Nombre = "Torta de Chocolate",
                Descripcion = "Deliciosa torta de chocolate con cobertura",
                CantidadPorciones = 12,
                PasosPreparacion = "1. Precalentar el horno a 180°C\n2. Mezclar ingredientes secos\n3. Agregar ingredientes húmedos\n4. Hornear por 35 minutos",
                PrecioVenta = 28000m,
                Disponibilidad = true
            };

            // Ingredientes de la receta
            var ingredientes = new List<RecetaInsumo>
            {
                new RecetaInsumo
                {
                    IdInsumo = 1,
                    CantidadUtilizada = 2.5m,
                    NombreInsumo = "Harina de Trigo",
                    UnidadMedida = "kg",
                    CostoUnitario = 2500.00m
                },
                new RecetaInsumo
                {
                    IdInsumo = 2,
                    CantidadUtilizada = 1.5m,
                    NombreInsumo = "Azúcar Blanca",
                    UnidadMedida = "kg",
                    CostoUnitario = 1200.00m
                }
            };

            receta.Ingredientes = ingredientes;

            return View(receta);
        }

        // POST: Receta/EditarReceta
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarReceta(Receta receta)
        {
            if (ModelState.IsValid)
            {
                // Aquí iría la lógica para actualizar en la base de datos
                TempData["Mensaje"] = "Receta actualizada correctamente";
                return RedirectToAction(nameof(ListadoRecetas));
            }

            // Si hay errores, recargar las categorías e insumos
            var categorias = new List<CategoriaReceta>
            {
                new CategoriaReceta { IdCategoriaReceta = 1, Nombre = "Pasteles", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 2, Nombre = "Galletas", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 3, Nombre = "Panadería", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 4, Nombre = "Bebidas Calientes", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 5, Nombre = "Bebidas Frías", Estado = true },
                new CategoriaReceta { IdCategoriaReceta = 6, Nombre = "Postres", Estado = true }
            };

            var insumos = new List<Insumo>
            {
                new Insumo { IdInsumo = 1, NombreInsumo = "Harina de Trigo", UnidadMedida = "kg", CostoUnitario = 2500.00m, CantidadDisponible = 50.00m },
                new Insumo { IdInsumo = 2, NombreInsumo = "Azúcar Blanca", UnidadMedida = "kg", CostoUnitario = 1200.00m, CantidadDisponible = 8.00m },
                new Insumo { IdInsumo = 3, NombreInsumo = "Mantequilla", UnidadMedida = "kg", CostoUnitario = 4500.00m, CantidadDisponible = 25.00m },
                new Insumo { IdInsumo = 4, NombreInsumo = "Huevos", UnidadMedida = "Unid", CostoUnitario = 200.00m, CantidadDisponible = 150.00m },
                new Insumo { IdInsumo = 5, NombreInsumo = "Chocolate en Polvo", UnidadMedida = "g", CostoUnitario = 8.00m, CantidadDisponible = 3000.00m },
                new Insumo { IdInsumo = 6, NombreInsumo = "Vainilla Líquida", UnidadMedida = "mL", CostoUnitario = 7.00m, CantidadDisponible = 500.00m },
                new Insumo { IdInsumo = 7, NombreInsumo = "Leche Entera", UnidadMedida = "L", CostoUnitario = 1500.00m, CantidadDisponible = 20.00m },
                new Insumo { IdInsumo = 8, NombreInsumo = "Levadura en Polvo", UnidadMedida = "g", CostoUnitario = 2.80m, CantidadDisponible = 1000.00m }
            };

            ViewBag.TodasCategorias = categorias;
            ViewBag.InsumosDisponibles = insumos;

            return View(receta);
        }

        // POST: Receta/EliminarReceta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarReceta(int id)
        {
            // Aquí iría la lógica para eliminar la receta
            TempData["Mensaje"] = "Receta eliminada correctamente";
            return RedirectToAction(nameof(ListadoRecetas));
        }

        // POST: Receta/DuplicarReceta/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DuplicarReceta(int id)
        {
            // Aquí iría la lógica para duplicar la receta
            TempData["Mensaje"] = "Receta duplicada correctamente";
            return RedirectToAction(nameof(ListadoRecetas));
        }
    }
}