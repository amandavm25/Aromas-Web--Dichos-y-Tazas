using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.Receta
{
    public class CrearReceta : ICrearReceta
    {
        private Contexto _contexto;

        public CrearReceta()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(Abstracciones.ModeloUI.Receta laReceta)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Validar que no exista una receta con el mismo nombre
                    bool nombreExiste = await contexto.Receta.AnyAsync(r => r.Nombre == laReceta.Nombre);
                    if (nombreExiste)
                    {
                        throw new Exception("Ya existe una receta registrada con ese nombre");
                    }

                    // Validar que la categoría exista
                    bool categoriaExiste = await contexto.CategoriaReceta
                        .AnyAsync(c => c.IdCategoriaReceta == laReceta.IdCategoriaReceta);
                    if (!categoriaExiste)
                    {
                        throw new Exception("La categoría de receta seleccionada no existe");
                    }

                    // Validar ingredientes si existen
                    if (laReceta.Ingredientes != null && laReceta.Ingredientes.Any())
                    {
                        foreach (var ingrediente in laReceta.Ingredientes)
                        {
                            bool insumoExiste = await contexto.Insumo
                                .AnyAsync(i => i.IdInsumo == ingrediente.IdInsumo);
                            if (!insumoExiste)
                            {
                                throw new Exception($"El insumo con ID {ingrediente.IdInsumo} no existe");
                            }
                        }
                    }

                    RecetaAD laRecetaAGuardar = ConvertirObjetoParaAD(laReceta);
                    contexto.Receta.Add(laRecetaAGuardar);
                    int cantidadDeDatosAgregados = await contexto.SaveChangesAsync();

                    // Guardar ingredientes si existen
                    if (cantidadDeDatosAgregados > 0 && laReceta.Ingredientes != null && laReceta.Ingredientes.Any())
                    {
                        foreach (var ingrediente in laReceta.Ingredientes)
                        {
                            RecetaInsumoAD recetaInsumoAD = new RecetaInsumoAD
                            {
                                IdReceta = laRecetaAGuardar.IdReceta,
                                IdInsumo = ingrediente.IdInsumo,
                                CantidadUtilizada = ingrediente.CantidadUtilizada,
                                CostoUnitario = ingrediente.CostoUnitario,
                                CostoTotalIngrediente = ingrediente.CostoTotalIngrediente
                            };

                            contexto.RecetaInsumo.Add(recetaInsumoAD);
                        }

                        await contexto.SaveChangesAsync();
                    }

                    return cantidadDeDatosAgregados;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al guardar receta: {ex.Message}");
                    throw;
                }
            }
        }

        private RecetaAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Receta receta)
        {
            return new RecetaAD
            {
                IdCategoriaReceta = receta.IdCategoriaReceta,
                Nombre = receta.Nombre?.Trim(),
                Descripcion = receta.Descripcion?.Trim(),
                CantidadPorciones = receta.CantidadPorciones,
                PasosPreparacion = receta.PasosPreparacion?.Trim(),
                PrecioVenta = receta.PrecioVenta,
                CostoTotal = receta.CostoTotal,
                CostoPorcion = receta.CostoPorcion,
                GananciaNeta = receta.GananciaNeta,
                PorcentajeMargen = receta.PorcentajeMargen,
                Disponibilidad = receta.Disponibilidad
            };
        }
    }
}