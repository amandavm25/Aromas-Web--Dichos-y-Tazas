using AromasWeb.Abstracciones.Logica.Bitacora;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Bitacoras
{
    public class ListarBitacoras : IListarBitacora
    {
        public List<Abstracciones.ModeloUI.Bitacora> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<BitacoraAD> bitacorasAD = contexto.Bitacora
                        .OrderByDescending(b => b.Fecha)
                        .ToList();

                    return bitacorasAD.Select(b => ConvertirObjetoParaUI(b, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener bitácoras: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Bitacora> BuscarPorFiltros(
            string buscar,
            string filtroModulo,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var query = contexto.Bitacora.AsQueryable();

                    // Guardar null checks: Descripcion y TablaAfectada pueden ser null en la BD
                    if (!string.IsNullOrWhiteSpace(buscar))
                    {
                        string buscarLower = buscar.ToLower();
                        query = query.Where(b =>
                            b.Accion.ToLower().Contains(buscarLower) ||
                            (b.Descripcion != null && b.Descripcion.ToLower().Contains(buscarLower)) ||
                            (b.TablaAfectada != null && b.TablaAfectada.ToLower().Contains(buscarLower))
                        );
                    }

                    if (!string.IsNullOrWhiteSpace(filtroModulo))
                    {
                        var modulo = contexto.Modulo
                            .FirstOrDefault(m => m.Nombre.ToLower() == filtroModulo.ToLower());
                        if (modulo != null)
                            query = query.Where(b => b.IdModulo == modulo.IdModulo);
                    }

                    if (fechaInicio.HasValue)
                        query = query.Where(b => b.Fecha.Date >= fechaInicio.Value.Date);

                    if (fechaFin.HasValue)
                        query = query.Where(b => b.Fecha.Date <= fechaFin.Value.Date);

                    List<BitacoraAD> bitacorasAD = query
                        .OrderByDescending(b => b.Fecha)
                        .ToList();

                    return bitacorasAD.Select(b => ConvertirObjetoParaUI(b, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar bitácoras por filtros: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Bitacora ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var bitacoraAD = contexto.Bitacora
                        .FirstOrDefault(b => b.IdBitacora == id);
                    return bitacoraAD != null ? ConvertirObjetoParaUI(bitacoraAD, contexto) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener bitácora por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Bitacora> ObtenerPorEmpleado(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<BitacoraAD> bitacorasAD = contexto.Bitacora
                        .Where(b => b.IdEmpleado == idEmpleado)
                        .OrderByDescending(b => b.Fecha)
                        .ToList();

                    return bitacorasAD.Select(b => ConvertirObjetoParaUI(b, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener bitácoras por empleado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Bitacora> ObtenerPorModulo(int idModulo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<BitacoraAD> bitacorasAD = contexto.Bitacora
                        .Where(b => b.IdModulo == idModulo)
                        .OrderByDescending(b => b.Fecha)
                        .ToList();

                    return bitacorasAD.Select(b => ConvertirObjetoParaUI(b, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener bitácoras por módulo: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Bitacora> ObtenerPorRangoDeFechas(
            DateTime fechaInicio,
            DateTime fechaFin)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<BitacoraAD> bitacorasAD = contexto.Bitacora
                        .Where(b => b.Fecha.Date >= fechaInicio.Date && b.Fecha.Date <= fechaFin.Date)
                        .OrderByDescending(b => b.Fecha)
                        .ToList();

                    return bitacorasAD.Select(b => ConvertirObjetoParaUI(b, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener bitácoras por rango de fechas: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Bitacora ConvertirObjetoParaUI(
            BitacoraAD bitacoraAD,
            Contexto contexto)
        {
            var empleado = contexto.Empleado
                .FirstOrDefault(e => e.IdEmpleado == bitacoraAD.IdEmpleado);
            string nombreEmpleado = empleado != null
                ? $"{empleado.Nombre} {empleado.Apellidos}".Trim()
                : "Empleado Desconocido";

            var modulo = contexto.Modulo
                .FirstOrDefault(m => m.IdModulo == bitacoraAD.IdModulo);
            string nombreModulo = modulo != null ? modulo.Nombre : "Módulo Desconocido";

            return new Abstracciones.ModeloUI.Bitacora
            {
                IdBitacora = bitacoraAD.IdBitacora,
                IdEmpleado = bitacoraAD.IdEmpleado,
                NombreEmpleado = nombreEmpleado,
                IdModulo = bitacoraAD.IdModulo,
                NombreModulo = nombreModulo,
                Accion = bitacoraAD.Accion,
                // Operador ?? para columnas TEXT que pueden ser NULL en PostgreSQL
                TablaAfectada = bitacoraAD.TablaAfectada ?? null,
                Descripcion = bitacoraAD.Descripcion ?? null,
                DatosAnteriores = bitacoraAD.DatosAnteriores ?? null,
                DatosNuevos = bitacoraAD.DatosNuevos ?? null,
                Fecha = bitacoraAD.Fecha
            };
        }
    }
}