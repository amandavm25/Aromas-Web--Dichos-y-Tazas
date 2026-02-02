using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Recetas
{
    public class ActualizarReceta : IActualizarReceta
    {
        private Contexto _contexto;

        public ActualizarReceta()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.Receta laReceta)
        {
            try
            {
                var recetaExistente = _contexto.Receta
                    .Include(r => r.RecetaInsumos)
                    .FirstOrDefault(r => r.IdReceta == laReceta.IdReceta);

                if (recetaExistente == null)
                {
                    return 0;
                }

                // Actualizar campos básicos de la receta
                recetaExistente.IdCategoriaReceta = laReceta.IdCategoriaReceta;
                recetaExistente.Nombre = laReceta.Nombre;
                recetaExistente.Descripcion = laReceta.Descripcion;
                recetaExistente.CantidadPorciones = laReceta.CantidadPorciones;
                recetaExistente.PasosPreparacion = laReceta.PasosPreparacion;
                recetaExistente.PrecioVenta = laReceta.PrecioVenta;
                recetaExistente.Disponibilidad = laReceta.Disponibilidad;

                // Eliminar ingredientes anteriores
                if (recetaExistente.RecetaInsumos != null && recetaExistente.RecetaInsumos.Any())
                {
                    _contexto.RecetaInsumo.RemoveRange(recetaExistente.RecetaInsumos);
                }

                // Agregar los nuevos ingredientes
                decimal costoTotal = 0;
                if (laReceta.Ingredientes != null && laReceta.Ingredientes.Any())
                {
                    foreach (var ingrediente in laReceta.Ingredientes)
                    {
                        // Obtener el insumo para calcular el costo
                        var insumo = _contexto.Insumo
                            .FirstOrDefault(i => i.IdInsumo == ingrediente.IdInsumo);

                        if (insumo != null)
                        {
                            decimal costoIngrediente = ingrediente.CantidadUtilizada * insumo.CostoUnitario;
                            costoTotal += costoIngrediente;

                            var nuevoIngrediente = new RecetaInsumoAD
                            {
                                IdReceta = laReceta.IdReceta,
                                IdInsumo = ingrediente.IdInsumo,
                                CantidadUtilizada = ingrediente.CantidadUtilizada,
                                CostoUnitario = insumo.CostoUnitario,
                                CostoTotalIngrediente = costoIngrediente
                            };

                            _contexto.RecetaInsumo.Add(nuevoIngrediente);
                        }
                    }
                }

                // Actualizar costos calculados
                recetaExistente.CostoTotal = costoTotal;
                recetaExistente.CostoPorcion = laReceta.CantidadPorciones > 0
                    ? costoTotal / laReceta.CantidadPorciones
                    : 0;

                // Calcular ganancia solo si hay precio de venta
                if (laReceta.PrecioVenta.HasValue && laReceta.PrecioVenta.Value > 0)
                {
                    recetaExistente.GananciaNeta = laReceta.PrecioVenta.Value - costoTotal;
                    recetaExistente.PorcentajeMargen = costoTotal > 0
                        ? ((laReceta.PrecioVenta.Value - costoTotal) / costoTotal) * 100
                        : 0;
                }
                else
                {
                    recetaExistente.GananciaNeta = 0;
                    recetaExistente.PorcentajeMargen = 0;
                }

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}