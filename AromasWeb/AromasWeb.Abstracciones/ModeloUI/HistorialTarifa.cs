using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class HistorialTarifa
    {
        public int IdHistorialTarifa { get; set; }

        [DisplayName("Empleado")]
        [Required(ErrorMessage = "El empleado es requerido")]
        public int IdEmpleado { get; set; }

        [DisplayName("Tarifa por hora")]
        [Required(ErrorMessage = "La tarifa por hora es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La tarifa debe ser mayor a 0")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TarifaHora { get; set; }

        [DisplayName("Motivo del cambio")]
        [Required(ErrorMessage = "El motivo es requerido")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
        public string Motivo { get; set; }

        [DisplayName("Fecha de inicio de vigencia")]
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de fin de vigencia")]
        [DataType(DataType.Date)]
        public DateTime? FechaFin { get; set; }

        [DisplayName("Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        // Propiedades calculadas
        [DisplayName("Estado de vigencia")]
        public string EstadoVigencia
        {
            get
            {
                DateTime hoy = DateTime.Today;

                if (hoy < FechaInicio)
                    return "Futura";
                else if (FechaFin.HasValue && hoy > FechaFin.Value)
                    return "Vencida";
                else if (!FechaFin.HasValue || (hoy >= FechaInicio && hoy <= FechaFin.Value))
                    return "Vigente";
                else
                    return "Inactiva";
            }
        }

        [DisplayName("Es tarifa actual")]
        public bool EsTarifaActual
        {
            get
            {
                return EstadoVigencia == "Vigente";
            }
        }

        [DisplayName("Días de vigencia")]
        public int DiasVigencia
        {
            get
            {
                if (!FechaFin.HasValue)
                    return (DateTime.Today - FechaInicio).Days;

                return (FechaFin.Value - FechaInicio).Days;
            }
        }

        [DisplayName("Salario mensual estimado")]
        public decimal SalarioMensualEstimado
        {
            get
            {
                // Cálculo: 48 horas por semana * 4.33 semanas = 207.84 horas al mes
                decimal horasMensuales = 207.84m;
                return TarifaHora * horasMensuales;
            }
        }

        [DisplayName("Fecha de inicio")]
        public string FechaInicioFormateada
        {
            get
            {
                return FechaInicio.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Fecha de fin")]
        public string FechaFinFormateada
        {
            get
            {
                return FechaFin.HasValue ? FechaFin.Value.ToString("dd/MM/yyyy") : "Sin definir";
            }
        }

        [DisplayName("Fecha de registro")]
        public string FechaRegistroFormateada
        {
            get
            {
                return FechaRegistro.ToString("dd/MM/yyyy HH:mm");
            }
        }

        // Validación del salario mínimo legal
        [DisplayName("Cumple salario mínimo")]
        public bool CumpleSalarioMinimo
        {
            get
            {
                // Salario mínimo en Costa Rica 2025 (ejemplo): ₡367,640
                decimal salarioMinimoMensual = 367640m;
                decimal horasMensuales = 207.84m;
                decimal tarifaMinimaLegal = salarioMinimoMensual / horasMensuales;

                return TarifaHora >= tarifaMinimaLegal;
            }
        }

        [DisplayName("Tarifa mínima legal")]
        public decimal TarifaMinimaLegal
        {
            get
            {
                decimal salarioMinimoMensual = 367640m;
                decimal horasMensuales = 207.84m;
                return salarioMinimoMensual / horasMensuales;
            }
        }
    }
}