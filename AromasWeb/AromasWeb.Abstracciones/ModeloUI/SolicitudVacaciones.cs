using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class SolicitudVacaciones
    {
        public int IdSolicitud { get; set; }

        [DisplayName("Empleado")]
        [Required(ErrorMessage = "El empleado es requerido")]
        public int IdEmpleado { get; set; }

        [DisplayName("Fecha de solicitud")]
        [Required(ErrorMessage = "La fecha de solicitud es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaSolicitud { get; set; }

        [DisplayName("Fecha de inicio")]
        [Required(ErrorMessage = "La fecha de inicio es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaInicio { get; set; }

        [DisplayName("Fecha de fin")]
        [Required(ErrorMessage = "La fecha de fin es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaFin { get; set; }

        [DisplayName("Días solicitados")]
        [Required(ErrorMessage = "Los días solicitados son requeridos")]
        [Range(1, 365, ErrorMessage = "Los días solicitados deben estar entre 1 y 365")]
        public int DiasSolicitados { get; set; }

        [DisplayName("Días disponibles")]
        public decimal DiasDisponibles { get; set; }

        [DisplayName("Estado")]
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(50, ErrorMessage = "El estado no puede exceder 50 caracteres")]
        public string Estado { get; set; }

        [DisplayName("Fecha de respuesta")]
        [DataType(DataType.Date)]
        public DateTime? FechaRespuesta { get; set; }

        // Propiedades de navegación
        [DisplayName("Empleado")]
        public string NombreEmpleado { get; set; }

        [DisplayName("Identificación")]
        public string IdentificacionEmpleado { get; set; }

        [DisplayName("Cargo")]
        public string CargoEmpleado { get; set; }

        [DisplayName("Fecha de contratación")]
        public DateTime FechaContratacionEmpleado { get; set; }

        // Propiedades calculadas
        [DisplayName("Fecha de solicitud")]
        public string FechaSolicitudFormateada
        {
            get
            {
                return FechaSolicitud.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Fecha de inicio")]
        public string FechaInicioFormateada
        {
            get
            {
                return FechaInicio.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Fecha de fin")]
        public string FechaFinFormateada
        {
            get
            {
                return FechaFin.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Fecha de respuesta")]
        public string FechaRespuestaFormateada
        {
            get
            {
                return FechaRespuesta?.ToString("dd/MM/yyyy") ?? "Pendiente";
            }
        }

        [DisplayName("Período de vacaciones")]
        public string PeriodoVacaciones
        {
            get
            {
                return $"{FechaInicioFormateada} - {FechaFinFormateada}";
            }
        }

        [DisplayName("Estado")]
        public string EstadoBadgeClass
        {
            get
            {
                return Estado switch
                {
                    "Aprobada" => "success",
                    "Rechazada" => "danger",
                    "Pendiente" => "warning",
                    _ => "secondary"
                };
            }
        }

        [DisplayName("Estado")]
        public string EstadoBadgeColor
        {
            get
            {
                return Estado switch
                {
                    "Aprobada" => "#27ae60",
                    "Rechazada" => "#e74c3c",
                    "Pendiente" => "#f39c12",
                    _ => "#95a5a6"
                };
            }
        }

        [DisplayName("Meses trabajados")]
        public int MesesTrabajados
        {
            get
            {
                var diferencia = DateTime.Now - FechaContratacionEmpleado;
                return (int)(diferencia.TotalDays / 30);
            }
        }

        [DisplayName("Días acumulados")]
        public decimal DiasAcumulados
        {
            get
            {
                // 1 día por mes trabajado
                return MesesTrabajados;
            }
        }

        [DisplayName("Días tomados")]
        public decimal DiasTomados { get; set; }

        [DisplayName("Antigüedad")]
        public string AntiguedadTexto
        {
            get
            {
                var anos = MesesTrabajados / 12;
                var meses = MesesTrabajados % 12;

                if (anos > 0 && meses > 0)
                    return $"{anos} año(s) y {meses} mes(es)";
                else if (anos > 0)
                    return $"{anos} año(s)";
                else
                    return $"{meses} mes(es)";
            }
        }

        [DisplayName("Días después de solicitud")]
        public decimal DiasTemporales
        {
            get
            {
                return DiasDisponibles - DiasSolicitados;
            }
        }

        [DisplayName("Puede solicitar")]
        public bool PuedeSolicitar
        {
            get
            {
                return DiasSolicitados <= DiasDisponibles;
            }
        }

        [DisplayName("Duración en días calendario")]
        public int DuracionCalendario
        {
            get
            {
                return (FechaFin - FechaInicio).Days + 1;
            }
        }

        [DisplayName("Tiempo de respuesta")]
        public string TiempoRespuesta
        {
            get
            {
                if (!FechaRespuesta.HasValue)
                    return "Sin respuesta";

                var dias = (FechaRespuesta.Value - FechaSolicitud).Days;

                if (dias == 0)
                    return "Mismo día";
                else if (dias == 1)
                    return "1 día";
                else
                    return $"{dias} días";
            }
        }

        // Método para calcular días laborables
        public int CalcularDiasLaborables()
        {
            int diasLaborables = 0;
            DateTime fechaActual = FechaInicio;

            while (fechaActual <= FechaFin)
            {
                // Excluir domingos (0 = domingo)
                if (fechaActual.DayOfWeek != DayOfWeek.Sunday)
                {
                    diasLaborables++;
                }
                fechaActual = fechaActual.AddDays(1);
            }

            return diasLaborables;
        }

        // Método para calcular días disponibles de un empleado
        public static decimal CalcularDiasDisponibles(DateTime fechaContratacion, decimal diasTomados)
        {
            var mesesTrabajados = (int)((DateTime.Now - fechaContratacion).TotalDays / 30);
            var diasAcumulados = mesesTrabajados; // 1 día por mes
            return diasAcumulados - diasTomados;
        }
    }
}