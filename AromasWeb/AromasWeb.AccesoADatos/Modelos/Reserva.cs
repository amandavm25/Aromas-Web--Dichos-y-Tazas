using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("Reserva")]
    public class ReservaAD
    {
        [Key]
        public int IdReserva { get; set; }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        [Range(1, 20)]
        public int CantidadPersonas { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public TimeSpan Hora { get; set; }

        [Required]
        [StringLength(20)]
        public string Estado { get; set; }

        [StringLength(500)]
        public string Observaciones { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        // Propiedades de navegación
        public virtual ClienteAD Cliente { get; set; }
    }
}