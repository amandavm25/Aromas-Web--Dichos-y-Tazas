using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasReceta
{
    public class ActualizarCategoriaReceta : IActualizarCategoriaReceta
    {
        private Contexto _contexto;

        public ActualizarCategoriaReceta()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.CategoriaReceta categoria)
        {
            try
            {
                var categoriaExistente = _contexto.CategoriaReceta
                    .FirstOrDefault(c => c.IdCategoriaReceta == categoria.IdCategoriaReceta);

                if (categoriaExistente == null)
                {
                    return 0;
                }

                // Validar que no exista otra categoría con el mismo nombre
                bool nombreDuplicado = _contexto.CategoriaReceta
                    .Any(c => c.Nombre == categoria.Nombre
                           && c.IdCategoriaReceta != categoria.IdCategoriaReceta);

                if (nombreDuplicado)
                {
                    throw new Exception("Ya existe otra categoría con ese nombre");
                }

                // Actualizar campos
                categoriaExistente.Nombre = categoria.Nombre?.Trim();
                categoriaExistente.Descripcion = categoria.Descripcion?.Trim();
                categoriaExistente.Estado = categoria.Estado;

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar categoría de receta: {ex.Message}");
                throw;
            }
        }
    }
}