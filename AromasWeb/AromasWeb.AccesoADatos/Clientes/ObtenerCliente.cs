using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.AccesoADatos.Modelos;
using System.Linq;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.AccesoADatos.Clientes
{
    public class ObtenerCliente : IObtenerCliente
    {
        private Contexto _contexto;

        public ObtenerCliente()
        {
            _contexto = new Contexto();
        }

        public ClienteUI Obtener(int id)
        {
            var clienteAD = _contexto.Cliente.FirstOrDefault(c => c.IdCliente == id);

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
                UltimaReserva = null // Se puede calcular con una consulta adicional si es necesario
            };
        }
    }
}