using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.AccesoADatos.Modelos;
using AromasWeb.Abstracciones.Logica.Promocion;

namespace AromasWeb.AccesoADatos.Promocion
{
    public class CrearPromociones : ICrearPromociones
    {
        public void Ejecutar(Abstracciones.ModeloUI.Promocion promocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var promocionAD = new PromocionAD
                    {
                        Nombre = promocion.Nombre,
                        Descripcion = promocion.Descripcion,
                        IdTipoPromocion = promocion.IdTipoPromocion,
                        PorcentajeDescuento = promocion.PorcentajeDescuento,
                        FechaInicio = promocion.FechaInicio,
                        FechaFin = promocion.FechaFin,
                        Estado = promocion.Estado
                    };

                    contexto.Promocion.Add(promocionAD);
                    contexto.SaveChanges();

                    if (promocion.Recetas != null && promocion.Recetas.Any())
                    {
                        foreach (var receta in promocion.Recetas)
                        {
                            var pr = new PromocionRecetaAD
                            {
                                IdPromocion = promocionAD.IdPromocion,
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
                    Console.WriteLine($"Error al crear promoción: {ex.Message}");
                    throw;
                }
            }
        }
    }
}