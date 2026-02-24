using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.AccesoADatos.Modelos;
using System.Linq;

namespace AromasWeb.AccesoADatos.Empleados
{
    public class ObtenerEmpleado : IObtenerEmpleado
    {
        private Contexto _contexto;

        public ObtenerEmpleado()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Empleado Obtener(int idEmpleado)
        {
            var empleadoAD = _contexto.Empleado
                .FirstOrDefault(e => e.IdEmpleado == idEmpleado);

            if (empleadoAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(empleadoAD);
        }

        private Abstracciones.ModeloUI.Empleado ConvertirObjetoParaUI(EmpleadoAD empleadoAD)
        {
            // Obtener el nombre del rol
            string nombreRol = "Sin rol";
            try
            {
                var rol = _contexto.Rol?.FirstOrDefault(r => r.IdRol == empleadoAD.IdRol);
                if (rol != null)
                {
                    nombreRol = rol.Nombre;
                }
            }
            catch
            {
                nombreRol = empleadoAD.IdRol == 1 ? "Administrador" : "Empleado";
            }

            return new Abstracciones.ModeloUI.Empleado
            {
                IdEmpleado = empleadoAD.IdEmpleado,
                IdRol = empleadoAD.IdRol,
                NombreRol = nombreRol,
                Identificacion = empleadoAD.Identificacion,
                Nombre = empleadoAD.Nombre,
                Apellidos = empleadoAD.Apellidos,
                Correo = empleadoAD.Correo,
                Contrasena = empleadoAD.Contrasena,
                Telefono = empleadoAD.Telefono,
                Cargo = empleadoAD.Cargo,
                FechaContratacion = empleadoAD.FechaContratacion,
                Estado = empleadoAD.Estado,
                ContactoEmergencia = empleadoAD.ContactoEmergencia,
                Alergias = empleadoAD.Alergias,
                Medicamentos = empleadoAD.Medicamentos
            };
        }
    }
}