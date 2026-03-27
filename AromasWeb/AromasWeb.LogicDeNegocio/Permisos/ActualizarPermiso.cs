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
            // Validar que el permiso no sea nulo y tenga nombre
            if (permiso == null || string.IsNullOrWhiteSpace(permiso.Nombre))
                throw new ArgumentException("El permiso y su nombre son requeridos");

            int resultado = _actualizarPermiso.Actualizar(permiso);
            return resultado;
        }
    }
}