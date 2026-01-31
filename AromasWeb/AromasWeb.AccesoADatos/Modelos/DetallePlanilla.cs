using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("detalleplanilla")]
    public class DetallePlanillaAD
    {
        [Key]
        public int IdDetallePlanilla { get; set; }

        [Required]
        public int IdPlanilla { get; set; }

        [Required]
        public int IdAsistencia { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan HoraEntrada { get; set; }

        [Required]
        public TimeSpan HoraSalida { get; set; }

        [Required]
        public TimeSpan TiempoAlmuerzo { get; set; }

        [Required]
        public decimal HorasRegulares { get; set; }

        [Required]
        public decimal HorasExtras { get; set; }

        [Required]
        public decimal Subtotal { get; set; }

        // Propiedades de navegación
        public virtual PlanillaAD Planilla { get; set; }
        public virtual AsistenciaAD Asistencia { get; set; }
    }
}