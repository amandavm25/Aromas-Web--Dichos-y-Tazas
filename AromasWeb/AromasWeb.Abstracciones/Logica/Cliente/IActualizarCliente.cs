using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.Abstracciones.Logica.Cliente
{
    public interface IActualizarCliente
    {
        int Actualizar(ClienteUI cliente);
    }
}