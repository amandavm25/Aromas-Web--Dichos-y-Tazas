using AromasWeb.Abstracciones.Logica.MovimientoInsumo;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.MovimientosInsumo
{
    public class ListarMovimientos : IListarMovimientos
    {
        public List<Abstracciones.ModeloUI.MovimientoInsumo> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<MovimientoInsumoAD> movimientosAD = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .OrderByDescending(m => m.FechaMovimiento)
                        .ToList();
                    return movimientosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener movimientos: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.MovimientoInsumo> BuscarPorInsumo(string nombreInsumo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<MovimientoInsumoAD> movimientosAD = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .Where(m => m.Insumo.NombreInsumo.ToLower().Contains(nombreInsumo.ToLower()))
                        .OrderByDescending(m => m.FechaMovimiento)
                        .ToList();
                    return movimientosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar movimientos por insumo: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.MovimientoInsumo> BuscarPorTipo(string tipo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<MovimientoInsumoAD> movimientosAD = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .Where(m => m.TipoMovimiento == tipo)
                        .OrderByDescending(m => m.FechaMovimiento)
                        .ToList();
                    return movimientosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar movimientos por tipo: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.MovimientoInsumo> BuscarPorRangoFechas(DateTime fechaDesde, DateTime fechaHasta)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<MovimientoInsumoAD> movimientosAD = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .Where(m => m.FechaMovimiento >= fechaDesde && m.FechaMovimiento <= fechaHasta)
                        .OrderByDescending(m => m.FechaMovimiento)
                        .ToList();
                    return movimientosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar movimientos por rango de fechas: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.MovimientoInsumo> BuscarConFiltros(string nombreInsumo, string tipo, DateTime? fechaDesde, DateTime? fechaHasta)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var query = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .AsQueryable();

                    // Filtro por nombre de insumo
                    if (!string.IsNullOrEmpty(nombreInsumo))
                    {
                        query = query.Where(m => m.Insumo.NombreInsumo.ToLower().Contains(nombreInsumo.ToLower()));
                    }

                    // Filtro por tipo
                    if (!string.IsNullOrEmpty(tipo))
                    {
                        query = query.Where(m => m.TipoMovimiento == tipo);
                    }

                    // Filtro por rango de fechas
                    if (fechaDesde.HasValue)
                    {
                        query = query.Where(m => m.FechaMovimiento >= fechaDesde.Value);
                    }

                    if (fechaHasta.HasValue)
                    {
                        // Agregar 23:59:59 al fechaHasta para incluir todo el día
                        var fechaHastaFinal = fechaHasta.Value.Date.AddDays(1).AddSeconds(-1);
                        query = query.Where(m => m.FechaMovimiento <= fechaHastaFinal);
                    }

                    List<MovimientoInsumoAD> movimientosAD = query
                        .OrderByDescending(m => m.FechaMovimiento)
                        .ToList();

                    return movimientosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar movimientos con filtros: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.MovimientoInsumo ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var movimientoAD = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .FirstOrDefault(m => m.IdMovimiento == id);
                    return movimientoAD != null ? ConvertirObjetoParaUI(movimientoAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener movimiento por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.MovimientoInsumo> ObtenerUltimosMovimientos(int cantidad)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<MovimientoInsumoAD> movimientosAD = contexto.MovimientoInsumo
                        .Include(m => m.Insumo)
                        .Include(m => m.Empleado)
                        .OrderByDescending(m => m.FechaMovimiento)
                        .Take(cantidad)
                        .ToList();
                    return movimientosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener últimos movimientos: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.MovimientoInsumo ConvertirObjetoParaUI(MovimientoInsumoAD movimientoAD)
        {
            return new Abstracciones.ModeloUI.MovimientoInsumo
            {
                IdMovimiento = movimientoAD.IdMovimiento,
                IdInsumo = movimientoAD.IdInsumo,
                TipoMovimiento = movimientoAD.TipoMovimiento,
                Cantidad = movimientoAD.Cantidad,
                Motivo = movimientoAD.Motivo,
                CostoUnitario = movimientoAD.CostoUnitario,
                IdEmpleado = movimientoAD.IdEmpleado,
                NombreEmpleado = movimientoAD.Empleado != null
                    ? $"{movimientoAD.Empleado.Nombre} {movimientoAD.Empleado.Apellidos}"
                    : "Empleado no encontrado",
                FechaMovimiento = movimientoAD.FechaMovimiento,
                NombreInsumo = movimientoAD.Insumo?.NombreInsumo ?? "Insumo no encontrado",
                UnidadMedida = movimientoAD.Insumo?.UnidadMedida ?? "N/A",
            };
        }
    }
}