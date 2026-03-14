using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AromasWeb.Abstracciones.ModeloUI
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }

        [DisplayName("Empleado")]
        [Required(ErrorMessage = "El empleado es requerido")]
        public int IdEmpleado { get; set; }

        [DisplayName("Módulo")]
        [Required(ErrorMessage = "El módulo es requerido")]
        public int IdModulo { get; set; }

        [DisplayName("Acción")]
        [Required(ErrorMessage = "La acción es requerida")]
        [StringLength(200, ErrorMessage = "La acción no puede exceder 200 caracteres")]
        public string Accion { get; set; }

        [DisplayName("Tabla Afectada")]
        [StringLength(100, ErrorMessage = "La tabla afectada no puede exceder 100 caracteres")]
        public string TablaAfectada { get; set; }

        [DisplayName("Descripción")]
        [StringLength(1000, ErrorMessage = "La descripción no puede exceder 1000 caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Datos Anteriores")]
        public string DatosAnteriores { get; set; }

        [DisplayName("Datos Nuevos")]
        public string DatosNuevos { get; set; }

        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        // Propiedades de navegación
        [DisplayName("Empleado")]
        public string NombreEmpleado { get; set; }

        [DisplayName("Módulo")]
        public string NombreModulo { get; set; }

        // Propiedades calculadas
        [DisplayName("Fecha")]
        public string FechaFormateada
        {
            get { return Fecha.ToString("dd/MM/yyyy HH:mm:ss"); }
        }

        // Acciones estándar
        public static class Acciones
        {
            public const string Crear = "Crear";
            public const string Actualizar = "Actualizar";
            public const string Eliminar = "Eliminar";

            // Autenticación
            public const string Login = "Login";
            public const string Logout = "Logout";

            // Empleados
            public const string CambiarEstado = "Cambiar Estado";

            // Asistencia
            public const string RegistrarEntrada = "Registrar Entrada";
            public const string RegistrarSalida = "Registrar Salida";

            // Planilla
            public const string Calcular = "Calcular";
            public const string MarcarPagado = "Marcar Pagado";

            // Tarifas
            public const string ActualizarTarifa = "Actualizar Tarifa";
            public const string FinalizarTarifa = "Finalizar Tarifa";

            // Vacaciones (EMP-13)
            public const string Aprobar = "Aprobar";
            public const string Rechazar = "Rechazar";

            // Inventario
            public const string RegistrarMovimiento = "Registrar Movimiento";

            // Permisos
            public const string AsignarPermisos = "Asignar Permiso";
        }
    }
}