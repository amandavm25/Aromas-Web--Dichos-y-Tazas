using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class CategoriaInsumo
    {
        public int IdCategoria { get; set; }

        [DisplayName("Nombre de la categoría")]
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NombreCategoria { get; set; }

        [DisplayName("Descripción")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
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
    }
}