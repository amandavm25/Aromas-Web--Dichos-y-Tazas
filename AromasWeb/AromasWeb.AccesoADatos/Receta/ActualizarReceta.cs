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
            using (var contexto = new Contexto())
            {
                try
                {
                    var recetaExistente = contexto.Receta
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
                        contexto.RecetaInsumo.RemoveRange(recetaExistente.RecetaInsumos);
                    }

                    // Agregar los nuevos ingredientes
                    decimal costoTotal = 0;
                    if (laReceta.Ingredientes != null && laReceta.Ingredientes.Any())
                    {
                        foreach (var ingrediente in laReceta.Ingredientes)
                        {
                            // Obtener el insumo para calcular el costo
                            var insumo = contexto.Insumo
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
                                    CostoTotalIngrediente = (int)Math.Round(costoIngrediente)
                                };

                                contexto.RecetaInsumo.Add(nuevoIngrediente);
                            }
                        }
                    }

                    // Actualizar costos calculados
                    recetaExistente.CostoTotal = (int)Math.Round(costoTotal);
                    recetaExistente.CostoPorcion = laReceta.CantidadPorciones > 0
                        ? (int)Math.Round(costoTotal / laReceta.CantidadPorciones)
                        : 0;

                    // Calcular ganancia solo si hay precio de venta
                    if (laReceta.PrecioVenta.HasValue && laReceta.PrecioVenta.Value > 0)
                    {
                        recetaExistente.GananciaNeta = laReceta.PrecioVenta.Value - (int)Math.Round(costoTotal);
                        recetaExistente.PorcentajeMargen = costoTotal > 0
                            ? ((laReceta.PrecioVenta.Value - costoTotal) / costoTotal) * 100
                            : 0;
                    }
                    else
                    {
                        recetaExistente.GananciaNeta = 0;
                        recetaExistente.PorcentajeMargen = 0;
                    }

                    int cantidadDeDatosActualizados = contexto.SaveChanges();

                    return cantidadDeDatosActualizados;
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }
    }
}