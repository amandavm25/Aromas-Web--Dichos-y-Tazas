using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.TiposPromociones
{
    public class ListarTiposPromociones : IListarTiposPromociones
    {
        public List<Abstracciones.ModeloUI.TipoPromocion> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<TipoPromocionAD> tiposPromocionAD = contexto.TipoPromocion
                        .OrderBy(t => t.NombreTipo)
                        .ToList();
                    return tiposPromocionAD.Select(t => ConvertirObjetoParaUI(t)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener tipos de promoción: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.TipoPromocion> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<TipoPromocionAD> tiposPromocionAD = contexto.TipoPromocion
                        .Where(t => t.NombreTipo.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(t => t.NombreTipo)
                        .ToList();
                    return tiposPromocionAD.Select(t => ConvertirObjetoParaUI(t)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar tipos de promoción por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.TipoPromocion ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var tipoPromocionAD = contexto.TipoPromocion.FirstOrDefault(t => t.IdTipoPromocion == id);
                    return tipoPromocionAD != null ? ConvertirObjetoParaUI(tipoPromocionAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener tipo de promoción por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.TipoPromocion ConvertirObjetoParaUI(TipoPromocionAD tipoPromocionAD)
        {
            return new Abstracciones.ModeloUI.TipoPromocion
            {
                IdTipoPromocion = tipoPromocionAD.IdTipoPromocion,
                Nombre = tipoPromocionAD.NombreTipo,
                Descripcion = tipoPromocionAD.Descripcion,
                Estado = tipoPromocionAD.Estado
            };
        }
    }
}