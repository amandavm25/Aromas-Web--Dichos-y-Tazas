using System;
using System.ComponentModel;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class RolPermisos
    {
        [DisplayName("ID")]
        public int IdRolPermiso { get; set; }

        [DisplayName("Rol")]
        public int IdRol { get; set; }

        [DisplayName("Permiso")]
        public int IdPermiso { get; set; }

        // Propiedades de navegación
        public virtual Rol Rol { get; set; }
        public virtual Permiso Permiso { get; set; }

        // Propiedades calculadas
        [DisplayName("Rol")]
        public string NombreRol
        {
            get
            {
                return Rol?.Nombre ?? "Sin rol";
            }
        }

        [DisplayName("Permiso")]
        public string NombrePermiso
        {
            get
            {
                return Permiso?.Nombre ?? "Sin permiso";
            }
        }

        [DisplayName("Módulo")]
        public string NombreModulo
        {
            get
            {
                return Permiso?.Modulo?.Nombre ?? "Sin módulo";
            }
        }
    }
}