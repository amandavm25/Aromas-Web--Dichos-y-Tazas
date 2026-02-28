using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.Empleados
{
    public class CrearEmpleado : ICrearEmpleado
    {
        private Contexto _contexto;

        public CrearEmpleado()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(Abstracciones.ModeloUI.Empleado empleado)
        {
            try
            {
                // Validar que no exista un empleado con la misma identificación
                bool identificacionExiste = await _contexto.Empleado
                    .AnyAsync(e => e.Identificacion == empleado.Identificacion);
                if (identificacionExiste)
                {
                    throw new Exception("Ya existe un empleado registrado con esa identificación");
                }

                // Validar que no exista un empleado con el mismo correo
                bool correoExiste = await _contexto.Empleado
                    .AnyAsync(e => e.Correo == empleado.Correo);
                if (correoExiste)
                {
                    throw new Exception("Ya existe un empleado registrado con ese correo electrónico");
                }

                // Validar que el rol exista
                bool rolExiste = await _contexto.Rol
                    .AnyAsync(r => r.IdRol == empleado.IdRol);
                if (!rolExiste)
                {
                    throw new Exception("El rol seleccionado no existe");
                }

                EmpleadoAD empleadoAGuardar = ConvertirObjetoParaAD(empleado);
                _contexto.Empleado.Add(empleadoAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar empleado: {ex.Message}");
                throw;
            }
        }

        private EmpleadoAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Empleado empleado)
        {
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

            return new EmpleadoAD
            {
                IdRol = empleado.IdRol,
                Identificacion = empleado.Identificacion?.Trim(),
                Nombre = empleado.Nombre?.Trim(),
                Apellidos = empleado.Apellidos?.Trim(),
                Correo = empleado.Correo?.Trim().ToLower(),
                Telefono = empleado.Telefono?.Trim(),
                Cargo = empleado.Cargo?.Trim(),
                FechaContratacion = fechaContratacionUtc, // ⭐ Usar la fecha convertida a UTC
                Contrasena = empleado.Contrasena,
                Estado = empleado.Estado,
                ContactoEmergencia = empleado.ContactoEmergencia?.Trim(),
                Alergias = empleado.Alergias?.Trim(),
                Medicamentos = empleado.Medicamentos?.Trim()
            };
        }
    }
}
