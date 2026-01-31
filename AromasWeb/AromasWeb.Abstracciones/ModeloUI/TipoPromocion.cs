using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class TipoPromocion
    {
        public int IdTipoPromocion { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "El nombre del tipo de promoción es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        [StringLength(300, ErrorMessage = "La descripción no puede exceder 300 caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        // Propiedades calculadas
        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Estado ? "Activo" : "Inactivo";
            }
        }

        // Para mostrar cantidad de promociones asociadas
        public int CantidadPromociones { get; set; }
    }
}