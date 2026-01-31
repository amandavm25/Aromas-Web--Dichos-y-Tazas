using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Roles
{
    public class ListarRoles : IListarRoles
    {
        private IListarRoles _listarRoles;

        public ListarRoles()
        {
            _listarRoles = new AccesoADatos.Roles.ListarRoles();
        }

        public List<Rol> Obtener()
        {
            return _listarRoles.Obtener();
        }

        public List<Rol> BuscarPorNombre(string nombre)
        {
            return _listarRoles.BuscarPorNombre(nombre);
        }

        public List<Rol> BuscarPorEstado(bool estado)
        {
            return _listarRoles.BuscarPorEstado(estado);
        }

        public Rol ObtenerPorId(int id)
        {
            return _listarRoles.ObtenerPorId(id);
        }
    }
}