using System.Threading.Tasks;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.Abstracciones.Logica.Cliente
{
    public interface ICrearCliente
    {
        Task<int> Crear(ClienteUI cliente);
    }
}