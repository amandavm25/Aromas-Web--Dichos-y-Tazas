using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Empleados
{
    public class ActualizarEmpleado : IActualizarEmpleado
    {
        private IActualizarEmpleado _actualizarEmpleado;

        public ActualizarEmpleado()
        {
            _actualizarEmpleado = new AccesoADatos.Empleados.ActualizarEmpleado();
        }

        public int Actualizar(Empleado empleado)
        {
            int resultado = _actualizarEmpleado.Actualizar(empleado);
            return resultado;
        }
    }
}