using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Receta
{
    public class ListarRecetas : IListarRecetas
    {
        public List<Abstracciones.ModeloUI.Receta> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RecetaAD> recetasAD = contexto.Receta
                        .Include(r => r.CategoriaReceta)
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return recetasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener recetas: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Receta> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RecetaAD> recetasAD = contexto.Receta
                        .Include(r => r.CategoriaReceta)
                        .Where(r => r.Nombre.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return recetasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar recetas por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Receta> BuscarPorCategoria(int idCategoria)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RecetaAD> recetasAD = contexto.Receta
                        .Include(r => r.CategoriaReceta)
                        .Where(r => r.IdCategoriaReceta == idCategoria)
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return recetasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar recetas por categoría: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Receta ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var recetaAD = contexto.Receta
                        .Include(r => r.CategoriaReceta)
                        .Include(r => r.RecetaInsumos)
                            .ThenInclude(ri => ri.Insumo)
                        .FirstOrDefault(r => r.IdReceta == id);

                    return recetaAD != null ? ConvertirObjetoParaUIConIngredientes(recetaAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener receta por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Receta> ObtenerDisponibles()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RecetaAD> recetasAD = contexto.Receta
                        .Include(r => r.CategoriaReceta)
                        .Where(r => r.Disponibilidad == true)
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return recetasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener recetas disponibles: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Receta> ObtenerNoDisponibles()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RecetaAD> recetasAD = contexto.Receta
                        .Include(r => r.CategoriaReceta)
                        .Where(r => r.Disponibilidad == false)
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return recetasAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener recetas no disponibles: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Receta ConvertirObjetoParaUI(RecetaAD recetaAD)
        {
            return new Abstracciones.ModeloUI.Receta
            {
                IdReceta = recetaAD.IdReceta,
                IdCategoriaReceta = recetaAD.IdCategoriaReceta,
                Nombre = recetaAD.Nombre,
                Descripcion = recetaAD.Descripcion,
                CantidadPorciones = recetaAD.CantidadPorciones,
                PasosPreparacion = recetaAD.PasosPreparacion,
                PrecioVenta = recetaAD.PrecioVenta,
                CostoTotal = recetaAD.CostoTotal,
                CostoPorcion = recetaAD.CostoPorcion,
                GananciaNeta = recetaAD.GananciaNeta,
                PorcentajeMargen = recetaAD.PorcentajeMargen,
                Disponibilidad = recetaAD.Disponibilidad,
                NombreCategoria = recetaAD.CategoriaReceta?.Nombre
            };
        }

        private Abstracciones.ModeloUI.Receta ConvertirObjetoParaUIConIngredientes(RecetaAD recetaAD)
        {
            var recetaUI = ConvertirObjetoParaUI(recetaAD);

            if (recetaAD.RecetaInsumos != null && recetaAD.RecetaInsumos.Any())
            {
                recetaUI.Ingredientes = recetaAD.RecetaInsumos.Select(ri => new Abstracciones.ModeloUI.RecetaInsumo
                {
                    IdRecetaInsumo = ri.IdRecetaInsumo,
                    IdInsumo = ri.IdInsumo,
                    IdReceta = ri.IdReceta,
                    CantidadUtilizada = ri.CantidadUtilizada,
                    CostoUnitario = ri.CostoUnitario,
                    CostoTotalIngrediente = ri.CostoTotalIngrediente,
                    NombreInsumo = ri.Insumo?.NombreInsumo,
                    UnidadMedida = ri.Insumo?.UnidadMedida,
                    CantidadDisponible = ri.Insumo?.CantidadDisponible ?? 0
                }).ToList();
            }

            return recetaUI;
        }
    }
}