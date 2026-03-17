using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Modulos
{
    public class ActualizarModulo : IActualizarModulo
    {
        private IActualizarModulo _actualizarModulo;

        public ActualizarModulo()
        {
            _actualizarModulo = new AccesoADatos.Modulos.ActualizarModulo();
        }

        public int Actualizar(Modulo modulo)
        {
            return _actualizarModulo.Actualizar(modulo);
        }
    }
}