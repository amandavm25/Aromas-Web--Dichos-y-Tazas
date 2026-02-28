using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Promocion
{
    public class ListarPromociones : IListarPromociones
    {
        // Obtener todas
        public List<Abstracciones.ModeloUI.Promocion> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Include(p => p.TipoPromocion)
                        .OrderBy(p => p.Nombre)
                        .ToList();
                    return promocionesAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener promociones: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    throw;
                }
            }
        }

        // Buscar por nombre
        public List<Abstracciones.ModeloUI.Promocion> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Include(p => p.TipoPromocion)
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

        // Buscar por tipo
        public List<Abstracciones.ModeloUI.Promocion> BuscarPorTipo(int idTipoPromocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Include(p => p.TipoPromocion)
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

        // Buscar por vigencia
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
                            .Include(p => p.TipoPromocion)
                            .Where(p => p.FechaInicio <= hoy && p.FechaFin >= hoy && p.Estado == true)
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }
                    else if (vigencia.ToLower() == "vencida")
                    {
                        promocionesAD = contexto.Promocion
                            .Include(p => p.TipoPromocion)
                            .Where(p => p.FechaFin < hoy)
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }
                    else if (vigencia.ToLower() == "proxima")
                    {
                        promocionesAD = contexto.Promocion
                            .Include(p => p.TipoPromocion)
                            .Where(p => p.FechaInicio > hoy)
                            .OrderBy(p => p.Nombre)
                            .ToList();
                    }
                    else
                    {
                        promocionesAD = contexto.Promocion
                            .Include(p => p.TipoPromocion)
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

        // Obtener por ID
        public Abstracciones.ModeloUI.Promocion ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var promocionAD = contexto.Promocion
                        .Include(p => p.TipoPromocion)
                        .Include(p => p.PromocionRecetas)
                            .ThenInclude(pr => pr.Receta)
                                .ThenInclude(r => r.CategoriaReceta)
                        .FirstOrDefault(p => p.IdPromocion == id);

                    return promocionAD != null ? ConvertirObjetoParaUIConRecetas(promocionAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener promoción por ID: {ex.Message}");
                    throw;
                }
            }
        }

        // Obtener vigentes
        public List<Abstracciones.ModeloUI.Promocion> ObtenerVigentes()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    DateTime hoy = DateTime.Now;
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Include(p => p.TipoPromocion)
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

        // Obtener por fecha
        public List<Abstracciones.ModeloUI.Promocion> ObtenerPorFecha(DateTime fecha)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<PromocionAD> promocionesAD = contexto.Promocion
                        .Include(p => p.TipoPromocion)
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

        // Conversión básica (para listados)
        private Abstracciones.ModeloUI.Promocion ConvertirObjetoParaUI(PromocionAD promocionAD)
        {
            return new Abstracciones.ModeloUI.Promocion
            {
                IdPromocion = promocionAD.IdPromocion,
                Nombre = promocionAD.Nombre,
                Descripcion = promocionAD.Descripcion,
                IdTipoPromocion = promocionAD.IdTipoPromocion,
                NombreTipoPromocion = promocionAD.TipoPromocion?.NombreTipo,
                PorcentajeDescuento = promocionAD.PorcentajeDescuento,
                FechaInicio = promocionAD.FechaInicio,
                FechaFin = promocionAD.FechaFin,
                Estado = promocionAD.Estado
            };
        }

        // Conversión completa (detalle / edición) — incluye recetas asociadas
        private Abstracciones.ModeloUI.Promocion ConvertirObjetoParaUIConRecetas(PromocionAD promocionAD)
        {
            var promocionUI = ConvertirObjetoParaUI(promocionAD);

            if (promocionAD.PromocionRecetas != null && promocionAD.PromocionRecetas.Any())
            {
                promocionUI.Recetas = promocionAD.PromocionRecetas.Select(pr =>
                    new Abstracciones.ModeloUI.PromocionReceta
                    {
                        IdPromocionReceta = pr.IdPromocionReceta,
                        IdPromocion = pr.IdPromocion,
                        IdReceta = pr.IdReceta,
                        PrecioPromocional = pr.PrecioPromocional,
                        PorcentajeDescuento = pr.PorcentajeDescuento,
                        Estado = pr.Estado,
                        NombreReceta = pr.Receta?.Nombre,
                        CategoriaReceta = pr.Receta?.CategoriaReceta?.Nombre,
                        PrecioOriginal = pr.Receta?.PrecioVenta ?? 0
                    }).ToList();
            }

            return promocionUI;
        }
    }
}