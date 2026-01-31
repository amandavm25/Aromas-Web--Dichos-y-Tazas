using AromasWeb.Abstracciones.Logica.Asistencia;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Asistencias
{
    public class ListarAsistencias : IListarAsistencias
    {
        public List<Abstracciones.ModeloUI.Asistencia> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<AsistenciaAD> asistenciasAD = contexto.Asistencia
                        .OrderByDescending(a => a.Fecha)
                        .ThenByDescending(a => a.HoraEntrada)
                        .ToList();
                    return asistenciasAD.Select(a => ConvertirObjetoParaUI(a, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener asistencias: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Asistencia> BuscarPorEmpleado(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<AsistenciaAD> asistenciasAD = contexto.Asistencia
                        .Where(a => a.IdEmpleado == idEmpleado)
                        .OrderByDescending(a => a.Fecha)
                        .ThenByDescending(a => a.HoraEntrada)
                        .ToList();
                    return asistenciasAD.Select(a => ConvertirObjetoParaUI(a, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar asistencias por empleado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Asistencia> BuscarPorFecha(DateTime fecha)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<AsistenciaAD> asistenciasAD = contexto.Asistencia
                        .Where(a => a.Fecha.Date == fecha.Date)
                        .OrderBy(a => a.HoraEntrada)
                        .ToList();
                    return asistenciasAD.Select(a => ConvertirObjetoParaUI(a, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar asistencias por fecha: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Asistencia> BuscarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<AsistenciaAD> asistenciasAD = contexto.Asistencia
                        .Where(a => a.Fecha.Date >= fechaInicio.Date && a.Fecha.Date <= fechaFin.Date)
                        .OrderByDescending(a => a.Fecha)
                        .ThenByDescending(a => a.HoraEntrada)
                        .ToList();
                    return asistenciasAD.Select(a => ConvertirObjetoParaUI(a, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar asistencias por rango de fechas: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Asistencia ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var asistenciaAD = contexto.Asistencia.FirstOrDefault(a => a.IdAsistencia == id);
                    return asistenciaAD != null ? ConvertirObjetoParaUI(asistenciaAD, contexto) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener asistencia por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Asistencia ConvertirObjetoParaUI(AsistenciaAD asistenciaAD, Contexto contexto)
        {
            string nombreEmpleado = "Desconocido";
            string identificacionEmpleado = "";
            string cargoEmpleado = "";

            try
            {
                var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == asistenciaAD.IdEmpleado);
                if (empleado != null)
                {
                    nombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                    identificacionEmpleado = empleado.Identificacion;
                    cargoEmpleado = empleado.Cargo;
                }
            }
            catch
            {
                // Si hay error, usar valores por defecto
            }

            return new Abstracciones.ModeloUI.Asistencia
            {
                IdAsistencia = asistenciaAD.IdAsistencia,
                IdEmpleado = asistenciaAD.IdEmpleado,
                Fecha = asistenciaAD.Fecha,
                HoraEntrada = asistenciaAD.HoraEntrada,
                HoraSalida = asistenciaAD.HoraSalida,
                TiempoAlmuerzo = asistenciaAD.TiempoAlmuerzo,
                HorasRegulares = asistenciaAD.HorasRegulares,
                HorasExtras = asistenciaAD.HorasExtras,
                HorasTotales = asistenciaAD.HorasTotales,
                NombreEmpleado = nombreEmpleado,
                IdentificacionEmpleado = identificacionEmpleado,
                CargoEmpleado = cargoEmpleado
            };
        }
    }
}