using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Roles
{
    public class ActualizarRol : IActualizarRol
    {
        private IActualizarRol _actualizarRol;

        public ActualizarRol()
        {
            _actualizarRol = new AccesoADatos.Roles.ActualizarRol();
        }

        public int Actualizar(Rol rol)
        {
            return _actualizarRol.Actualizar(rol);
        }
    }
}