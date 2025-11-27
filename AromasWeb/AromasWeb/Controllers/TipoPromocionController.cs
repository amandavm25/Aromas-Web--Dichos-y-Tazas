using Microsoft.AspNetCore.Mvc;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.Controllers
{
    public class TipoPromocionController : Controller
    {
        // GET: TipoPromocion/ListadoTiposPromociones
        public IActionResult ListadoTiposPromociones(string buscar)
        {
            ViewBag.Buscar = buscar;

            // Tipos de promoción de ejemplo con estadísticas
            var tiposPromociones = new List<TipoPromocion>
            {
                new TipoPromocion
                {
                    IdTipoPromocion = 1,
                    Nombre = "Descuento Directo",
                    Descripcion = "Descuento porcentual directo aplicado al precio del producto",
                    Estado = true,
                    CantidadPromociones = 3
                },
                new TipoPromocion
                {
                    IdTipoPromocion = 2,
                    Nombre = "2x1",
                    Descripcion = "Dos productos por el precio de uno",
                    Estado = true,
                    CantidadPromociones = 1
                },
                new TipoPromocion
                {
                    IdTipoPromocion = 3,
                    Nombre = "Combo",
                    Descripcion = "Combo de productos con descuento especial",
                    Estado = true,
                    CantidadPromociones = 1
                },
                new TipoPromocion
                {
                    IdTipoPromocion = 4,
                    Nombre = "Happy Hour",
                    Descripcion = "Descuento aplicado en horarios específicos del día",
                    Estado = true,
                    CantidadPromociones = 1
                },
                new TipoPromocion
                {
                    IdTipoPromocion = 5,
                    Nombre = "Día Especial",
                    Descripcion = "Promoción especial para fechas conmemorativas",
                    Estado = true,
                    CantidadPromociones = 1
                },
                new TipoPromocion
                {
                    IdTipoPromocion = 6,
                    Nombre = "Descuento por Volumen",
                    Descripcion = "Descuento basado en cantidad de productos adquiridos",
                    Estado = false,
                    CantidadPromociones = 0
                }
            };

            // Filtrar por búsqueda si existe
            if (!string.IsNullOrWhiteSpace(buscar))
            {
                tiposPromociones = tiposPromociones
                    .Where(t => t.Nombre.Contains(buscar, StringComparison.OrdinalIgnoreCase) ||
                               (t.Descripcion != null && t.Descripcion.Contains(buscar, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return View(tiposPromociones);
        }

        // GET: TipoPromocion/CrearTipoPromocion
        public IActionResult CrearTipoPromocion()
        {
            return View();
        }

        // POST: TipoPromocion/CrearTipoPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearTipoPromocion(TipoPromocion tipoPromocion)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Tipo de promoción registrado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }

        // GET: TipoPromocion/EditarTipoPromocion/5
        public IActionResult EditarTipoPromocion(int id)
        {
            // Datos de ejemplo - en producción vendría de la base de datos
            var tipoPromocion = new TipoPromocion
            {
                IdTipoPromocion = id,
                Nombre = "Descuento Directo",
                Descripcion = "Descuento porcentual directo aplicado al precio del producto",
                Estado = true
            };

            return View(tipoPromocion);
        }

        // POST: TipoPromocion/EditarTipoPromocion
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarTipoPromocion(TipoPromocion tipoPromocion)
        {
            if (ModelState.IsValid)
            {
                TempData["Mensaje"] = "Tipo de promoción actualizado correctamente";
                TempData["TipoMensaje"] = "success";
                return RedirectToAction(nameof(ListadoTiposPromociones));
            }

            return View(tipoPromocion);
        }

        // POST: TipoPromocion/EliminarTipoPromocion/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarTipoPromocion(int id)
        {
            // Aquí iría la lógica para verificar si tiene promociones asociadas
            // Si tiene promociones asociadas, no se puede eliminar

            TempData["Mensaje"] = "Tipo de promoción eliminado correctamente";
            TempData["TipoMensaje"] = "success";
            return RedirectToAction(nameof(ListadoTiposPromociones));
        }

        // GET: TipoPromocion/DetallesTipoPromocion/5
        public IActionResult DetallesTipoPromocion(int id)
        {
            var tipoPromocion = new TipoPromocion
            {
                IdTipoPromocion = id,
                Nombre = "Descuento Directo",
                Descripcion = "Descuento porcentual directo aplicado al precio del producto",
                Estado = true,
                CantidadPromociones = 3
            };

            return View(tipoPromocion);
        }
    }
}