using AromasWeb.Abstracciones.Logica.Planilla;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Planillas
{
    public class ListarPlanillas : IListarPlanillas
    {
        private IListarPlanillas _listarPlanillas;

        public ListarPlanillas()
        {
            _listarPlanillas = new AccesoADatos.Planillas.ListarPlanillas();
        }

        public List<Planilla> Obtener()
        {
            return _listarPlanillas.Obtener();
        }

        public List<Planilla> ObtenerPorEmpleado(int idEmpleado)
        {
            return _listarPlanillas.ObtenerPorEmpleado(idEmpleado);
        }

        public List<Planilla> ObtenerPorEstado(string estado)
        {
            return _listarPlanillas.ObtenerPorEstado(estado);
        }

        public List<Planilla> ObtenerPorPeriodo(DateTime fechaInicio, DateTime fechaFin)
        {
            return _listarPlanillas.ObtenerPorPeriodo(fechaInicio, fechaFin);
        }

        public Planilla ObtenerPorId(int id)
        {
            return _listarPlanillas.ObtenerPorId(id);
        }

        public List<DetallePlanilla> ObtenerDetallesPorPlanilla(int idPlanilla)
        {
            return _listarPlanillas.ObtenerDetallesPorPlanilla(idPlanilla);
        }
    }
}