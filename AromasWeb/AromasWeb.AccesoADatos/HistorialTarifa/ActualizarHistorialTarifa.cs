using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.HistorialTarifas
{
    public class ActualizarHistorialTarifa : IActualizarHistorialTarifa
    {
        private Contexto _contexto;

        public ActualizarHistorialTarifa()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.HistorialTarifa historialTarifa)
        {
            try
            {
                var historialExistente = _contexto.HistorialTarifa
                    .FirstOrDefault(h => h.IdHistorialTarifa == historialTarifa.IdHistorialTarifa);

                if (historialExistente == null)
                {
                    return 0;
                }

                // Validar que el empleado exista
                bool empleadoExiste = _contexto.Empleado
                    .Any(e => e.IdEmpleado == historialTarifa.IdEmpleado);
                if (!empleadoExiste)
                {
                    throw new Exception("El empleado especificado no existe");
                }

                // Validar que la tarifa sea mayor a 0
                if (historialTarifa.TarifaHora <= 0)
                {
                    throw new Exception("La tarifa por hora debe ser mayor a 0");
                }

                // Validar que FechaFin sea mayor a FechaInicio si está definida
                if (historialTarifa.FechaFin.HasValue &&
                    historialTarifa.FechaFin.Value <= historialTarifa.FechaInicio)
                {
                    throw new Exception("La fecha de fin debe ser mayor a la fecha de inicio");
                }

                // Convertir FechaInicio a UTC
                DateTime fechaInicioUtc = historialTarifa.FechaInicio;
                if (fechaInicioUtc.Kind == DateTimeKind.Unspecified)
                    fechaInicioUtc = DateTime.SpecifyKind(fechaInicioUtc, DateTimeKind.Utc);
                else if (fechaInicioUtc.Kind == DateTimeKind.Local)
                    fechaInicioUtc = fechaInicioUtc.ToUniversalTime();

                // Convertir FechaFin a UTC si tiene valor
                DateTime? fechaFinUtc = null;
                if (historialTarifa.FechaFin.HasValue)
                {
                    fechaFinUtc = historialTarifa.FechaFin.Value;
                    if (fechaFinUtc.Value.Kind == DateTimeKind.Unspecified)
                        fechaFinUtc = DateTime.SpecifyKind(fechaFinUtc.Value, DateTimeKind.Utc);
                    else if (fechaFinUtc.Value.Kind == DateTimeKind.Local)
                        fechaFinUtc = fechaFinUtc.Value.ToUniversalTime();
                }

                // Actualizar campos
                historialExistente.IdEmpleado = historialTarifa.IdEmpleado;
                historialExistente.TarifaHora = historialTarifa.TarifaHora;
                historialExistente.Motivo = historialTarifa.Motivo?.Trim();
                historialExistente.FechaInicio = fechaInicioUtc;
                historialExistente.FechaFin = fechaFinUtc;

                int cantidadDeDatosActualizados = _contexto.SaveChanges();
                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar historial de tarifa: {ex.Message}");
                throw;
            }
        }
    }
}