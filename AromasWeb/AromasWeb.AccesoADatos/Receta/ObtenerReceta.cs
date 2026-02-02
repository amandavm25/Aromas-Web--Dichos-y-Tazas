using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Collections.Generic;

namespace AromasWeb.AccesoADatos.Recetas
{
    public class ObtenerReceta : IObtenerReceta
    {
        private Contexto _contexto;

        public ObtenerReceta()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Receta Obtener(int idReceta)
        {
            var recetaAD = _contexto.Receta
                .Include(r => r.CategoriaReceta)
                .Include(r => r.RecetaInsumos)
                    .ThenInclude(ri => ri.Insumo)
                .FirstOrDefault(r => r.IdReceta == idReceta);

            if (recetaAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(recetaAD);
        }

        private Abstracciones.ModeloUI.Receta ConvertirObjetoParaUI(RecetaAD recetaAD)
        {
            var recetaUI = new Abstracciones.ModeloUI.Receta
            {
                IdReceta = recetaAD.IdReceta,
                IdCategoriaReceta = recetaAD.IdCategoriaReceta,
                Nombre = recetaAD.Nombre,
                Descripcion = recetaAD.Descripcion,
                CantidadPorciones = recetaAD.CantidadPorciones,
                PasosPreparacion = recetaAD.PasosPreparacion,
                PrecioVenta = recetaAD.PrecioVenta,
                CostoTotal = recetaAD.CostoTotal,
                CostoPorcion = recetaAD.CostoPorcion,
                GananciaNeta = recetaAD.GananciaNeta,
                PorcentajeMargen = recetaAD.PorcentajeMargen,
                Disponibilidad = recetaAD.Disponibilidad,
                NombreCategoria = recetaAD.CategoriaReceta?.Nombre,
                Ingredientes = new List<Abstracciones.ModeloUI.RecetaInsumo>()
            };

            // Convertir ingredientes
            if (recetaAD.RecetaInsumos != null && recetaAD.RecetaInsumos.Any())
            {
                foreach (var ri in recetaAD.RecetaInsumos)
                {
                    recetaUI.Ingredientes.Add(new Abstracciones.ModeloUI.RecetaInsumo
                    {
                        IdRecetaInsumo = ri.IdRecetaInsumo,
                        IdInsumo = ri.IdInsumo,
                        IdReceta = ri.IdReceta,
                        CantidadUtilizada = ri.CantidadUtilizada,
                        CostoUnitario = ri.CostoUnitario,
                        CostoTotalIngrediente = ri.CostoTotalIngrediente,
                        NombreInsumo = ri.Insumo?.NombreInsumo,
                        UnidadMedida = ri.Insumo?.UnidadMedida,
                        CantidadDisponible = ri.Insumo?.CantidadDisponible ?? 0
                    });
                }
            }

            return recetaUI;
        }
    }
}