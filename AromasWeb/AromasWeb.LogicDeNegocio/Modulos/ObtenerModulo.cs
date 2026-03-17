using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Modulos
{
    public class ObtenerModulo : IObtenerModulo
    {
        private IObtenerModulo _obtenerModulo;

        public ObtenerModulo()
        {
            _obtenerModulo = new AccesoADatos.Modulos.ObtenerModulo();
        }

        public Modulo Obtener(int id)
        {
            return _obtenerModulo.Obtener(id);
        }
    }
}