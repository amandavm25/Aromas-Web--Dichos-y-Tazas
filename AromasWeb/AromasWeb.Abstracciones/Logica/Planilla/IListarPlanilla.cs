using System;
using System.Collections.Generic;
using PlanillaUI = AromasWeb.Abstracciones.ModeloUI.Planilla;
using DetallePlanillaUI = AromasWeb.Abstracciones.ModeloUI.DetallePlanilla;

namespace AromasWeb.Abstracciones.Logica.Planilla
{
    public interface IListarPlanillas
    {
        List<PlanillaUI> Obtener();
        List<PlanillaUI> ObtenerPorEmpleado(int idEmpleado);
        List<PlanillaUI> ObtenerPorEstado(string estado);
        List<PlanillaUI> ObtenerPorPeriodo(DateTime fechaInicio, DateTime fechaFin);
        PlanillaUI ObtenerPorId(int id);
        List<DetallePlanillaUI> ObtenerDetallesPorPlanilla(int idPlanilla);
    }
}