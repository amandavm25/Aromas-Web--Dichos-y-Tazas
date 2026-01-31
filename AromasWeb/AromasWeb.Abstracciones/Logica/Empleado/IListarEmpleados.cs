using System.Collections.Generic;
using EmpleadoUI = AromasWeb.Abstracciones.ModeloUI.Empleado;

namespace AromasWeb.Abstracciones.Logica.Empleado
{
    public interface IListarEmpleados
    {
        List<EmpleadoUI> Obtener();
        List<EmpleadoUI> BuscarPorNombre(string nombre);
        List<EmpleadoUI> BuscarPorIdentificacion(string identificacion);
        List<EmpleadoUI> BuscarPorCargo(string cargo);
        List<EmpleadoUI> BuscarPorEstado(bool estado);
        List<EmpleadoUI> BuscarPorRol(int idRol);
        EmpleadoUI ObtenerPorId(int id);
    }
}