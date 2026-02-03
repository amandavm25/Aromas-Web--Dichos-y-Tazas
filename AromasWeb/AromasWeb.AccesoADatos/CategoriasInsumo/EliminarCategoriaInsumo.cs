using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasInsumo
{
    public class EliminarCategoriaInsumo : IEliminarCategoriaInsumo
    {
        private Contexto _contexto;

        public EliminarCategoriaInsumo()
        {
            _contexto = new Contexto();
        }

        public int Eliminar(int id)
        {
            try
            {
                CategoriaInsumoAD categoriaAEliminar = _contexto.CategoriaInsumo
                    .FirstOrDefault(c => c.IdCategoria == id);

                if (categoriaAEliminar == null)
                {
                    return 0;
                }

                // Guardar información antes de eliminar para registro
                var infoCategoria = new
                {
                    categoriaAEliminar.IdCategoria,
                    categoriaAEliminar.NombreCategoria,
                    categoriaAEliminar.Estado
                };

                _contexto.CategoriaInsumo.Remove(categoriaAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                // Verificar si el error es por relaciones con otras tablas
                if (dbEx.InnerException != null &&
                    dbEx.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("No se puede eliminar la categoría porque tiene insumos asociados.", dbEx);
                }

                throw new Exception("No se puede eliminar la categoría. Verifica que no esté siendo utilizada.", dbEx);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}