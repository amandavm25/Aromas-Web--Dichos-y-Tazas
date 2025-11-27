using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Promocion
    {
        public int IdPromocion { get; set; }

        [DisplayName("Tipo de promoción")]
        [Required(ErrorMessage = "El tipo de promoción es requerido")]
        public int IdTipoPromocion { get; set; }

        [DisplayName("Nombre de la promoción")]
        [Required(ErrorMessage = "El nombre de la promoción es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [DisplayName("Descripción")]
        [StringLength(500, ErrorMessage = "La descripción no puede exceder 500 caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Porcentaje de descuento")]
        [Required(ErrorMessage = "El porcentaje de descuento es requerido")]
        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100")]
        public decimal PorcentajeDescuento { get; set; }

        [DisplayName("Fecha de inicio")]
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de fin")]
        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        // Lista de recetas asociadas
        public List<PromocionReceta> Recetas { get; set; } = new List<PromocionReceta>();

        // Propiedades calculadas
        [DisplayName("Tipo")]
        public string NombreTipoPromocion { get; set; }

        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Estado ? "Activa" : "Inactiva";
            }
        }

        [DisplayName("Vigencia")]
        public string VigenciaTexto
        {
            get
            {
                var hoy = DateTime.Today;
                if (hoy < FechaInicio)
                    return "Próximamente";
                else if (hoy > FechaFin)
                    return "Expirada";
                else
                    return "Vigente";
            }
        }

        public bool EstaVigente
        {
            get
            {
                var hoy = DateTime.Today;
                return hoy >= FechaInicio && hoy <= FechaFin && Estado;
            }
        }

        public int DiasRestantes
        {
            get
            {
                if (!EstaVigente)
                    return 0;
                return (FechaFin - DateTime.Today).Days;
            }
        }

        [DisplayName("Cantidad de recetas")]
        public int CantidadRecetas
        {
            get
            {
                return Recetas?.Count ?? 0;
            }
        }
    }
}