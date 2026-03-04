using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.Abstracciones.Logica.Cliente
{
    public interface IBuscarClientePorCorreo
    {
        ClienteUI ObtenerPorCorreo(string correo);
    }
}