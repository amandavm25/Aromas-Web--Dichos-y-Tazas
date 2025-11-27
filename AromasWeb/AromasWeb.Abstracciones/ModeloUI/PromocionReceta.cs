using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class PromocionReceta
    {
        [DisplayName("ID Promoción Receta")]
        public int IdPromocionReceta { get; set; }

        [DisplayName("ID Promoción")]
        [Required(ErrorMessage = "El ID de la promoción es requerido")]
        public int IdPromocion { get; set; }

        [DisplayName("ID Receta")]
        [Required(ErrorMessage = "El ID de la receta es requerido")]
        public int IdReceta { get; set; }

        [DisplayName("Precio Promocional")]
        [Required(ErrorMessage = "El precio promocional es requerido")]
        [Range(0, double.MaxValue, ErrorMessage = "El precio promocional debe ser mayor o igual a 0")]
        [DataType(DataType.Currency)]
        public decimal PrecioPromocional { get; set; }

        [DisplayName("Porcentaje de Descuento")]
        [Required(ErrorMessage = "El porcentaje de descuento es requerido")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        public decimal PorcentajeDescuento { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        // Propiedades de navegación / adicionales para la UI
        [DisplayName("Nombre de la Receta")]
        public string NombreReceta { get; set; }

        [DisplayName("Categoría")]
        public string CategoriaReceta { get; set; }

        [DisplayName("Precio Original")]
        [DataType(DataType.Currency)]
        public decimal PrecioOriginal { get; set; }

        [DisplayName("Nombre de la Promoción")]
        public string NombrePromocion { get; set; }

        // Propiedades calculadas
        [DisplayName("Ahorro")]
        [DataType(DataType.Currency)]
        public decimal Ahorro
        {
            get
            {
                return PrecioOriginal - PrecioPromocional;
            }
        }

        [DisplayName("Porcentaje de Ahorro")]
        public decimal PorcentajeAhorro
        {
            get
            {
                if (PrecioOriginal == 0)
                    return 0;

                return ((PrecioOriginal - PrecioPromocional) / PrecioOriginal) * 100;
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

        // Método para formatear precio con símbolo de moneda
        public string PrecioPromocionalFormateado
        {
            get
            {
                return string.Format("₡{0:N2}", PrecioPromocional);
            }
        }

        public string PrecioOriginalFormateado
        {
            get
            {
                return string.Format("₡{0:N2}", PrecioOriginal);
            }
        }

        public string AhorroFormateado
        {
            get
            {
                return string.Format("₡{0:N2}", Ahorro);
            }
        }

        // Validación personalizada: el precio promocional debe ser menor que el original
        public bool EsPrecioValido()
        {
            return PrecioPromocional < PrecioOriginal;
        }

        // Calcular precio promocional basado en el porcentaje de descuento
        public void CalcularPrecioPromocional()
        {
            if (PrecioOriginal > 0 && PorcentajeDescuento > 0)
            {
                PrecioPromocional = PrecioOriginal * (1 - (PorcentajeDescuento / 100));
            }
        }

        // Calcular porcentaje de descuento basado en los precios
        public void CalcularPorcentajeDescuento()
        {
            if (PrecioOriginal > 0)
            {
                PorcentajeDescuento = ((PrecioOriginal - PrecioPromocional) / PrecioOriginal) * 100;
            }
        }
    }
}