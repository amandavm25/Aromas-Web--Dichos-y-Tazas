using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasReceta
{
    public class EliminarCategoriaReceta : IEliminarCategoriaReceta
    {
        private Contexto _contexto;

        public EliminarCategoriaReceta()
        {
            _contexto = new Contexto();
        }

        public int Eliminar(int id)
        {
            try
            {
                CategoriaRecetaAD categoriaAEliminar = _contexto.CategoriaReceta
                    .FirstOrDefault(c => c.IdCategoriaReceta == id);

                if (categoriaAEliminar == null)
                {
                    return 0;
                }

                // Guardar información antes de eliminar para registro
                var infoCategoria = new
                {
                    categoriaAEliminar.IdCategoriaReceta,
                    categoriaAEliminar.Nombre,
                    categoriaAEliminar.Estado
                };

                _contexto.CategoriaReceta.Remove(categoriaAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                // Verificar si el error es por relaciones con otras tablas
                if (dbEx.InnerException != null &&
                    dbEx.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("No se puede eliminar la categoría porque tiene recetas asociadas.", dbEx);
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