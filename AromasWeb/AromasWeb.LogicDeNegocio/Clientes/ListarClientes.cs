using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Clientes
{
    public class ListarClientes : IListarClientes
    {
        private IListarClientes _listarClientes;

        public ListarClientes()
        {
            _listarClientes = new AccesoADatos.Clientes.ListarClientes();
        }

        public List<Cliente> Obtener()
        {
            return _listarClientes.Obtener();
        }

        public List<Cliente> BuscarPorNombre(string nombre)
        {
            return _listarClientes.BuscarPorNombre(nombre);
        }

        public List<Cliente> BuscarPorIdentificacion(string identificacion)
        {
            return _listarClientes.BuscarPorIdentificacion(identificacion);
        }

        public List<Cliente> BuscarPorTelefono(string telefono)
        {
            return _listarClientes.BuscarPorTelefono(telefono);
        }

        public List<Cliente> BuscarPorEstado(bool estado)
        {
            return _listarClientes.BuscarPorEstado(estado);
        }

        public Cliente ObtenerPorId(int id)
        {
            return _listarClientes.ObtenerPorId(id);
        }
    }
}