using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class RecetaInsumo
    {
        public int IdRecetaInsumo { get; set; }

        [DisplayName("Insumo")]
        [Required(ErrorMessage = "El insumo es requerido")]
        public int IdInsumo { get; set; }

        [DisplayName("Receta")]
        [Required(ErrorMessage = "La receta es requerida")]
        public int IdReceta { get; set; }

        [DisplayName("Cantidad utilizada")]
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal CantidadUtilizada { get; set; }

        [DisplayName("Costo unitario")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal CostoUnitario { get; set; }

        [DisplayName("Costo total ingrediente")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal CostoTotalIngrediente { get; set; }

        // Propiedades de navegación
        [DisplayName("Nombre del insumo")]
        public string NombreInsumo { get; set; }

        [DisplayName("Unidad de medida")]
        public string UnidadMedida { get; set; }

        [DisplayName("Disponible en inventario")]
        public decimal CantidadDisponible { get; set; }

        // Propiedades calculadas
        public bool EsSuficiente
        {
            get
            {
                return CantidadDisponible >= CantidadUtilizada;
            }
        }

        public string EstadoDisponibilidad
        {
            get
            {
                if (CantidadDisponible >= CantidadUtilizada)
                    return "Suficiente";
                else if (CantidadDisponible > 0)
                    return "Insuficiente";
                else
                    return "Sin stock";
            }
        }
    }
}