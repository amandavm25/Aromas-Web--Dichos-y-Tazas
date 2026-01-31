using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        [DisplayName("Identificación")]
        [Required(ErrorMessage = "La identificación es requerida")]
        [StringLength(20, ErrorMessage = "La identificación no puede exceder 20 caracteres")]
        public string Identificacion { get; set; }

        [DisplayName("Nombre")]
        [Required(ErrorMessage = "El nombre es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string Nombre { get; set; }

        [DisplayName("Apellidos")]
        [Required(ErrorMessage = "Los apellidos es requerido")]
        [StringLength(200, ErrorMessage = "Los apellidos no puede exceder 200 caracteres")]
        public string Apellidos { get; set; }

        [DisplayName("Correo electrónico")]
        [Required(ErrorMessage = "El correo es requerido")]
        [EmailAddress(ErrorMessage = "El correo no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        public string Correo { get; set; }

        [DisplayName("Teléfono")]
        [Required(ErrorMessage = "El teléfono es requerido")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        public string Telefono { get; set; }

        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        [DisplayName("Fecha de registro")]
        public DateTime FechaRegistro { get; set; }

        [DisplayName("Última reserva")]
        public DateTime? UltimaReserva { get; set; }

        // Propiedades calculadas
        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Estado ? "Activo" : "Inactivo";
            }
        }

        [DisplayName("Fecha de registro")]
        public string FechaRegistroFormateada
        {
            get
            {
                return FechaRegistro.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Última reserva")]
        public string UltimaReservaFormateada
        {
            get
            {
                return UltimaReserva?.ToString("dd/MM/yyyy") ?? "Sin reservas";
            }
        }

        [DisplayName("Días desde última reserva")]
        public int? DiasDesdeUltimaReserva
        {
            get
            {
                if (UltimaReserva.HasValue)
                {
                    return (DateTime.Now - UltimaReserva.Value).Days;
                }
                return null;
            }
        }

        [DisplayName("Cliente frecuente")]
        public bool EsClienteFrecuente
        {
            get
            {
                return UltimaReserva.HasValue && DiasDesdeUltimaReserva <= 30;
            }
        }
    }
}