using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("MovimientoInsumo")]
    public class MovimientoInsumoAD
    {
        [Key]
        public int IdMovimiento { get; set; }

        [Required]
        public int IdInsumo { get; set; }

        [Required]
        [StringLength(1)]
        public string TipoMovimiento { get; set; } // "E" = Entrada, "S" = Salida

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Cantidad { get; set; }

        [Required]
        [StringLength(500)]
        public string Motivo { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Required]
        public int IdUsuario { get; set; }

        [Required]
        public DateTime FechaMovimiento { get; set; }
    }
}