using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Empleado
    {
        public int IdEmpleado { get; set; }

        [DisplayName("Rol")]
        [Required(ErrorMessage = "El rol es requerido")]
        public int IdRol { get; set; }

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

        [DisplayName("Cargo")]
        [Required(ErrorMessage = "El cargo es requerido")]
        [StringLength(100, ErrorMessage = "El cargo no puede exceder 100 caracteres")]
        public string Cargo { get; set; }

        [DisplayName("Fecha de contratación")]
        [Required(ErrorMessage = "La fecha de contratación es requerida")]
        [DataType(DataType.Date)]
        public DateTime FechaContratacion { get; set; }

        [DisplayName("Contraseña")]
        [DataType(DataType.Password)]
        public string Contrasena { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        // Propiedades de navegación
        [DisplayName("Rol")]
        public string NombreRol { get; set; }

        // Propiedades calculadas
        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Estado ? "Activo" : "Inactivo";
            }
        }

        [DisplayName("Fecha de contratación")]
        public string FechaContratacionFormateada
        {
            get
            {
                return FechaContratacion.ToString("dd/MM/yyyy");
            }
        }

        [DisplayName("Meses trabajados")]
        public int MesesTrabajados
        {
            get
            {
                var diferencia = DateTime.Now - FechaContratacion;
                return (int)(diferencia.TotalDays / 30);
            }
        }

        [DisplayName("Años trabajados")]
        public string AnosTrabajados
        {
            get
            {
                var anos = MesesTrabajados / 12;
                var meses = MesesTrabajados % 12;

                if (anos > 0)
                    return $"{anos} año(s) y {meses} mes(es)";
                else
                    return $"{meses} mes(es)";
            }
        }

        [DisplayName("Es empleado antiguo")]
        public bool EsEmpleadoAntiguo
        {
            get
            {
                return MesesTrabajados >= 12;
            }
        }
    }
}