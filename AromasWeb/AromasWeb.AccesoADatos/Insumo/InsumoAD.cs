using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.Insumo
{
   public class InsumoAD
    {
        public int IdInsumo { get; set; }
        public string Nombre { get; set; }
        public string UnidadMedida { get; set; }
        public int Categoria { get; set; }
        public decimal CostoUnitario { get; set; }
        public int Cantidad { get; set; }
        public DateTime FechaRegistro { get; set; }
    }
}
