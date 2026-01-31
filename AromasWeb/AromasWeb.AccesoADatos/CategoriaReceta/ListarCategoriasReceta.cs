using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasReceta
{
    public class ListarCategoriasReceta : IListarCategoriasReceta
    {
        public List<Abstracciones.ModeloUI.CategoriaReceta> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<CategoriaRecetaAD> categoriasAD = contexto.CategoriaReceta
                        .OrderBy(c => c.Nombre)
                        .ToList();
                    return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener categorías de recetas: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.CategoriaReceta> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<CategoriaRecetaAD> categoriasAD = contexto.CategoriaReceta
                        .Where(c => c.Nombre.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(c => c.Nombre)
                        .ToList();
                    return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar categorías de recetas por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.CategoriaReceta ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var categoriaAD = contexto.CategoriaReceta.FirstOrDefault(c => c.IdCategoriaReceta == id);
                    return categoriaAD != null ? ConvertirObjetoParaUI(categoriaAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener categoría de receta por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.CategoriaReceta ConvertirObjetoParaUI(CategoriaRecetaAD categoriaAD)
        {
            return new Abstracciones.ModeloUI.CategoriaReceta
            {
                IdCategoriaReceta = categoriaAD.IdCategoriaReceta,
                Nombre = categoriaAD.Nombre,
                Descripcion = categoriaAD.Descripcion,
                Estado = categoriaAD.Estado
            };
        }
    }
}