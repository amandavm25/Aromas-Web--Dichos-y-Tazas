using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Recetas
{
    public class ActualizarReceta : IActualizarReceta
    {
        private IActualizarReceta _actualizarReceta;

        public ActualizarReceta()
        {
            _actualizarReceta = new AccesoADatos.Recetas.ActualizarReceta();
        }

        public int Actualizar(Abstracciones.ModeloUI.Receta receta)
        {
            int resultado = _actualizarReceta.Actualizar(receta);
            return resultado;
        }
    }
}