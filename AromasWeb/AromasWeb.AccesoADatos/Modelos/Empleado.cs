using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("Empleado")]
    public class EmpleadoAD
    {
        [Key]
        public int IdEmpleado { get; set; }

        [Required]
        public int IdRol { get; set; }

        [Required]
        [StringLength(20)]
        public string Identificacion { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(200)]
        public string Apellidos { get; set; }

        [Required]
        [StringLength(100)]
        public string Correo { get; set; }

        [Required]
        [StringLength(20)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(100)]
        public string Cargo { get; set; }

        [Required]
        public DateTime FechaContratacion { get; set; }

        [StringLength(255)]
        public string Contrasena { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Propiedades de navegación
        public virtual RolAD Rol { get; set; }

        [NotMapped]
        public decimal TarifaHora { get; set; }
    }
}