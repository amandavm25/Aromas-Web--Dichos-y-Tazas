using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Modulos
{
    public class ListarModulos : IListarModulos
    {
        private IListarModulos _listarModulos;

        public ListarModulos()
        {
            _listarModulos = new AccesoADatos.Modulos.ListarModulos();
        }

        public List<Modulo> Obtener()
        {
            return _listarModulos.Obtener();
        }

        public List<Modulo> BuscarPorNombre(string nombre)
        {
            return _listarModulos.BuscarPorNombre(nombre);
        }

        public List<Modulo> BuscarPorEstado(bool estado)
        {
            return _listarModulos.BuscarPorEstado(estado);
        }

        public Modulo ObtenerPorId(int id)
        {
            return _listarModulos.ObtenerPorId(id);
        }
    }
}