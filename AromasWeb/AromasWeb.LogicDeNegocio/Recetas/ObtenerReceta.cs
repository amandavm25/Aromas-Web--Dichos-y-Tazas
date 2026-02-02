using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Recetas
{
    public class ObtenerReceta : IObtenerReceta
    {
        private IObtenerReceta _obtenerReceta;

        public ObtenerReceta()
        {
            _obtenerReceta = new AccesoADatos.Recetas.ObtenerReceta();
        }

        public Abstracciones.ModeloUI.Receta Obtener(int id)
        {
            Abstracciones.ModeloUI.Receta laReceta = _obtenerReceta.Obtener(id);
            return laReceta;
        }
    }
}