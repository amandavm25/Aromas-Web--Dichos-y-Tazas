using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.CategoriasReceta
{
    public class ObtenerCategoriaReceta : IObtenerCategoriaReceta
    {
        private IObtenerCategoriaReceta _obtenerCategoriaReceta;

        public ObtenerCategoriaReceta()
        {
            _obtenerCategoriaReceta = new AccesoADatos.CategoriasReceta.ObtenerCategoriaReceta();
        }

        public CategoriaReceta Obtener(int id)
        {
            CategoriaReceta categoria = _obtenerCategoriaReceta.Obtener(id);
            return categoria;
        }
    }
}