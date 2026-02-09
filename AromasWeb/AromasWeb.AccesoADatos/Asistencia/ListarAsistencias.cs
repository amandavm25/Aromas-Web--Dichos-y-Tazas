using AromasWeb.Abstracciones.Logica.Asistencia;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
                    var inicio = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
                    var fin = inicio.AddDays(1);

                    List<AsistenciaAD> asistenciasAD = contexto.Asistencia
                        .Where(a => a.Fecha >= inicio && a.Fecha <fin)
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
                    var inicio = DateTime.SpecifyKind(fechaInicio.Date, DateTimeKind.Utc);

                    var fin = DateTime.SpecifyKind(fechaFin.Date, DateTimeKind.Utc).AddDays(1);

                    List<AsistenciaAD> asistenciasAD = contexto.Asistencia
                        .Where(a => a.Fecha >= inicio && a.Fecha < fin)
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

            var fechaUtc = DateTime.SpecifyKind(asistenciaAD.Fecha, DateTimeKind.Utc);

            return new Abstracciones.ModeloUI.Asistencia
            {
                IdAsistencia = asistenciaAD.IdAsistencia,
                IdEmpleado = asistenciaAD.IdEmpleado,
                Fecha = fechaUtc,
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

        public bool ExisteEntradaAbierta(int idEmpleado, DateTime fecha)
        {
            using (var contexto = new Contexto())
            {

                var inicioDiaUtc = DateTime.SpecifyKind(fecha.Date, DateTimeKind.Utc);
                var finDiaUtc = inicioDiaUtc.AddDays(1);

                return contexto.Asistencia.Any(a =>
                    a.IdEmpleado == idEmpleado &&
                    a.Fecha >= inicioDiaUtc &&
                    a.Fecha < finDiaUtc &&
                    a.HoraSalida == null
                );
            }
        }

        public void CrearEntrada(Abstracciones.ModeloUI.Asistencia asistencia)
        {
            using (var contexto = new Contexto())
            {
                var entidad = new AsistenciaAD
                {
                    IdEmpleado = asistencia.IdEmpleado,
                    Fecha = DateTime.SpecifyKind(asistencia.Fecha.Date, DateTimeKind.Utc),
                    HoraEntrada = asistencia.HoraEntrada,
                    TiempoAlmuerzo = asistencia.TiempoAlmuerzo,
                    HorasRegulares = asistencia.HorasRegulares,
                    HorasExtras = asistencia.HorasExtras,
                    HorasTotales = asistencia.HorasTotales
                };

                contexto.Asistencia.Add(entidad);
                contexto.SaveChanges();
            }
        }

        public Abstracciones.ModeloUI.Asistencia ObtenerEntradaAbierta(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                var entidad = contexto.Asistencia
                    .Where(a => a.IdEmpleado == idEmpleado && a.HoraSalida == null)
                    .OrderByDescending(a => a.Fecha)
                    .ThenByDescending(a => a.HoraEntrada)
                    .FirstOrDefault();

                return entidad != null ? ConvertirObjetoParaUI(entidad, contexto) : null;
            }
        }

        public void RegistrarSalida(int idAsistencia, TimeSpan horaSalida)
        {
            using (var contexto = new Contexto())
            {
                var asistencia = contexto.Asistencia.FirstOrDefault(a => a.IdAsistencia == idAsistencia);

                if (asistencia == null)
                    throw new Exception("No se encontró la asistencia para registrar salida.");

                if (asistencia.HoraSalida != null)
                    throw new Exception("Esta asistencia ya tiene salida registrada.");

                asistencia.HoraSalida = horaSalida;

                var tiempoTrabajado = asistencia.HoraSalida.Value - asistencia.HoraEntrada;
                var horasTrabajadas = (decimal)tiempoTrabajado.TotalHours - (asistencia.TiempoAlmuerzo / 60m);
                asistencia.HorasTotales = Math.Max(0, horasTrabajadas);

                if (asistencia.HorasTotales <= 8)
                {
                    asistencia.HorasRegulares = asistencia.HorasTotales;
                    asistencia.HorasExtras = 0;
                }
                else
                {
                    asistencia.HorasRegulares = 8;
                    asistencia.HorasExtras = asistencia.HorasTotales - 8;
                }

                contexto.SaveChanges();
            }
        }

    }

}


