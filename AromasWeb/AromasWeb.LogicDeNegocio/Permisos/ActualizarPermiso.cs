using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Permisos
{
    public class ActualizarPermiso : IActualizarPermiso
    {
        private IActualizarPermiso _actualizarPermiso;

        public ActualizarPermiso()
        {
            _actualizarPermiso = new AccesoADatos.Permisos.ActualizarPermiso();
        }

        public int Actualizar(Permiso permiso)
        {
            int resultado = _actualizarPermiso.Actualizar(permiso);
            return resultado;
        }
    }
}