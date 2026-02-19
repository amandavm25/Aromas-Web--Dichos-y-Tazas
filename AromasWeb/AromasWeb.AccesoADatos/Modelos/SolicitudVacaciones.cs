using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("SolicitudVacaciones")]
    public class SolicitudVacacionesAD
    {
        [Key]
        public int IdSolicitud { get; set; }

        [Required]
        public int IdEmpleado { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public int DiasSolicitados { get; set; }

        [Required]
        [StringLength(50)]
        public string Estado { get; set; }

        public DateTime? FechaRespuesta { get; set; }

        // Propiedades de navegación
        public virtual EmpleadoAD Empleado { get; set; }
    }
}