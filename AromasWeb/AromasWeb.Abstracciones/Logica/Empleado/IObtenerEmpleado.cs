using EmpleadoUI = AromasWeb.Abstracciones.ModeloUI.Empleado;

namespace AromasWeb.Abstracciones.Logica.Empleado
{
    public interface IObtenerEmpleado
    {
        EmpleadoUI Obtener(int id);
    }
}
