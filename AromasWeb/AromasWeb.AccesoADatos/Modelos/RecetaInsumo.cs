using System;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class RecetaInsumoAD
    {
        public int IdRecetaInsumo { get; set; }
        public int IdInsumo { get; set; }
        public int IdReceta { get; set; }
        public decimal CantidadUtilizada { get; set; }
        public decimal CostoUnitario { get; set; }
        public decimal CostoTotalIngrediente { get; set; }

        // Propiedades de navegación
        public virtual RecetaAD Receta { get; set; }
        public virtual InsumoAD Insumo { get; set; }
    }
}