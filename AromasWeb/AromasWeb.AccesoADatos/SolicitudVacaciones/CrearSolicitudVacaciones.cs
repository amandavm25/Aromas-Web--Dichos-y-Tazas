using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.SolicitudesVacaciones
{
    public class CrearSolicitudVacaciones : ICrearSolicitudVacaciones
    {
        private Contexto _contexto;

        public CrearSolicitudVacaciones()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(Abstracciones.ModeloUI.SolicitudVacaciones solicitud)
        {
            try
            {
                // Convertir primero para usar en la validación de solapamiento
                DateTime fechaInicioUtc = EnsureUtc(solicitud.FechaInicio);
                DateTime fechaFinUtc = EnsureUtc(solicitud.FechaFin);

                bool empleadoExiste = await _contexto.Empleado
                    .AnyAsync(e => e.IdEmpleado == solicitud.IdEmpleado);
                if (!empleadoExiste)
                    throw new Exception("El empleado seleccionado no existe");

                if (fechaInicioUtc > fechaFinUtc)
                    throw new Exception("La fecha de inicio no puede ser mayor a la fecha de fin");

                // Usar fechas UTC en la comparación
                bool solicitudSolapada = await _contexto.SolicitudVacaciones
                    .AnyAsync(s => s.IdEmpleado == solicitud.IdEmpleado
                                && s.Estado != "Rechazada"
                                && s.Estado != "Cancelada"
                                && s.FechaInicio <= fechaFinUtc
                                && s.FechaFin >= fechaInicioUtc);
                if (solicitudSolapada)
                    throw new Exception("Ya existe una solicitud de vacaciones en ese período");

                SolicitudVacacionesAD solicitudAGuardar = ConvertirObjetoParaAD(solicitud);
                _contexto.SolicitudVacaciones.Add(solicitudAGuardar);
                return await _contexto.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar solicitud de vacaciones: {ex.Message}");
                throw;
            }
        }

        private SolicitudVacacionesAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.SolicitudVacaciones solicitud)
        {
            return new SolicitudVacacionesAD
            {
                IdEmpleado = solicitud.IdEmpleado,
                FechaSolicitud = EnsureUtc(solicitud.FechaSolicitud),
                FechaInicio = EnsureUtc(solicitud.FechaInicio),
                FechaFin = EnsureUtc(solicitud.FechaFin),
                DiasSolicitados = solicitud.DiasSolicitados,
                Estado = solicitud.Estado ?? "Pendiente",
                FechaRespuesta = null
            };
        }

        private DateTime EnsureUtc(DateTime fecha)
        {
            return DateTime.SpecifyKind(fecha, DateTimeKind.Utc);
        }
    }
}