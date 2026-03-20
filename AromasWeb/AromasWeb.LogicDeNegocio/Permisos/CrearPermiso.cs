using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Permisos
{
    public class CrearPermiso : ICrearPermiso
    {
        private ICrearPermiso _crearPermiso;

        public CrearPermiso()
        {
            _crearPermiso = new AccesoADatos.Permisos.CrearPermiso();
        }

        public int Crear(Permiso permiso)
        {
            int resultado = _crearPermiso.Crear(permiso);
            return resultado;
        }
    }
}