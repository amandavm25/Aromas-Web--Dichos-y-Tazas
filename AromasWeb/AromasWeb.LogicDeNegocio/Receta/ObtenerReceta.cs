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

        public Receta Obtener(int id)
        {
            Receta laReceta = _obtenerReceta.Obtener(id);
            return laReceta;
        }
    }
}