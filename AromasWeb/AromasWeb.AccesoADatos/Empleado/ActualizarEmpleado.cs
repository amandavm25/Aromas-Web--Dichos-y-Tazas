using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Empleados
{
    public class ActualizarEmpleado : IActualizarEmpleado
    {
        private Contexto _contexto;

        public ActualizarEmpleado()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.Empleado empleado)
        {
            try
            {
                var empleadoExistente = _contexto.Empleado
                    .FirstOrDefault(e => e.IdEmpleado == empleado.IdEmpleado);

                if (empleadoExistente == null)
                {
                    return 0;
                }

                // Validar que no exista otro empleado con la misma identificación
                bool identificacionExiste = _contexto.Empleado
                    .Any(e => e.Identificacion == empleado.Identificacion &&
                              e.IdEmpleado != empleado.IdEmpleado);
                if (identificacionExiste)
                {
                    throw new Exception("Ya existe otro empleado registrado con esa identificación");
                }

                // Validar que no exista otro empleado con el mismo correo
                bool correoExiste = _contexto.Empleado
                    .Any(e => e.Correo == empleado.Correo &&
                              e.IdEmpleado != empleado.IdEmpleado);
                if (correoExiste)
                {
                    throw new Exception("Ya existe otro empleado registrado con ese correo electrónico");
                }

                // ⭐ Convertir FechaContratacion a UTC
                DateTime fechaContratacionUtc = empleado.FechaContratacion;
                if (fechaContratacionUtc.Kind == DateTimeKind.Unspecified)
                {
                    fechaContratacionUtc = DateTime.SpecifyKind(fechaContratacionUtc, DateTimeKind.Utc);
                }
                else if (fechaContratacionUtc.Kind == DateTimeKind.Local)
                {
                    fechaContratacionUtc = fechaContratacionUtc.ToUniversalTime();
                }

                // Actualizar campos
                empleadoExistente.IdRol = empleado.IdRol;
                empleadoExistente.Identificacion = empleado.Identificacion?.Trim();
                empleadoExistente.Nombre = empleado.Nombre?.Trim();
                empleadoExistente.Apellidos = empleado.Apellidos?.Trim();
                empleadoExistente.Correo = empleado.Correo?.Trim().ToLower();
                empleadoExistente.Telefono = empleado.Telefono?.Trim();
                empleadoExistente.Cargo = empleado.Cargo?.Trim();
                empleadoExistente.FechaContratacion = fechaContratacionUtc; // ⭐ Usar la fecha convertida a UTC
                empleadoExistente.Estado = empleado.Estado;

                // Solo actualizar contraseña si se proporcionó una nueva
                if (!string.IsNullOrWhiteSpace(empleado.Contrasena))
                {
                    empleadoExistente.Contrasena = empleado.Contrasena;
                }

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar empleado: {ex.Message}");
                throw;
            }
        }
    }
}