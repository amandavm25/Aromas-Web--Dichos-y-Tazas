using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.Abstracciones.Logica.Insumos
{
    public class Insumo
    { 
        public int idInsumo { get; set; }

        [DisplayName("Nombre del Insumo")]
        [Required(ErrorMessage = "El nombre es obligatorio")]
        public string Nombre { get; set; }

        [DisplayName("Unidad de medida")]
        [Required(ErrorMessage = "La unidad de medida es obligatoria")]
        public string UnidadMedida { get; set; }

        [DisplayName("Categoría")]
        [Required(ErrorMessage = "La categoría es obligatoria")]
        public string Categoria { get; set; }

        [DisplayName("Costo unitario")]
        [Required(ErrorMessage = "El costo unitario es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
        public decimal CostoUnitario { get; set; }

        [DisplayName("Cantidad inicial")]
        [Required(ErrorMessage = "La cantidad es obligatoria")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad no puede ser negativa")]
        public int Cantidad { get; set; }
    }

}
