using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("rolpermiso")]
    public class RolPermisoAD
    {
        [Key]
        public int IdRolPermiso { get; set; }

        [Required]
        public int IdRol { get; set; }

        [Required]
        public int IdPermiso { get; set; }

        // Propiedades de navegación
        public virtual PermisoAD Permiso { get; set; }
        public virtual RolAD Rol { get; set; }
    }
}