using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Roles
{
    public class ObtenerRol : IObtenerRol
    {
        private IObtenerRol _obtenerRol;

        public ObtenerRol()
        {
            _obtenerRol = new AccesoADatos.Roles.ObtenerRol();
        }

        public Rol Obtener(int id)
        {
            return _obtenerRol.Obtener(id);
        }
    }
}