using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("TipoPromocion")]
    public class TipoPromocionAD
    {
        [Key]
        public int IdTipoPromocion { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreTipo { get; set; }

        [StringLength(300)]
        public string Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}