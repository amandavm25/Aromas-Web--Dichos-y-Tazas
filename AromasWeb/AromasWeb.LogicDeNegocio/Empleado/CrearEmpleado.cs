using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.Empleados
{
    public class CrearEmpleado : ICrearEmpleado
    {
        private ICrearEmpleado _crearEmpleado;

        public CrearEmpleado()
        {
            _crearEmpleado = new AccesoADatos.Empleados.CrearEmpleado();
        }

        public async Task<int> Crear(Empleado empleado)
        {
            int resultado = await _crearEmpleado.Crear(empleado);
            return resultado;
        }
    }
}