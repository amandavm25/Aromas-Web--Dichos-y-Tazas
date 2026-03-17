using System.Threading.Tasks;
using ModuloUI = AromasWeb.Abstracciones.ModeloUI.Modulo;

namespace AromasWeb.Abstracciones.Logica.Modulo
{
    public interface ICrearModulo
    {
        Task<int> Crear(ModuloUI modulo);
    }
}