using System;

namespace AromasWeb.AccesoADatos.Modelos
{
    public class AsistenciaAD
    {
        public int IdAsistencia { get; set; }
        public int IdEmpleado { get; set; }
        public DateTime Fecha { get; set; }
        public TimeSpan HoraEntrada { get; set; }
        public TimeSpan? HoraSalida { get; set; }
        public int TiempoAlmuerzo { get; set; }
        public decimal HorasRegulares { get; set; }
        public decimal HorasExtras { get; set; }
        public decimal HorasTotales { get; set; }

        // Propiedades de navegación
        public virtual EmpleadoAD Empleado { get; set; }
    }
}