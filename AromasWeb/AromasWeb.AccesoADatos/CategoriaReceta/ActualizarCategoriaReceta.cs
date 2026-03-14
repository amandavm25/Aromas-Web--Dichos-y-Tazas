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
            using (var contexto = new Contexto())
            {
                try
                {
                    var categoriaExistente = contexto.CategoriaReceta
                        .FirstOrDefault(c => c.IdCategoriaReceta == categoria.IdCategoriaReceta);

                    if (categoriaExistente == null)
                    {
                        return 0;
                    }

                    // Validar que no exista otra categoría con el mismo nombre
                    bool nombreDuplicado = contexto.CategoriaReceta
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