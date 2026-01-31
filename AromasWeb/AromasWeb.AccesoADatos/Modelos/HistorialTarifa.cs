using System;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class HistorialTarifaAD
    {
        public int IdHistorialTarifa { get; set; }
        public int IdEmpleado { get; set; }
        public decimal TarifaHora { get; set; }
        public string Motivo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public DateTime FechaRegistro { get; set; }

        // Propiedades de navegación
        public virtual EmpleadoAD Empleado { get; set; }
    }
}