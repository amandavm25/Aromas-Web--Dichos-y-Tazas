using AromasWeb.Abstracciones.Logica.HistorialTarifa;
using AromasWeb.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.HistorialTarifas
{
    public class ListarHistorialTarifa : IListarHistorialTarifa
    {
        private IListarHistorialTarifa _listarHistorialTarifa;

        public ListarHistorialTarifa()
        {
            _listarHistorialTarifa = new AccesoADatos.HistorialTarifas.ListarHistorialTarifa();
        }

        public List<HistorialTarifa> Obtener()
        {
            return _listarHistorialTarifa.Obtener();
        }

        public List<HistorialTarifa> ObtenerPorEmpleado(int idEmpleado)
        {
            return _listarHistorialTarifa.ObtenerPorEmpleado(idEmpleado);
        }

        public List<HistorialTarifa> ObtenerPorEstado(string estado)
        {
            return _listarHistorialTarifa.ObtenerPorEstado(estado);
        }

        public HistorialTarifa ObtenerPorId(int id)
        {
            return _listarHistorialTarifa.ObtenerPorId(id);
        }

        public HistorialTarifa ObtenerTarifaActualPorEmpleado(int idEmpleado)
        {
            return _listarHistorialTarifa.ObtenerTarifaActualPorEmpleado(idEmpleado);
        }
    }
}