using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Asistencia
    {
        public int IdAsistencia { get; set; }

        [DisplayName("Empleado")]
        [Required(ErrorMessage = "El empleado es requerido")]
        public int IdEmpleado { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DisplayName("Hora de entrada")]
        [Required(ErrorMessage = "La hora de entrada es requerida")]
        [DataType(DataType.Time)]
        public TimeSpan HoraEntrada { get; set; }

        [DisplayName("Hora de salida")]
        [DataType(DataType.Time)]
        public TimeSpan? HoraSalida { get; set; }

        [DisplayName("Tiempo de almuerzo (minutos)")]
        [Range(0, 120, ErrorMessage = "El tiempo de almuerzo debe estar entre 0 y 120 minutos")]
        public int TiempoAlmuerzo { get; set; }

        [DisplayName("Horas regulares")]
        public decimal HorasRegulares { get; set; }

        [DisplayName("Horas extras")]
        public decimal HorasExtras { get; set; }

        [DisplayName("Horas totales")]
        public decimal HorasTotales { get; set; }

        // Propiedades de navegación
        [DisplayName("Empleado")]
        public string? NombreEmpleado { get; set; }

        [DisplayName("Identificación")]
        public string? IdentificacionEmpleado { get; set; }

        [DisplayName("Cargo")]
        public string? CargoEmpleado { get; set; }

        // Propiedades calculadas
        [DisplayName("Fecha")]
        public string FechaFormateada
        {
            get
            {
                return Fecha.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Día de la semana")]
        public string DiaSemana
        {
            get
            {
                var dias = new[] { "Domingo", "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado" };
                return dias[(int)Fecha.DayOfWeek];
            }
        }

        [DisplayName("Hora de entrada")]
        public string HoraEntradaFormateada
        {
            get
            {
                return HoraEntrada.ToString(@"hh\:mm");
            }
        }

        [DisplayName("Hora de salida")]
        public string HoraSalidaFormateada
        {
            get
            {
                return HoraSalida?.ToString(@"hh\:mm") ?? "Pendiente";
            }
        }

        [DisplayName("Estado del registro")]
        public string EstadoRegistro
        {
            get
            {
                return HoraSalida.HasValue ? "Completo" : "En curso";
            }
        }

        [DisplayName("Tiempo de almuerzo")]
        public string TiempoAlmuerzoFormateado
        {
            get
            {
                if (TiempoAlmuerzo == 0)
                    return "Sin almuerzo";

                var horas = TiempoAlmuerzo / 60;
                var minutos = TiempoAlmuerzo % 60;

                if (horas > 0 && minutos > 0)
                    return $"{horas}h {minutos}min";
                else if (horas > 0)
                    return $"{horas}h";
                else
                    return $"{minutos}min";
            }
        }

        [DisplayName("Horas trabajadas")]
        public string HorasTrabajadasTexto
        {
            get
            {
                if (!HoraSalida.HasValue)
                    return "En curso";

                var horas = (int)HorasTotales;
                var minutos = (int)((HorasTotales - horas) * 60);

                return $"{horas}h {minutos}min";
            }
        }

        [DisplayName("Desglose de horas")]
        public string DesgloseHoras
        {
            get
            {
                if (!HoraSalida.HasValue)
                    return "Pendiente";

                return $"Regulares: {HorasRegulares:F2}h | Extras: {HorasExtras:F2}h";
            }
        }

        [DisplayName("Tiene horas extras")]
        public bool TieneHorasExtras
        {
            get
            {
                return HorasExtras > 0;
            }
        }

        // Método para calcular horas trabajadas
        public void CalcularHoras()
        {
            if (!HoraSalida.HasValue)
            {
                HorasTotales = 0;
                HorasRegulares = 0;
                HorasExtras = 0;
                return;
            }

            // Calcular horas totales trabajadas
            var tiempoTrabajado = HoraSalida.Value - HoraEntrada;
            var horasTrabajadas = (decimal)tiempoTrabajado.TotalHours - (TiempoAlmuerzo / 60m);

            HorasTotales = Math.Max(0, horasTrabajadas);

            // Calcular horas regulares y extras
            if (HorasTotales <= 8)
            {
                HorasRegulares = HorasTotales;
                HorasExtras = 0;
            }
            else
            {
                HorasRegulares = 8;
                HorasExtras = HorasTotales - 8;
            }
        }
    }
}