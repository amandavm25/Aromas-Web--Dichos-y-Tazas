using EmpleadoUI = AromasWeb.Abstracciones.ModeloUI.Empleado;

namespace AromasWeb.Abstracciones.Logica.Empleado
{
    public interface IActualizarEmpleado
    {
        int Actualizar(EmpleadoUI empleado);
    }
}