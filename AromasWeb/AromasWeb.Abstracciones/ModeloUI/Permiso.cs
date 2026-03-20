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

        [DisplayName("Descripción")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; } = true;

        // Propiedad de navegación
        public virtual Modulo? Modulo { get; set; }

        // Propiedades calculadas
        [DisplayName("Módulo")]
        public string NombreModulo
        {
            get { return Modulo?.Nombre ?? "Sin módulo"; }
        }

        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get { return Estado ? "Activo" : "Inactivo"; }
        }
    }
}