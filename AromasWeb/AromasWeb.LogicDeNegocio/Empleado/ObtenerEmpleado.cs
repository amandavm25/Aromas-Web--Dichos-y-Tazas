using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Empleados
{
    public class ObtenerEmpleado : IObtenerEmpleado
    {
        private IObtenerEmpleado _obtenerEmpleado;

        public ObtenerEmpleado()
        {
            _obtenerEmpleado = new AccesoADatos.Empleados.ObtenerEmpleado();
        }

        public Empleado Obtener(int id)
        {
            Empleado empleado = _obtenerEmpleado.Obtener(id);
            return empleado;
        }
    }
}