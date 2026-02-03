using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("detalleplanilla")]
    public class DetallePlanillaAD
    {
        [Key]
        [Column("iddetalleplanilla")]
        public int IdDetallePlanilla { get; set; }

        [Required]
        [Column("idplanilla")]
        public int IdPlanilla { get; set; }

        [Required]
        [Column("idasistencia")]
        public int IdAsistencia { get; set; }

        [Required]
        [Column("fecha")]
        public DateTime Fecha { get; set; }

        [Required]
        [Column("horasregulares")]
        public decimal HorasRegulares { get; set; }

        [Required]
        [Column("horasextras")]
        public decimal HorasExtras { get; set; }

        [Required]
        [Column("subtotal")]
        public decimal Subtotal { get; set; }

        // Propiedades de navegación
        public virtual PlanillaAD Planilla { get; set; }
        public virtual AsistenciaAD Asistencia { get; set; }
    }
}