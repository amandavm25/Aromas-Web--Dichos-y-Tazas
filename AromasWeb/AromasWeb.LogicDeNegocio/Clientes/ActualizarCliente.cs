using AromasWeb.Abstracciones.Logica.Cliente;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.LogicaDeNegocio.Clientes
{
    public class ActualizarCliente : IActualizarCliente
    {
        private IActualizarCliente _actualizarCliente;

        public ActualizarCliente()
        {
            _actualizarCliente = new AccesoADatos.Clientes.ActualizarCliente();
        }

        public int Actualizar(ClienteUI cliente)
        {
            return _actualizarCliente.Actualizar(cliente);
        }
    }
}