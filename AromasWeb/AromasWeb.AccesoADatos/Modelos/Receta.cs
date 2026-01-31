using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class RecetaAD
    {
        public int IdReceta { get; set; }
        public int IdCategoriaReceta { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public int CantidadPorciones { get; set; }
        public string PasosPreparacion { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal CostoTotal { get; set; }
        public decimal CostoPorcion { get; set; }
        public decimal GananciaNeta { get; set; }
        public decimal PorcentajeMargen { get; set; }
        public bool Disponibilidad { get; set; }

        // Propiedades de navegación
        public virtual CategoriaRecetaAD CategoriaReceta { get; set; }
        public virtual ICollection<RecetaInsumoAD> RecetaInsumos { get; set; }
    }
}