using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Insumo
    {
        public int IdInsumo { get; set; }

        [DisplayName("Nombre del insumo")]
        [Required(ErrorMessage = "El nombre del insumo es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string NombreInsumo { get; set; }

        [DisplayName("Unidad de medida")]
        [Required(ErrorMessage = "La unidad de medida es requerida")]
        [StringLength(50, ErrorMessage = "La unidad no puede exceder 50 caracteres")]
        public string UnidadMedida { get; set; }

        [DisplayName("Categoría")]
        [Required(ErrorMessage = "La categoría es requerida")]
        public int IdCategoria { get; set; }

        [DisplayName("Costo unitario")]
        [Required(ErrorMessage = "El costo unitario es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El costo debe ser mayor a 0")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal CostoUnitario { get; set; }

        [DisplayName("Cantidad disponible")]
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        public decimal CantidadDisponible { get; set; }

        [DisplayName("Stock mínimo")]
        [Required(ErrorMessage = "El stock mínimo es requerido")]
        [Range(0, int.MaxValue, ErrorMessage = "El stock mínimo debe ser mayor o igual a 0")]
        public decimal StockMinimo { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        [DisplayName("Fecha de Creación")]
        public DateTime FechaCreacion { get; set; }

        [DisplayName("Fecha de Actualización")]
        public DateTime FechaActualizacion { get; set; }

        // Propiedades calculadas
        [DisplayName("Valor total")]
        public decimal ValorTotal
        {
            get
            {
                return CostoUnitario * CantidadDisponible;
            }
        }

        [DisplayName("Estado del stock")]
        public string EstadoStock
        {
            get
            {
                if (CantidadDisponible <= StockMinimo)
                    return "Bajo";
                else if (CantidadDisponible <= StockMinimo * 1.5m)
                    return "Medio";
                else
                    return "Normal";
            }
        }

        [DisplayName("Requiere alerta")]
        public bool RequiereAlerta
        {
            get
            {
                return CantidadDisponible <= StockMinimo;
            }
        }

        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Estado ? "Activo" : "Inactivo";
            }
        }

        [DisplayName("Fecha de Creación")]
        public string FechaCreacionFormateada
        {
            get
            {
                return FechaCreacion.ToString("dd/MM/yyyy HH:mm");
            }
        }

        [DisplayName("Fecha de Actualización")]
        public string FechaActualizacionFormateada
        {
            get
            {
                return FechaActualizacion.ToString("dd/MM/yyyy HH:mm");
            }
        }
    }
}