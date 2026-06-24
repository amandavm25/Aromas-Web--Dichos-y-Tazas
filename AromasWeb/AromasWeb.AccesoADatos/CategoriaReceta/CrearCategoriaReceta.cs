using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.CategoriasReceta
{
    public class CrearCategoriaReceta : ICrearCategoriaReceta
    {
        public async Task<int> Crear(Abstracciones.ModeloUI.CategoriaReceta categoriaReceta)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Validar que no exista una categoría con el mismo nombre
                    bool nombreExiste = await contexto.CategoriaReceta
                        .AnyAsync(c => c.Nombre == categoriaReceta.Nombre);

                    if (nombreExiste)
                    {
                        throw new Exception("Ya existe una categoría registrada con ese nombre");
                    }

                    CategoriaRecetaAD categoriaAGuardar = ConvertirObjetoParaAD(categoriaReceta);
                    contexto.CategoriaReceta.Add(categoriaAGuardar);
                    int cantidadDeDatosAgregados = await contexto.SaveChangesAsync();

                    return cantidadDeDatosAgregados;
                }
                catch (Exception)
                {
                    throw;
                }
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