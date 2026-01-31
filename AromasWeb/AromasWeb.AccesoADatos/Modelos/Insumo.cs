using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("Insumo")]
    public class InsumoAD
    {
        [Key]
        public int IdInsumo { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreInsumo { get; set; }

        [Required]
        [StringLength(50)]
        public string UnidadMedida { get; set; }

        [Required]
        public int IdCategoria { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CostoUnitario { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal CantidadDisponible { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal StockMinimo { get; set; }

        [Required]
        public bool Estado { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaActualizacion { get; set; }

        // Propiedades de navegación
        public virtual CategoriaInsumoAD CategoriaInsumo { get; set; }
    }
}