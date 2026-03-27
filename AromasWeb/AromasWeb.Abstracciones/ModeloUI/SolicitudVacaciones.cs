using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class SolicitudVacaciones
    {
        public const int DiasAnticipacionCancelacion = 3;

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
        public string FechaSolicitudFormateada => FechaSolicitud.ToString("dd/MM/yyyy");

        [DisplayName("Fecha de inicio")]
        public string FechaInicioFormateada => FechaInicio.ToString("dd/MM/yyyy");

        [DisplayName("Fecha de fin")]
        public string FechaFinFormateada => FechaFin.ToString("dd/MM/yyyy");

        [DisplayName("Fecha de respuesta")]
        public string FechaRespuestaFormateada => FechaRespuesta?.ToString("dd/MM/yyyy") ?? "Pendiente";

        [DisplayName("Período de vacaciones")]
        public string PeriodoVacaciones => $"{FechaInicioFormateada} - {FechaFinFormateada}";

        public string EstadoBadgeClass => Estado switch
        {
            "Aprobada" => "success",
            "Rechazada" => "danger",
            "Pendiente" => "warning",
            "Cancelada" => "secondary",
            _ => "secondary"
        };

        public string EstadoBadgeColor => Estado switch
        {
            "Aprobada" => "var(--green)",
            "Rechazada" => "var(--red)",
            "Pendiente" => "var(--yellow)",
            "Cancelada" => "var(--gray)",
            _ => "var(--gray)"
        };

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
        public decimal DiasAcumulados => MesesTrabajados; // 1 día por mes trabajado

        [DisplayName("Días tomados")]
        public decimal DiasTomados { get; set; }

        [DisplayName("Antigüedad")]
        public string AntiguedadTexto
        {
            get
            {
                var anos = MesesTrabajados / 12;
                var meses = MesesTrabajados % 12;
                if (anos > 0 && meses > 0) return $"{anos} año(s) y {meses} mes(es)";
                if (anos > 0) return $"{anos} año(s)";
                return $"{meses} mes(es)";
            }
        }

        [DisplayName("Días después de solicitud")]
        public decimal DiasTemporales => DiasDisponibles - DiasSolicitados;

        [DisplayName("Puede solicitar")]
        public bool PuedeSolicitar => DiasSolicitados <= DiasDisponibles;

        [DisplayName("Duración en días calendario")]
        public int DuracionCalendario => (FechaFin - FechaInicio).Days + 1;

        [DisplayName("Tiempo de respuesta")]
        public string TiempoRespuesta
        {
            get
            {
                if (!FechaRespuesta.HasValue) return "Sin respuesta";
                var dias = (FechaRespuesta.Value - FechaSolicitud).Days;
                if (dias == 0) return "Mismo día";
                if (dias == 1) return "1 día";
                return $"{dias} días";
            }
        }

        [DisplayName("Puede cancelar")]
        public bool PuedeCancelar
        {
            get
            {
                if (Estado == "Cancelada" || Estado == "Rechazada")
                    return false;

                int diasRestantes = (FechaInicio.Date - DateTime.Now.Date).Days;
                return diasRestantes >= DiasAnticipacionCancelacion;
            }
        }

        [DisplayName("Motivo sin cancelación")]
        public string MotivoPuedeCancelar
        {
            get
            {
                if (Estado == "Cancelada") return "La solicitud ya está cancelada.";
                if (Estado == "Rechazada") return "La solicitud ya fue rechazada.";

                int dias = (FechaInicio.Date - DateTime.Now.Date).Days;
                if (dias < DiasAnticipacionCancelacion)
                    return $"Solo puedes cancelar con al menos {DiasAnticipacionCancelacion} días de anticipación. Quedan {dias} día(s).";

                return string.Empty;
            }
        }

        // Métodos

        public int CalcularDiasLaborables()
        {
            int diasLaborables = 0;
            DateTime fechaActual = FechaInicio;
            while (fechaActual <= FechaFin)
            {
                if (fechaActual.DayOfWeek != DayOfWeek.Sunday)
                    diasLaborables++;
                fechaActual = fechaActual.AddDays(1);
            }
            return diasLaborables;
        }

        public static decimal CalcularDiasDisponibles(DateTime fechaContratacion, decimal diasTomados)
        {
            var mesesTrabajados = (int)((DateTime.Now - fechaContratacion).TotalDays / 30);
            return mesesTrabajados - diasTomados; // 1 día por mes
        }
    }
}