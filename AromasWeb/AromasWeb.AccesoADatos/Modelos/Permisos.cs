using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("Permiso")]
    public class PermisoAD
    {
        [Key]
        public int IdPermiso { get; set; }

        [Required]
        public int IdModulo { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        // Propiedades de navegación
        public virtual ModuloAD Modulo { get; set; }
    }
}