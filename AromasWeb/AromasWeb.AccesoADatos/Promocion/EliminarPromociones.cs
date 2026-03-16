using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.Logica.Promocion;

namespace AromasWeb.AccesoADatos.Promocion
{
    public class EliminarPromociones : IEliminarPromociones
    {
        public void Ejecutar(int idPromocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var promocion = contexto.Promocion
                        .FirstOrDefault(p => p.IdPromocion == idPromocion);

                    if (promocion == null)
                        throw new Exception("Promoción no encontrada");

                    var recetas = contexto.PromocionReceta
                        .Where(r => r.IdPromocion == idPromocion);

                    contexto.PromocionReceta.RemoveRange(recetas);
                    contexto.Promocion.Remove(promocion);

                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al eliminar promoción: {ex.Message}");
                    throw;
                }
            }
        }
    }
}