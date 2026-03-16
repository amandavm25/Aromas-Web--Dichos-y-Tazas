using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.AccesoADatos.Modelos;
using AromasWeb.Abstracciones.Logica.Promocion;

namespace AromasWeb.AccesoADatos.Promocion
{
    public class EditarPromociones : IEditarPromociones
    {
        public void Ejecutar(Abstracciones.ModeloUI.Promocion promocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var promocionAD = contexto.Promocion
                        .FirstOrDefault(p => p.IdPromocion == promocion.IdPromocion);

                    if (promocionAD == null)
                        throw new Exception("Promoción no encontrada");

                    promocionAD.Nombre = promocion.Nombre;
                    promocionAD.Descripcion = promocion.Descripcion;
                    promocionAD.IdTipoPromocion = promocion.IdTipoPromocion;
                    promocionAD.PorcentajeDescuento = promocion.PorcentajeDescuento;
                    promocionAD.FechaInicio = DateTime.SpecifyKind(promocion.FechaInicio, DateTimeKind.Utc);
                    promocionAD.FechaFin = DateTime.SpecifyKind(promocion.FechaFin, DateTimeKind.Utc);
                    promocionAD.Estado = promocion.Estado;

                    contexto.SaveChanges();

                    var recetasActuales = contexto.PromocionReceta
                        .Where(r => r.IdPromocion == promocion.IdPromocion);

                    contexto.PromocionReceta.RemoveRange(recetasActuales);
                    contexto.SaveChanges();

                    if (promocion.Recetas != null && promocion.Recetas.Any())
                    {
                        foreach (var receta in promocion.Recetas)
                        {
                            var pr = new PromocionRecetaAD
                            {
                                IdPromocion = promocion.IdPromocion,
                                IdReceta = receta.IdReceta,
                                PorcentajeDescuento = promocion.PorcentajeDescuento,
                                PrecioPromocional = receta.PrecioPromocional,
                                Estado = true
                            };

                            contexto.PromocionReceta.Add(pr);
                        }

                        contexto.SaveChanges();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al editar promoción: {ex.Message}");
                    throw;
                }
            }
        }
    }
}