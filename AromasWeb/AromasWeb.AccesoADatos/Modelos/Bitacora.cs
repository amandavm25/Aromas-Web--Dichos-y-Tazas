using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("Bitacora")]
    public class BitacoraAD
    {
        [Key]
        public int IdBitacora { get; set; }

        [Required]
        public int IdEmpleado { get; set; }

        [Required]
        public int IdModulo { get; set; }

        [Required]
        [StringLength(200)]
        public string Accion { get; set; }

        [StringLength(100)]
        public string TablaAfectada { get; set; }

        [StringLength(1000)]
        public string Descripcion { get; set; }

        public string DatosAnteriores { get; set; }

        public string DatosNuevos { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        // Propiedades de navegación
        public virtual EmpleadoAD Empleado { get; set; }
        public virtual ModuloAD Modulo { get; set; }
    }
}