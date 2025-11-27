using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class MovimientoInsumo
    {
        public int IdMovimiento { get; set; }

        [DisplayName("Insumo")]
        [Required(ErrorMessage = "El insumo es requerido")]
        public int IdInsumo { get; set; }

        [DisplayName("Tipo de movimiento")]
        [Required(ErrorMessage = "El tipo de movimiento es requerido")]
        public string TipoMovimiento { get; set; } // "Entrada" o "Salida"

        [DisplayName("Cantidad")]
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0.01, double.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }

        [DisplayName("Motivo")]
        [Required(ErrorMessage = "El motivo es requerido")]
        [StringLength(500, ErrorMessage = "El motivo no puede exceder 500 caracteres")]
        public string Motivo { get; set; }

        [DisplayName("Costo unitario al momento del movimiento")]
        public decimal CostoUnitario { get; set; }

        [DisplayName("Usuario")]
        public int IdUsuario { get; set; }

        [DisplayName("Fecha del movimiento")]
        [Required(ErrorMessage = "La fecha es requerida")]
        public DateTime FechaMovimiento { get; set; }

        // Propiedades calculadas
        [DisplayName("Valor del movimiento")]
        public decimal ValorMovimiento
        {
            get
            {
                return Cantidad * CostoUnitario;
            }
        }

        [DisplayName("Tipo")]
        public string TipoMovimientoTexto
        {
            get
            {
                return TipoMovimiento == "E" ? "Entrada" : "Salida";
            }
        }

        [DisplayName("Fecha")]
        public string FechaMovimientoFormateada
        {
            get
            {
                return FechaMovimiento.ToString("dd/MM/yyyy HH:mm");
            }
        }

        // Propiedad para mostrar el símbolo según el tipo
        public string SimboloMovimiento
        {
            get
            {
                return TipoMovimiento == "E" ? "+" : "-";
            }
        }
    }
}