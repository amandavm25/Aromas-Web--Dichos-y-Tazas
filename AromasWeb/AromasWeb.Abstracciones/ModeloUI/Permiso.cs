using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Permiso
    {
        public int IdPermiso { get; set; }

        [DisplayName("Módulo")]
        [Required(ErrorMessage = "El módulo es requerido")]
        public int IdModulo { get; set; }

        [DisplayName("Nombre del Permiso")]
        [Required(ErrorMessage = "El nombre del permiso es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        // Propiedades de navegación
        public virtual Modulo Modulo { get; set; }

        // Propiedades calculadas
        [DisplayName("Módulo")]
        public string NombreModulo
        {
            get
            {
                return Modulo?.Nombre ?? "Sin módulo";
            }
        }
    }
}