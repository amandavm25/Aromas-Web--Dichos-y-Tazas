using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Empleados
{
    public class ListarEmpleados : IListarEmpleados
    {
        private IListarEmpleados _listarEmpleados;

        public ListarEmpleados()
        {
            _listarEmpleados = new AccesoADatos.Empleados.ListarEmpleados();
        }

        public List<Empleado> Obtener()
        {
            return _listarEmpleados.Obtener();
        }

        public List<Empleado> BuscarPorNombre(string nombre)
        {
            return _listarEmpleados.BuscarPorNombre(nombre);
        }

        public List<Empleado> BuscarPorIdentificacion(string identificacion)
        {
            return _listarEmpleados.BuscarPorIdentificacion(identificacion);
        }

        public List<Empleado> BuscarPorCargo(string cargo)
        {
            return _listarEmpleados.BuscarPorCargo(cargo);
        }

        public List<Empleado> BuscarPorEstado(bool estado)
        {
            return _listarEmpleados.BuscarPorEstado(estado);
        }

        public List<Empleado> BuscarPorRol(int idRol)
        {
            return _listarEmpleados.BuscarPorRol(idRol);
        }

        public Empleado ObtenerPorId(int id)
        {
            return _listarEmpleados.ObtenerPorId(id);
        }
    }
}