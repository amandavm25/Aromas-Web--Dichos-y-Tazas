using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Permisos
{
    public class ListarPermisos : IListarPermisos
    {
        private IListarPermisos _listarPermisos;

        public ListarPermisos()
        {
            _listarPermisos = new AccesoADatos.Permisos.ListarPermisos();
        }

        public List<Permiso> Obtener()
        {
            return _listarPermisos.Obtener();
        }

        public List<Permiso> ObtenerPorModulo(int idModulo)
        {
            return _listarPermisos.ObtenerPorModulo(idModulo);
        }

        public List<Permiso> ObtenerPorRol(int idRol)
        {
            return _listarPermisos.ObtenerPorRol(idRol);
        }

        public Permiso ObtenerPorId(int id)
        {
            return _listarPermisos.ObtenerPorId(id);
        }

        public bool AsignarPermisosARol(int idRol, List<int> idsPermisos)
        {
            return _listarPermisos.AsignarPermisosARol(idRol, idsPermisos);
        }

        public List<int> ObtenerPermisosDeRol(int idRol)
        {
            return _listarPermisos.ObtenerPermisosDeRol(idRol);
        }
    }
}