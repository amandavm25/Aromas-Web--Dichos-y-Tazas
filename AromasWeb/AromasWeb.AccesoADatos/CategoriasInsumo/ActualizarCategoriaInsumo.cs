using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasInsumo
{
    public class ActualizarCategoriaInsumo : IActualizarCategoriaInsumo
    {
        private Contexto _contexto;

        public ActualizarCategoriaInsumo()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.CategoriaInsumo categoria)
        {
            try
            {
                var categoriaExistente = _contexto.CategoriaInsumo
                    .FirstOrDefault(c => c.IdCategoria == categoria.IdCategoria);

                if (categoriaExistente == null)
                {
                    return 0;
                }

                // Validar que no exista otra categoría con el mismo nombre
                bool nombreDuplicado = _contexto.CategoriaInsumo
                    .Any(c => c.NombreCategoria == categoria.NombreCategoria
                           && c.IdCategoria != categoria.IdCategoria);

                if (nombreDuplicado)
                {
                    throw new Exception("Ya existe otra categoría con ese nombre");
                }

                // Actualizar campos
                categoriaExistente.NombreCategoria = categoria.NombreCategoria?.Trim();
                categoriaExistente.Descripcion = categoria.Descripcion?.Trim();
                categoriaExistente.Estado = categoria.Estado;

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar categoría: {ex.Message}");
                throw;
            }
        }
    }
}