using AromasWeb.Abstracciones.Logica.Cliente;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.LogicaDeNegocio.Clientes
{
    public class ObtenerCliente : IObtenerCliente
    {
        private IObtenerCliente _obtenerCliente;

        public ObtenerCliente()
        {
            _obtenerCliente = new AccesoADatos.Clientes.ObtenerCliente();
        }

        public ClienteUI Obtener(int id)
        {
            return _obtenerCliente.Obtener(id);
        }
    }
}