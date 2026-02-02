using System;
using System.Collections.Generic;
using AsistenciaUI = AromasWeb.Abstracciones.ModeloUI.Asistencia;

namespace AromasWeb.Abstracciones.Logica.Asistencia
{
    public interface IListarAsistencias
    {
        List<AsistenciaUI> Obtener();
        List<AsistenciaUI> BuscarPorEmpleado(int idEmpleado);
        List<AsistenciaUI> BuscarPorFecha(DateTime fecha);
        List<AsistenciaUI> BuscarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin);
        AsistenciaUI ObtenerPorId(int id);
    }
}