using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Reserva
    {
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
        public string FechaFormateada
        {
            get
            {
                return Fecha.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Hora")]
        public string HoraFormateada
        {
            get
            {
                return Hora.ToString(@"hh\:mm");
            }
        }

        [DisplayName("Fecha y hora completa")]
        public string FechaHoraCompleta
        {
            get
            {
                return $"{FechaFormateada} {HoraFormateada}";
            }
        }

        [DisplayName("Fecha de creación")]
        public string FechaCreacionFormateada
        {
            get
            {
                return FechaCreacion.ToString("dd/MM/yyyy HH:mm");
            }
        }

        [DisplayName("Es futura")]
        public bool EsFutura
        {
            get
            {
                var fechaHoraReserva = Fecha.Date + Hora;
                return fechaHoraReserva > DateTime.Now;
            }
        }

        [DisplayName("Es hoy")]
        public bool EsHoy
        {
            get
            {
                return Fecha.Date == DateTime.Now.Date;
            }
        }

        [DisplayName("Días restantes")]
        public int? DiasRestantes
        {
            get
            {
                if (EsFutura)
                {
                    return (Fecha.Date - DateTime.Now.Date).Days;
                }
                return null;
            }
        }

        [DisplayName("Color de estado")]
        public string ColorEstado
        {
            get
            {
                return Estado switch
                {
                    "Pendiente" => "var(--yellow)",
                    "Confirmada" => "var(--green)",
                    "Completada" => "#3498db",
                    "Cancelada" => "var(--red)",
                    _ => "var(--gray)"
                };
            }
        }

        [DisplayName("Icono de estado")]
        public string IconoEstado
        {
            get
            {
                return Estado switch
                {
                    "Pendiente" => "fa-clock",
                    "Confirmada" => "fa-check-circle",
                    "Completada" => "fa-check-double",
                    "Cancelada" => "fa-times-circle",
                    _ => "fa-question-circle"
                };
            }
        }

        [DisplayName("Tiene observaciones")]
        public bool TieneObservaciones
        {
            get
            {
                return !string.IsNullOrWhiteSpace(Observaciones);
            }
        }

        [DisplayName("Días hasta la reserva")]
        public int? DiasHastaReserva
        {
            get
            {
                if (EsFutura)
                {
                    return (Fecha.Date - DateTime.Now.Date).Days;
                }
                return null;
            }
        }

        [DisplayName("Puede cancelar")]
        public bool PuedeCancelar
        {
            get
            {
                // Puede cancelar si:
                // 1. La reserva es futura o es hoy
                // 2. El estado NO es "Cancelada" ni "Completada"
                return (EsFutura || EsHoy) &&
                       Estado != "Cancelada" &&
                       Estado != "Completada";
            }
        }

        [DisplayName("Puede modificar")]
        public bool PuedeModificar
        {
            get
            {
                // Puede cancelar si:
                // 1. La reserva es futura o es hoy
                // 2. El estado NO es "Cancelada" ni "Completada"
                return (EsFutura || EsHoy) &&
                       Estado != "Cancelada" &&
                       Estado != "Completada";
            }
        }

        public bool EsPasada { get; set; }
    }
}