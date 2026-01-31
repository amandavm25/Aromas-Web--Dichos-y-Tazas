using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.SolicitudesVacaciones
{
    public class ListarSolicitudesVacaciones : IListarSolicitudesVacaciones
    {
        public List<Abstracciones.ModeloUI.SolicitudVacaciones> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<SolicitudVacacionesAD> solicitudesAD = contexto.SolicitudVacaciones
                        .OrderByDescending(s => s.FechaSolicitud)
                        .ToList();
                    return solicitudesAD.Select(s => ConvertirObjetoParaUI(s, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener solicitudes: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.SolicitudVacaciones> BuscarPorEmpleado(int idEmpleado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<SolicitudVacacionesAD> solicitudesAD = contexto.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == idEmpleado)
                        .OrderByDescending(s => s.FechaSolicitud)
                        .ToList();
                    return solicitudesAD.Select(s => ConvertirObjetoParaUI(s, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar solicitudes por empleado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.SolicitudVacaciones> BuscarPorEstado(string estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<SolicitudVacacionesAD> solicitudesAD = contexto.SolicitudVacaciones
                        .Where(s => s.Estado == estado)
                        .OrderByDescending(s => s.FechaSolicitud)
                        .ToList();
                    return solicitudesAD.Select(s => ConvertirObjetoParaUI(s, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar solicitudes por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.SolicitudVacaciones ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var solicitudAD = contexto.SolicitudVacaciones.FirstOrDefault(s => s.IdSolicitud == id);
                    return solicitudAD != null ? ConvertirObjetoParaUI(solicitudAD, contexto) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener solicitud por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.SolicitudVacaciones ConvertirObjetoParaUI(SolicitudVacacionesAD solicitudAD, Contexto contexto)
        {
            string nombreEmpleado = "Desconocido";
            string identificacionEmpleado = "";
            string cargoEmpleado = "";
            DateTime fechaContratacionEmpleado = DateTime.Now;
            decimal diasDisponibles = 0;
            decimal diasTomados = 0;

            try
            {
                var empleado = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == solicitudAD.IdEmpleado);
                if (empleado != null)
                {
                    nombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                    identificacionEmpleado = empleado.Identificacion;
                    cargoEmpleado = empleado.Cargo;
                    fechaContratacionEmpleado = empleado.FechaContratacion;

                    // Calcular días disponibles
                    var mesesTrabajados = (int)((DateTime.Now - empleado.FechaContratacion).TotalDays / 30);
                    var diasAcumulados = mesesTrabajados;

                    // Calcular días tomados (solicitudes aprobadas)
                    diasTomados = contexto.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == solicitudAD.IdEmpleado && s.Estado == "Aprobada")
                        .Sum(s => s.DiasSolicitados);

                    diasDisponibles = diasAcumulados - diasTomados;
                }
            }
            catch
            {
                // Si hay error, usar valores por defecto
            }

            return new Abstracciones.ModeloUI.SolicitudVacaciones
            {
                IdSolicitud = solicitudAD.IdSolicitud,
                IdEmpleado = solicitudAD.IdEmpleado,
                FechaSolicitud = solicitudAD.FechaSolicitud,
                FechaInicio = solicitudAD.FechaInicio,
                FechaFin = solicitudAD.FechaFin,
                DiasSolicitados = solicitudAD.DiasSolicitados,
                Estado = solicitudAD.Estado,
                FechaRespuesta = solicitudAD.FechaRespuesta,
                NombreEmpleado = nombreEmpleado,
                IdentificacionEmpleado = identificacionEmpleado,
                CargoEmpleado = cargoEmpleado,
                FechaContratacionEmpleado = fechaContratacionEmpleado,
                DiasDisponibles = diasDisponibles,
                DiasTomados = diasTomados
            };
        }
    }
}