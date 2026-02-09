using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Receta
{
    public class EliminarReceta : IEliminarReceta
    {
        private Contexto _contexto;

        public EliminarReceta()
        {
            _contexto = new Contexto();
        }

        public int Eliminar(int id)
        {
            try
            {
                // Cargar la receta CON sus ingredientes usando Include
                // Esto permite que Entity Framework sepa que debe eliminar los hijos también
                RecetaAD recetaAEliminar = _contexto.Receta
                    .Include(r => r.RecetaInsumos)  // ⭐ Cargar la relación
                    .FirstOrDefault(r => r.IdReceta == id);

                if (recetaAEliminar == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Receta con ID {id} no encontrada");
                    return 0;
                }

                // Guardar información antes de eliminar para registro
                var infoReceta = new
                {
                    recetaAEliminar.IdReceta,
                    recetaAEliminar.Nombre,
                    recetaAEliminar.IdCategoriaReceta,
                    recetaAEliminar.Descripcion,
                    recetaAEliminar.CantidadPorciones,
                    recetaAEliminar.PrecioVenta,
                    recetaAEliminar.CostoTotal,
                    recetaAEliminar.Disponibilidad,
                    CantidadIngredientes = recetaAEliminar.RecetaInsumos?.Count ?? 0
                };

                System.Diagnostics.Debug.WriteLine($"Eliminando receta '{recetaAEliminar.Nombre}' con {infoReceta.CantidadIngredientes} ingredientes");

                // Con DeleteBehavior.Cascade configurado en el Contexto,
                // EF Core eliminará automáticamente los RecetaInsumo asociados
                _contexto.Receta.Remove(recetaAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                if (cantidadDeDatosEliminados > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"✓ Receta eliminada exitosamente");
                }

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                // Esto podría ocurrir si la receta está en PromocionReceta u otras tablas
                System.Diagnostics.Debug.WriteLine($"Error de BD al eliminar receta: {dbEx.Message}");

                // Verificar si el error es por relaciones con otras tablas
                if (dbEx.InnerException != null &&
                    dbEx.InnerException.Message.Contains("foreign key constraint"))
                {
                    throw new Exception("No se puede eliminar la receta porque está siendo utilizada en promociones, pedidos u otros registros relacionados.", dbEx);
                }

                throw new Exception("No se puede eliminar la receta. Verifica que no esté siendo utilizada en otros registros.", dbEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar receta: {ex.Message}");
                throw;
            }
        }
    }
}