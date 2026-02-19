using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.SolicitudesVacaciones
{
    public class ActualizarSolicitudVacaciones : IActualizarSolicitudVacaciones
    {
        private Contexto _contexto;

        public ActualizarSolicitudVacaciones()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.SolicitudVacaciones solicitud)
        {
            try
            {
                var solicitudExistente = _contexto.SolicitudVacaciones
                    .FirstOrDefault(s => s.IdSolicitud == solicitud.IdSolicitud);

                if (solicitudExistente == null)
                {
                    return 0;
                }

                solicitudExistente.FechaInicio = EnsureUtc(solicitud.FechaInicio);
                solicitudExistente.FechaFin = EnsureUtc(solicitud.FechaFin);
                solicitudExistente.DiasSolicitados = solicitud.DiasSolicitados;
                solicitudExistente.Estado = solicitud.Estado;
                solicitudExistente.FechaRespuesta = solicitud.FechaRespuesta;

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar solicitud de vacaciones: {ex.Message}");
                throw;
            }
        }

        public int ActualizarEstado(int idSolicitud, string estado)
        {
            try
            {
                var solicitudExistente = _contexto.SolicitudVacaciones
                    .FirstOrDefault(s => s.IdSolicitud == idSolicitud);

                if (solicitudExistente == null)
                {
                    return 0;
                }

                solicitudExistente.Estado = estado;
                solicitudExistente.FechaRespuesta = DateTime.UtcNow;

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar estado de solicitud: {ex.Message}");
                throw;
            }
        }

        private DateTime EnsureUtc(DateTime fecha)
        {
            return DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
        }
    }
}