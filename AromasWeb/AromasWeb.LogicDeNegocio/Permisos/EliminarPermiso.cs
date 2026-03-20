using AromasWeb.Abstracciones.Logica.Permiso;

namespace AromasWeb.LogicaDeNegocio.Permisos
{
    public class EliminarPermiso : IEliminarPermiso
    {
        private IEliminarPermiso _eliminarPermiso;

        public EliminarPermiso()
        {
            _eliminarPermiso = new AccesoADatos.Permisos.EliminarPermiso();
        }

        public int Eliminar(int id)
        {
            int resultado = _eliminarPermiso.Eliminar(id);
            return resultado;
        }
    }
}