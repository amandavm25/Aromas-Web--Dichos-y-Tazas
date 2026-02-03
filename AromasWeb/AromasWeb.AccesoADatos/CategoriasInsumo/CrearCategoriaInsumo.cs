using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.CategoriasInsumo
{
    public class CrearCategoriaInsumo : ICrearCategoriaInsumo
    {
        private Contexto _contexto;

        public CrearCategoriaInsumo()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(Abstracciones.ModeloUI.CategoriaInsumo categoriaInsumo)
        {
            try
            {
                // Validar que no exista una categoría con el mismo nombre
                bool nombreExiste = await _contexto.CategoriaInsumo
                    .AnyAsync(c => c.NombreCategoria == categoriaInsumo.NombreCategoria);

                if (nombreExiste)
                {
                    throw new Exception("Ya existe una categoría registrada con ese nombre");
                }

                CategoriaInsumoAD categoriaAGuardar = ConvertirObjetoParaAD(categoriaInsumo);
                _contexto.CategoriaInsumo.Add(categoriaAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar categoría: {ex.Message}");
                throw;
            }
        }

        private CategoriaInsumoAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.CategoriaInsumo categoria)
        {
            return new CategoriaInsumoAD
            {
                NombreCategoria = categoria.NombreCategoria?.Trim(),
                Descripcion = categoria.Descripcion?.Trim(),
                Estado = categoria.Estado
            };
        }
    }
}