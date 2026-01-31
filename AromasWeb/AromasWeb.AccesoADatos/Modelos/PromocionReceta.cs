using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("PromocionReceta")]
    public class PromocionRecetaAD
    {
        [Key]
        public int IdPromocionReceta { get; set; }

        [Required]
        public int IdPromocion { get; set; }

        [Required]
        public int IdReceta { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal PrecioPromocional { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PorcentajeDescuento { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Propiedades de navegación
        public virtual PromocionAD Promocion { get; set; }
        public virtual RecetaAD Receta { get; set; }
    }
}