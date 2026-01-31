using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class PlanillaAD
    {
        public int IdPlanilla { get; set; }

        public int IdEmpleado { get; set; }

        public DateTime PeriodoInicio { get; set; }

        public DateTime PeriodoFin { get; set; }

        public decimal TarifaHora { get; set; }

        public decimal TotalHorasRegulares { get; set; }

        public decimal TotalHorasExtras { get; set; }

        public decimal PagoHorasRegulares { get; set; }

        public decimal PagoHorasExtras { get; set; }

        public decimal PagoBruto { get; set; }

        public decimal DeduccionCCSS { get; set; }

        public decimal ImpuestoRenta { get; set; }

        public decimal OtrasDeducciones { get; set; }

        public decimal TotalDeducciones { get; set; }

        public decimal PagoNeto { get; set; }

        public string Estado { get; set; } = "Calculado"; // Calculado, Pagado, Anulado

        // Propiedades de navegación
        public virtual EmpleadoAD Empleado { get; set; }

    }
}