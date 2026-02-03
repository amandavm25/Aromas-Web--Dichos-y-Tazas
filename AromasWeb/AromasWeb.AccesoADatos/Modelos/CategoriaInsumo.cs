using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AromasWeb.AccesoADatos.Modelos
{
    [Table("CategoriaInsumo")]
    public class CategoriaInsumoAD
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCategoria { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}