using System.Threading.Tasks;
using RolUI = AromasWeb.Abstracciones.ModeloUI.Rol;

namespace AromasWeb.Abstracciones.Logica.Rol
{
    public interface ICrearRol
    {
        Task<int> Crear(RolUI rol);
    }
}