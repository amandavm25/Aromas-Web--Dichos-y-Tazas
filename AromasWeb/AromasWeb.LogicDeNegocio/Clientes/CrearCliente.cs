using AromasWeb.Abstracciones.Logica.Cliente;
using System.Threading.Tasks;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.LogicaDeNegocio.Clientes
{
    public class CrearCliente : ICrearCliente
    {
        private ICrearCliente _crearCliente;

        public CrearCliente()
        {
            _crearCliente = new AccesoADatos.Clientes.CrearCliente();
        }

        public async Task<int> Crear(ClienteUI cliente)
        {
            return await _crearCliente.Crear(cliente);
        }
    }
}