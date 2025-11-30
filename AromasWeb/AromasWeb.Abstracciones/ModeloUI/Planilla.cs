using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Planilla
    {
        public int IdPlanilla { get; set; }

        [DisplayName("Empleado")]
        [Required(ErrorMessage = "El empleado es requerido")]
        public int IdEmpleado { get; set; }

        [DisplayName("Período Inicio")]
        [Required(ErrorMessage = "La fecha de inicio del período es requerida")]
        [DataType(DataType.Date)]
        public DateTime PeriodoInicio { get; set; }

        [DisplayName("Período Fin")]
        [Required(ErrorMessage = "La fecha de fin del período es requerida")]
        [DataType(DataType.Date)]
        public DateTime PeriodoFin { get; set; }

        [DisplayName("Tarifa por hora")]
        [Required(ErrorMessage = "La tarifa por hora es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La tarifa debe ser mayor a 0")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TarifaHora { get; set; }

        [DisplayName("Total Horas Regulares")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal TotalHorasRegulares { get; set; }

        [DisplayName("Total Horas Extras")]
        [DisplayFormat(DataFormatString = "{0:N2}", ApplyFormatInEditMode = false)]
        public decimal TotalHorasExtras { get; set; }

        [DisplayName("Pago Horas Regulares")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal PagoHorasRegulares { get; set; }

        [DisplayName("Pago Horas Extras")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal PagoHorasExtras { get; set; }

        [DisplayName("Pago Bruto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal PagoBruto { get; set; }

        [DisplayName("Deducción CCSS (10.67%)")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal DeduccionCCSS { get; set; }

        [DisplayName("Impuesto Renta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal ImpuestoRenta { get; set; }

        [DisplayName("Otras Deducciones")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal OtrasDeducciones { get; set; }

        [DisplayName("Total Deducciones")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal TotalDeducciones { get; set; }

        [DisplayName("Pago Neto")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal PagoNeto { get; set; }

        [DisplayName("Estado")]
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20, ErrorMessage = "El estado no puede exceder 20 caracteres")]
        public string Estado { get; set; } = "Calculado"; // Calculado, Pagado, Anulado

        // Propiedades de navegación
        [DisplayName("Empleado")]
        public string NombreEmpleado { get; set; }

        [DisplayName("Identificación")]
        public string IdentificacionEmpleado { get; set; }

        [DisplayName("Cargo")]
        public string CargoEmpleado { get; set; }

        // Propiedades calculadas
        [DisplayName("Período")]
        public string PeriodoFormateado
        {
            get
            {
                return $"{PeriodoInicio:dd/MM/yyyy} - {PeriodoFin:dd/MM/yyyy}";
            }
        }

        [DisplayName("Duración del período")]
        public int DiasPeriodo
        {
            get
            {
                return (PeriodoFin - PeriodoInicio).Days + 1;
            }
        }

        [DisplayName("Total horas trabajadas")]
        public decimal TotalHorasTrabajadas
        {
            get
            {
                return TotalHorasRegulares + TotalHorasExtras;
            }
        }

        [DisplayName("Estado badge")]
        public string EstadoBadge
        {
            get
            {
                return Estado switch
                {
                    "Calculado" => "warning",
                    "Pagado" => "success",
                    "Anulado" => "danger",
                    _ => "secondary"
                };
            }
        }

        [DisplayName("Porcentaje deducciones")]
        public decimal PorcentajeDeducciones
        {
            get
            {
                if (PagoBruto == 0) return 0;
                return (TotalDeducciones / PagoBruto) * 100;
            }
        }

        // Método para calcular impuesto sobre la renta (escala progresiva CR 2025)
        public static decimal CalcularImpuestoRenta(decimal salarioBruto)
        {
            decimal impuesto = 0;

            // Escala progresiva Costa Rica
            if (salarioBruto <= 941000m)
            {
                impuesto = 0; // Exento
            }
            else if (salarioBruto <= 1381000m)
            {
                impuesto = (salarioBruto - 941000m) * 0.10m;
            }
            else if (salarioBruto <= 2423000m)
            {
                impuesto = (440000m * 0.10m) + ((salarioBruto - 1381000m) * 0.15m);
            }
            else if (salarioBruto <= 4845000m)
            {
                impuesto = (440000m * 0.10m) + (1042000m * 0.15m) + ((salarioBruto - 2423000m) * 0.20m);
            }
            else
            {
                impuesto = (440000m * 0.10m) + (1042000m * 0.15m) + (2422000m * 0.20m) + ((salarioBruto - 4845000m) * 0.25m);
            }

            return Math.Round(impuesto, 2);
        }

        // Método para calcular CCSS
        public static decimal CalcularCCSS(decimal salarioBruto)
        {
            return Math.Round(salarioBruto * 0.1067m, 2);
        }
    }
}