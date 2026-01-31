using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.AccesoADatos.Modelos;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasInsumo
{
    public class ListarCategoriasInsumo : IListarCategoriasInsumo
    {
        public List<Abstracciones.ModeloUI.CategoriaInsumo> Obtener()
        {
            // USAR USING PARA DISPOSE AUTOMÁTICO DEL CONTEXTO
            using (var contexto = new Contexto())
            {
                try
                {
                    List<CategoriaInsumoAD> categoriasAD = contexto.CategoriaInsumo
                        .OrderBy(c => c.NombreCategoria)
                        .ToList();
                    return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    // Log del error
                    Console.WriteLine($"Error al obtener categorías: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    // Si hay inner exception, también logearla
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw; // Re-lanzar la excepción
                }
            }
        }

        public List<Abstracciones.ModeloUI.CategoriaInsumo> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<CategoriaInsumoAD> categoriasAD = contexto.CategoriaInsumo
                        .Where(c => c.NombreCategoria.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(c => c.NombreCategoria)
                        .ToList();
                    return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar categorías por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.CategoriaInsumo ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var categoriaAD = contexto.CategoriaInsumo.FirstOrDefault(c => c.IdCategoria == id);
                    return categoriaAD != null ? ConvertirObjetoParaUI(categoriaAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener categoría por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.CategoriaInsumo ConvertirObjetoParaUI(CategoriaInsumoAD categoriaAD)
        {
            return new Abstracciones.ModeloUI.CategoriaInsumo
            {
                IdCategoria = categoriaAD.IdCategoria,
                NombreCategoria = categoriaAD.NombreCategoria,
                Descripcion = categoriaAD.Descripcion,
                Estado = categoriaAD.Estado,
                FechaCreacion = categoriaAD.FechaCreacion,
                FechaActualizacion = categoriaAD.FechaActualizacion
            };
        }
    }
}