using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.SolicitudesVacaciones
{
    public class ListarSolicitudesVacaciones : IListarSolicitudesVacaciones
    {
        private IListarSolicitudesVacaciones _listarSolicitudesVacaciones;

        public ListarSolicitudesVacaciones()
        {
            _listarSolicitudesVacaciones = new AccesoADatos.SolicitudesVacaciones.ListarSolicitudesVacaciones();
        }

        public List<SolicitudVacaciones> Obtener()
        {
            return _listarSolicitudesVacaciones.Obtener();
        }

        public List<SolicitudVacaciones> BuscarPorEmpleado(int idEmpleado)
        {
            return _listarSolicitudesVacaciones.BuscarPorEmpleado(idEmpleado);
        }

        public List<SolicitudVacaciones> BuscarPorEstado(string estado)
        {
            return _listarSolicitudesVacaciones.BuscarPorEstado(estado);
        }

        public SolicitudVacaciones ObtenerPorId(int id)
        {
            return _listarSolicitudesVacaciones.ObtenerPorId(id);
        }
    }
}