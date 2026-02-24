using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;
using HistorialTarifaUI = AromasWeb.Abstracciones.ModeloUI.HistorialTarifa;

namespace AromasWeb.AccesoADatos.HistorialTarifas
{
    public class CrearHistorialTarifa : ICrearHistorialTarifa
    {
        private Contexto _contexto;

        public CrearHistorialTarifa()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(HistorialTarifaUI historialTarifa)
        {
            try
            {
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

                var historialAD = ConvertirObjetoParaAD(historialTarifa);

                _contexto.HistorialTarifa.Add(historialAD);

                int cantidadDeDatosInsertados = await _contexto.SaveChangesAsync();
                return cantidadDeDatosInsertados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear historial de tarifa: {ex.Message}");
                throw;
            }
        }

        private HistorialTarifaAD ConvertirObjetoParaAD(HistorialTarifaUI historialTarifa)
        {
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

            // Convertir FechaRegistro a UTC
            DateTime fechaRegistroUtc = historialTarifa.FechaRegistro;
            if (fechaRegistroUtc == DateTime.MinValue)
                fechaRegistroUtc = DateTime.UtcNow;
            else if (fechaRegistroUtc.Kind == DateTimeKind.Unspecified)
                fechaRegistroUtc = DateTime.SpecifyKind(fechaRegistroUtc, DateTimeKind.Utc);
            else if (fechaRegistroUtc.Kind == DateTimeKind.Local)
                fechaRegistroUtc = fechaRegistroUtc.ToUniversalTime();

            return new HistorialTarifaAD
            {
                IdEmpleado = historialTarifa.IdEmpleado,
                TarifaHora = historialTarifa.TarifaHora,
                Motivo = historialTarifa.Motivo?.Trim(),
                FechaInicio = fechaInicioUtc,
                FechaFin = fechaFinUtc,
                FechaRegistro = fechaRegistroUtc
            };
        }
    }
}