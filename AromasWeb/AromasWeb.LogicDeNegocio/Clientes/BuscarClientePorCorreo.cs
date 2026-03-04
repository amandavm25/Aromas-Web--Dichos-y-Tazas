using AromasWeb.Abstracciones.Logica.Cliente;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.LogicaDeNegocio.Clientes
{
    public class BuscarClientePorCorreo : IBuscarClientePorCorreo
    {
        private AccesoADatos.Clientes.BuscarClientePorCorreo _buscarClientePorCorreo;

        public BuscarClientePorCorreo()
        {
            _buscarClientePorCorreo = new AccesoADatos.Clientes.BuscarClientePorCorreo();
        }

        public ClienteUI ObtenerPorCorreo(string correo)
        {
            return _buscarClientePorCorreo.ObtenerPorCorreo(correo);
        }
    }
}