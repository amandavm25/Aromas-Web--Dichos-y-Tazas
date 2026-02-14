using System.Threading.Tasks;
using EmpleadoUI = AromasWeb.Abstracciones.ModeloUI.Empleado;

namespace AromasWeb.Abstracciones.Logica.Empleado
{
    public interface ICrearEmpleado
    {
        Task<int> Crear(EmpleadoUI empleado);
    }
}
