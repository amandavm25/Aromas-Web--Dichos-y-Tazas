using System;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class SolicitudVacacionesAD
    {
        public int IdSolicitud { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public int DiasSolicitados { get; set; }
        public string Estado { get; set; }
        public DateTime? FechaRespuesta { get; set; }

        // Propiedades de navegación
        public virtual EmpleadoAD Empleado { get; set; }
    }
}