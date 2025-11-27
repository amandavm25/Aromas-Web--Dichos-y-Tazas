using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Receta
    {
        public int IdReceta { get; set; }

        [DisplayName("Categoría de receta")]
        [Required(ErrorMessage = "La categoría es requerida")]
        public int IdCategoriaReceta { get; set; }

        [DisplayName("Nombre de la receta")]
        [Required(ErrorMessage = "El nombre de la receta es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Cantidad de porciones")]
        [Required(ErrorMessage = "La cantidad de porciones es requerida")]
        [Range(1, 1000, ErrorMessage = "La cantidad debe estar entre 1 y 1000")]
        public int CantidadPorciones { get; set; }

        [DisplayName("Pasos de preparación")]
        [Required(ErrorMessage = "Los pasos de preparación son requeridos")]
        public string PasosPreparacion { get; set; }

        [DisplayName("Precio de venta")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio debe ser mayor o igual a 0")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal? PrecioVenta { get; set; }

        [DisplayName("Costo total")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal CostoTotal { get; set; }

        [DisplayName("Costo por porción")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal CostoPorcion { get; set; }

        [DisplayName("Ganancia neta")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal GananciaNeta { get; set; }

        [DisplayName("Porcentaje de margen")]
        [DisplayFormat(DataFormatString = "{0:N2}%", ApplyFormatInEditMode = false)]
        public decimal PorcentajeMargen { get; set; }

        [DisplayName("Disponibilidad")]
        public bool Disponibilidad { get; set; }

        [DisplayName("Imagen")]
        [StringLength(500)]
        public string Imagen { get; set; }

        // Lista de ingredientes
        public List<RecetaInsumo> Ingredientes { get; set; } = new List<RecetaInsumo>();

        // Propiedades calculadas
        [DisplayName("Categoría")]
        public string NombreCategoria { get; set; }

        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Disponibilidad ? "Disponible" : "No disponible";
            }
        }

        [DisplayName("Rentabilidad")]
        public string NivelRentabilidad
        {
            get
            {
                if (PorcentajeMargen >= 50)
                    return "Alta";
                else if (PorcentajeMargen >= 30)
                    return "Media";
                else
                    return "Baja";
            }
        }

        public bool TienePrecioVenta
        {
            get
            {
                return PrecioVenta.HasValue && PrecioVenta.Value > 0;
            }
        }
    }
}