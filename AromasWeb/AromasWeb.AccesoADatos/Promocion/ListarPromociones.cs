using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Promocion
{
    public class ListarPromociones : IListarPromociones
    {
        public List<Abstracciones.ModeloUI.Promocion> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener promociones: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Promocion> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Where(p => p.Nombre.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar promociones por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Promocion> BuscarPorTipo(int idTipoPromocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Where(p => p.IdTipoPromocion == idTipoPromocion)
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar promociones por tipo: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Promocion> BuscarPorVigencia(string vigencia)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD;
                    DateTime hoy = DateTime.Now;

                    if (vigencia.ToLower() == "vigente")
                    {
                        promocionesAD = contexto.Promocion
                            .Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy && p.Estado == true)
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }
                    else if (vigencia.ToLower() == "vencida")
                    {
                        promocionesAD = contexto.Promocion
                            .Where(p => p.FechaFin < hoy)
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }
                    else if (vigencia.ToLower() == "proxima")
                    {
                        promocionesAD = contexto.Promocion
                            .Where(p => p.FechaInicio > hoy)
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }
                    else
                    {
                        promocionesAD = contexto.Promocion
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }

                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar promociones por vigencia: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Promocion ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var promocionAD = contexto.Promocion.FirstOrDefault(p => p.IdPromocion == id);
                    return promocionAD != null ? ConvertirObjetoParaUI(promocionAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener promoción por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Promocion> ObtenerVigentes()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime hoy = DateTime.Now;
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy && p.Estado == true)
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener promociones vigentes: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Promocion> ObtenerPorFecha(DateTime fecha)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Where(p => p.FechaInicio <= fecha && p.FechaFin >= fecha)
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener promociones por fecha: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Promocion ConvertirObjetoParaUI(PromocionAD promocionAD)
        {
            return new Abstracciones.ModeloUI.Promocion
            {
                IdPromocion = promocionAD.IdPromocion,
                Nombre = promocionAD.Nombre,
                Descripcion = promocionAD.Descripcion,
                IdTipoPromocion = promocionAD.IdTipoPromocion,
                PorcentajeDescuento = promocionAD.PorcentajeDescuento,
                FechaInicio = promocionAD.FechaInicio,
                FechaFin = promocionAD.FechaFin,
                Estado = promocionAD.Estado
            };
        }
    }
}