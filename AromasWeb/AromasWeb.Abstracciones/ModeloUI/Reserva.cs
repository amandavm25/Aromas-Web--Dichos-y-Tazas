using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Reserva
    {
        public const int DiasAnticipacionCancelacion = 2;

        public int IdReserva { get; set; }

        [DisplayName("Cliente")]
        [Required(ErrorMessage = "El cliente es requerido")]
        public int IdCliente { get; set; }

        [DisplayName("Nombre del cliente")]
        public string NombreCliente { get; set; }

        [DisplayName("Teléfono del cliente")]
        public string TelefonoCliente { get; set; }

        [DisplayName("Cantidad de personas")]
        [Required(ErrorMessage = "La cantidad de personas es requerida")]
        [Range(1, 20, ErrorMessage = "La cantidad debe estar entre 1 y 20 personas")]
        public int CantidadPersonas { get; set; }

        [DisplayName("Fecha")]
        [Required(ErrorMessage = "La fecha es requerida")]
        [DataType(DataType.Date)]
        public DateTime Fecha { get; set; }

        [DisplayName("Hora")]
        [Required(ErrorMessage = "La hora es requerida")]
        [DataType(DataType.Time)]
        public TimeSpan Hora { get; set; }

        [DisplayName("Estado")]
        [Required(ErrorMessage = "El estado es requerido")]
        [StringLength(20)]
        public string Estado { get; set; }

        [DisplayName("Observaciones")]
        [StringLength(500, ErrorMessage = "Las observaciones no pueden exceder 500 caracteres")]
        public string Observaciones { get; set; }

        [DisplayName("Fecha de creación")]
        public DateTime FechaCreacion { get; set; }

        // Propiedades calculadas

        [DisplayName("Fecha")]
        public string FechaFormateada => Fecha.ToString("dd/MM/yyyy");

        [DisplayName("Hora")]
        public string HoraFormateada => Hora.ToString(@"hh\:mm");

        [DisplayName("Fecha y hora completa")]
        public string FechaHoraCompleta => $"{FechaFormateada} {HoraFormateada}";

        [DisplayName("Fecha de creación")]
        public string FechaCreacionFormateada => FechaCreacion.ToString("dd/MM/yyyy HH:mm");

        [DisplayName("Es futura")]
        public bool EsFutura => (Fecha.Date + Hora) > DateTime.Now;

        [DisplayName("Es hoy")]
        public bool EsHoy => Fecha.Date == DateTime.Now.Date;

        [DisplayName("Es pasada")]
        public bool EsPasada => (Fecha.Date + Hora) <= DateTime.Now;

        [DisplayName("Días hasta la reserva")]
        public int? DiasHastaReserva => EsFutura ? (int?)(Fecha.Date - DateTime.Now.Date).Days : null;

        [DisplayName("Puede cancelar")]
        public bool PuedeCancelar
        {
            get
            {
                if (Estado == "Cancelada" || Estado == "Completada")
                    return false;

                int diasRestantes = (Fecha.Date - DateTime.Now.Date).Days;
                return diasRestantes >= DiasAnticipacionCancelacion;
            }
        }

        [DisplayName("Días para poder cancelar")]
        public int DiasParaCancelar => (Fecha.Date - DateTime.Now.Date).Days - DiasAnticipacionCancelacion;

        [DisplayName("Motivo sin cancelación")]
        public string MotivoPuedeCancelar
        {
            get
            {
                if (Estado == "Cancelada") return "La reserva ya está cancelada.";
                if (Estado == "Completada") return "La reserva ya fue completada.";

                int dias = (Fecha.Date - DateTime.Now.Date).Days;
                if (dias < DiasAnticipacionCancelacion)
                    return $"Solo puedes cancelar con al menos {DiasAnticipacionCancelacion} días de anticipación. Quedan {dias} día(s).";

                return string.Empty;
            }
        }

        [DisplayName("Puede modificar")]
        public bool PuedeModificar => PuedeCancelar;

        public string EstadoBadgeClass => Estado switch
        {
            "Confirmada" => "success",
            "Cancelada" => "danger",
            "Pendiente" => "warning",
            "Completada" => "secondary",
            _ => "secondary"
        };

        public string ColorEstado => Estado switch
        {
            "Pendiente" => "var(--yellow)",
            "Confirmada" => "var(--green)",
            "Completada" => "var(--gold)",
            "Cancelada" => "var(--red)",
            _ => "var(--gray)"
        };

        [DisplayName("Icono de estado")]
        public string IconoEstado => Estado switch
        {
            "Pendiente" => "fa-clock",
            "Confirmada" => "fa-check-circle",
            "Completada" => "fa-check-double",
            "Cancelada" => "fa-times-circle",
            _ => "fa-question-circle"
        };

        [DisplayName("Tiene observaciones")]
        public bool TieneObservaciones => !string.IsNullOrWhiteSpace(Observaciones);
    }
}