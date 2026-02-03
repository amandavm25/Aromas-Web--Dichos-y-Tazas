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
                    throw;
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
                Estado = categoriaAD.Estado
            };
        }
    }
}