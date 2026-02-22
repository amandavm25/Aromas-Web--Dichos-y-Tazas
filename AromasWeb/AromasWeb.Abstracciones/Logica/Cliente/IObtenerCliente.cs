using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.Abstracciones.Logica.Cliente
{
    public interface IObtenerCliente
    {
        ClienteUI Obtener(int id);
    }
}