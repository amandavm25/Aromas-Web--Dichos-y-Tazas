using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("Promocion")]
    public class PromocionAD
    {
        [Key]
        public int IdPromocion { get; set; }

        [Required]
        public int IdTipoPromocion { get; set; }

        [Required]
        [StringLength(200)]
        public string Nombre { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        [Column(TypeName = "decimal(5,2)")]
        public decimal PorcentajeDescuento { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }

        [Required]
        public DateTime FechaFin { get; set; }

        [Required]
        public bool Estado { get; set; }

        // Propiedades de navegación
        public virtual TipoPromocionAD TipoPromocion { get; set; }
        public virtual ICollection<PromocionRecetaAD> PromocionRecetas { get; set; }
            = new List<PromocionRecetaAD>();
    }
}