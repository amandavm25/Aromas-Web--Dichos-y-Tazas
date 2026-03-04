using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.AccesoADatos.Modelos;
using System.Linq;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.AccesoADatos.Clientes
{
    public class BuscarClientePorCorreo : IBuscarClientePorCorreo
    {
        private Contexto _contexto;

        public BuscarClientePorCorreo()
        {
            _contexto = new Contexto();
        }

        public ClienteUI ObtenerPorCorreo(string correo)
        {
            var clienteAD = _contexto.Cliente
                .FirstOrDefault(c => c.Correo.ToLower() == correo.ToLower());

            if (clienteAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(clienteAD);
        }

        private ClienteUI ConvertirObjetoParaUI(ClienteAD clienteAD)
        {
            return new ClienteUI
            {
                IdCliente = clienteAD.IdCliente,
                Identificacion = clienteAD.Identificacion,
                Nombre = clienteAD.Nombre,
                Apellidos = clienteAD.Apellidos,
                Correo = clienteAD.Correo,
                Telefono = clienteAD.Telefono,
                Contrasena = clienteAD.Contrasena,
                Estado = clienteAD.Estado,
                FechaRegistro = clienteAD.FechaRegistro,
                UltimaReserva = clienteAD.UltimaReserva
            };
        }
    }
}