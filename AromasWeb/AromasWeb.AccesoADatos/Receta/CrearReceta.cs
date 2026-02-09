//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AromasWeb.AccesoADatos.Receta
//{
//internal class CrearReceta
//{
//}
//}


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
            try
            {
                // Validar que no exista una receta con el mismo nombre
                bool nombreExiste = await _contexto.Receta.AnyAsync(r => r.Nombre == laReceta.Nombre);
                if (nombreExiste)
                {
                    throw new Exception("Ya existe una receta registrada con ese nombre");
                }

                // Validar que la categoría exista
                bool categoriaExiste = await _contexto.CategoriaReceta
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
                        bool insumoExiste = await _contexto.Insumo
                            .AnyAsync(i => i.IdInsumo == ingrediente.IdInsumo);
                        if (!insumoExiste)
                        {
                            throw new Exception($"El insumo con ID {ingrediente.IdInsumo} no existe");
                        }
                    }
                }

                RecetaAD laRecetaAGuardar = ConvertirObjetoParaAD(laReceta);
                _contexto.Receta.Add(laRecetaAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

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

                        _contexto.RecetaInsumo.Add(recetaInsumoAD);
                    }

                    await _contexto.SaveChangesAsync();
                }

                // Aquí podrías agregar auditoría implementada
                // if (cantidadDeDatosAgregados > 0)
                // {
                //     _auditoria.RegistrarCreacion("Receta", laRecetaAGuardar.IdReceta, new
                //     {
                //         laRecetaAGuardar.IdReceta,
                //         laRecetaAGuardar.Nombre,
                //         laRecetaAGuardar.IdCategoriaReceta,
                //         laRecetaAGuardar.Descripcion,
                //         laRecetaAGuardar.CantidadPorciones,
                //         laRecetaAGuardar.PrecioVenta,
                //         laRecetaAGuardar.CostoTotal,
                //         laRecetaAGuardar.Disponibilidad
                //     });
                // }

                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar receta: {ex.Message}");
                throw;
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