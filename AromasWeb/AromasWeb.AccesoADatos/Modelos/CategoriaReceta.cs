using System;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class CategoriaRecetaAD
    {
        public int IdCategoriaReceta { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public bool Estado { get; set; }
    }
}