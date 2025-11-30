using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class DetallePlanilla
    {
        public int IdDetallePlanilla { get; set; }

        [DisplayName("Planilla")]
        [Required(ErrorMessage = "La planilla es requerida")]
        public int IdPlanilla { get; set; }

        [DisplayName("Asistencia")]
        [Required(ErrorMessage = "La asistencia es requerida")]
        public int IdAsistencia { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DisplayName("Horas Regulares")]
        [Required(ErrorMessage = "Las horas regulares son requeridas")]
        [Range(0, 24, ErrorMessage = "Las horas regulares deben estar entre 0 y 24")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal HorasRegulares { get; set; }

        [DisplayName("Horas Extras")]
        [Range(0, 24, ErrorMessage = "Las horas extras deben estar entre 0 y 24")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal HorasExtras { get; set; }

        [DisplayName("Subtotal")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Subtotal { get; set; }

        // Propiedades de navegación
        [DisplayName("Día de la semana")]
        public string DiaSemana { get; set; }

        [DisplayName("Hora entrada")]
        public TimeSpan? HoraEntrada { get; set; }

        [DisplayName("Hora salida")]
        public TimeSpan? HoraSalida { get; set; }

        [DisplayName("Tiempo almuerzo")]
        public TimeSpan? TiempoAlmuerzo { get; set; }

        // Propiedades calculadas
        [DisplayName("Fecha formateada")]
        public string FechaFormateada
        {
            get
            {
                return Fecha.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Día de la semana")]
        public string DiaSemanaCalculado
        {
            get
            {
                return Fecha.ToString("dddd", new System.Globalization.CultureInfo("es-ES"));
            }
        }

        [DisplayName("Total horas del día")]
        public decimal TotalHorasDia
        {
            get
            {
                return HorasRegulares + HorasExtras;
            }
        }

        [DisplayName("Hora entrada formateada")]
        public string HoraEntradaFormateada
        {
            get
            {
                return HoraEntrada.HasValue ? HoraEntrada.Value.ToString(@"hh\:mm") : "-";
            }
        }

        [DisplayName("Hora salida formateada")]
        public string HoraSalidaFormateada
        {
            get
            {
                return HoraSalida.HasValue ? HoraSalida.Value.ToString(@"hh\:mm") : "-";
            }
        }

        [DisplayName("Tiempo almuerzo formateado")]
        public string TiempoAlmuerzoFormateado
        {
            get
            {
                return TiempoAlmuerzo.HasValue ? $"{TiempoAlmuerzo.Value.TotalMinutes} min" : "-";
            }
        }

        [DisplayName("Es día con horas extras")]
        public bool TieneHorasExtras
        {
            get
            {
                return HorasExtras > 0;
            }
        }

        [DisplayName("Es fin de semana")]
        public bool EsFinDeSemana
        {
            get
            {
                return Fecha.DayOfWeek == DayOfWeek.Saturday || Fecha.DayOfWeek == DayOfWeek.Sunday;
            }
        }

        // Método para calcular el subtotal del día
        public static decimal CalcularSubtotalDia(decimal horasRegulares, decimal horasExtras, decimal tarifaHora)
        {
            decimal pagoRegular = horasRegulares * tarifaHora;
            decimal pagoExtra = horasExtras * tarifaHora * 1.5m; // Horas extras al 150%
            return Math.Round(pagoRegular + pagoExtra, 2);
        }

        // Método para calcular horas desde asistencia
        public static (decimal horasRegulares, decimal horasExtras) CalcularHorasDesdeAsistencia(
            TimeSpan horaEntrada,
            TimeSpan horaSalida,
            TimeSpan tiempoAlmuerzo)
        {
            // Calcular total de horas trabajadas
            var tiempoTotal = horaSalida - horaEntrada - tiempoAlmuerzo;
            decimal horasTotales = (decimal)tiempoTotal.TotalHours;

            // Si trabajó 8 horas o menos, todo son horas regulares
            if (horasTotales <= 8)
            {
                return (horasTotales, 0);
            }
            // Si trabajó más de 8 horas, las primeras 8 son regulares y el resto extras
            else
            {
                return (8, horasTotales - 8);
            }
        }
    }
}