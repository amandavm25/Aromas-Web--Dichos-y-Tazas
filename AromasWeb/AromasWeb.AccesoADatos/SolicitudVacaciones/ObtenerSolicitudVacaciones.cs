using AromasWeb.Abstracciones.Logica.SolicitudVacaciones;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.SolicitudesVacaciones
{
    public class ObtenerSolicitudVacaciones : IObtenerSolicitudVacaciones
    {
        private Contexto _contexto;

        public ObtenerSolicitudVacaciones()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.SolicitudVacaciones Obtener(int idSolicitud)
        {
            var solicitudAD = _contexto.SolicitudVacaciones
                .FirstOrDefault(s => s.IdSolicitud == idSolicitud);

            if (solicitudAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(solicitudAD);
        }

        private Abstracciones.ModeloUI.SolicitudVacaciones ConvertirObjetoParaUI(SolicitudVacacionesAD solicitudAD)
        {
            string nombreEmpleado = "Desconocido";
            string identificacionEmpleado = "";
            string cargoEmpleado = "";
            DateTime fechaContratacionEmpleado = DateTime.Now;
            decimal diasDisponibles = 0;
            decimal diasTomados = 0;

            try
            {
                var empleado = _contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == solicitudAD.IdEmpleado);
                if (empleado != null)
                {
                    nombreEmpleado = $"{empleado.Nombre} {empleado.Apellidos}";
                    identificacionEmpleado = empleado.Identificacion;
                    cargoEmpleado = empleado.Cargo;
                    fechaContratacionEmpleado = empleado.FechaContratacion;

                    diasTomados = _contexto.SolicitudVacaciones
                        .Where(s => s.IdEmpleado == solicitudAD.IdEmpleado && s.Estado == "Aprobada")
                        .Sum(s => (decimal?)s.DiasSolicitados) ?? 0;

                    var mesesTrabajados = (int)((DateTime.Now - empleado.FechaContratacion).TotalDays / 30);
                    diasDisponibles = mesesTrabajados - diasTomados;
                }
            }
            catch
            {
                // Usar valores por defecto si hay error
            }

            return new Abstracciones.ModeloUI.SolicitudVacaciones
            {
                IdSolicitud = solicitudAD.IdSolicitud,
                IdEmpleado = solicitudAD.IdEmpleado,
                FechaSolicitud = solicitudAD.FechaSolicitud,
                FechaInicio = solicitudAD.FechaInicio,
                FechaFin = solicitudAD.FechaFin,
                DiasSolicitados = solicitudAD.DiasSolicitados,
                Estado = solicitudAD.Estado,
                FechaRespuesta = solicitudAD.FechaRespuesta,
                NombreEmpleado = nombreEmpleado,
                IdentificacionEmpleado = identificacionEmpleado,
                CargoEmpleado = cargoEmpleado,
                FechaContratacionEmpleado = fechaContratacionEmpleado,
                DiasDisponibles = diasDisponibles,
                DiasTomados = diasTomados
            };
        }
    }
}