using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.CategoriasReceta
{
    public class CrearCategoriaReceta : ICrearCategoriaReceta
    {
        private Contexto _contexto;

        public CrearCategoriaReceta()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(Abstracciones.ModeloUI.CategoriaReceta categoriaReceta)
        {
            try
            {
                // Validar que no exista una categoría con el mismo nombre
                bool nombreExiste = await _contexto.CategoriaReceta
                    .AnyAsync(c => c.Nombre == categoriaReceta.Nombre);

                if (nombreExiste)
                {
                    throw new Exception("Ya existe una categoría registrada con ese nombre");
                }

                CategoriaRecetaAD categoriaAGuardar = ConvertirObjetoParaAD(categoriaReceta);
                _contexto.CategoriaReceta.Add(categoriaAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        private CategoriaRecetaAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.CategoriaReceta categoria)
        {
            return new CategoriaRecetaAD
            {
                Nombre = categoria.Nombre?.Trim(),
                Descripcion = categoria.Descripcion?.Trim(),
                Estado = categoria.Estado
            };
        }
    }
}