using System.Collections.Generic;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.Abstracciones.Logica.Cliente
{
    public interface IListarClientes
    {
        List<ClienteUI> Obtener();
        List<ClienteUI> BuscarPorNombre(string nombre);
        List<ClienteUI> BuscarPorIdentificacion(string identificacion);
        List<ClienteUI> BuscarPorTelefono(string telefono);
        List<ClienteUI> BuscarPorEstado(bool estado);
        ClienteUI ObtenerPorId(int id);
    }
}