using AromasWeb.Abstracciones.Logica.Empleado;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Empleados
{
    public class ListarEmpleados : IListarEmpleados
    {
        public List<Abstracciones.ModeloUI.Empleado> Obtener()
        {
            // USAR USING PARA DISPOSE AUTOMÁTICO DEL CONTEXTO
            using (var contexto = new Contexto())
            {
                try
                {
                    List<EmpleadoAD> empleadosAD = contexto.Empleado
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .ToList();
                    return empleadosAD.Select(e => ConvertirObjetoParaUI(e, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    // Log del error
                    Console.WriteLine($"Error al obtener empleados: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    // Si hay inner exception, también logearla
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Empleado> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<EmpleadoAD> empleadosAD = contexto.Empleado
                        .Where(e => (e.Nombre + " " + e.Apellidos).ToLower().Contains(nombre.ToLower()))
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .ToList();
                    return empleadosAD.Select(e => ConvertirObjetoParaUI(e, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar empleados por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Empleado> BuscarPorIdentificacion(string identificacion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<EmpleadoAD> empleadosAD = contexto.Empleado
                        .Where(e => e.Identificacion.Contains(identificacion))
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .ToList();
                    return empleadosAD.Select(e => ConvertirObjetoParaUI(e, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar empleados por identificación: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Empleado> BuscarPorCargo(string cargo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<EmpleadoAD> empleadosAD = contexto.Empleado
                        .Where(e => e.Cargo.ToLower().Contains(cargo.ToLower()))
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .ToList();
                    return empleadosAD.Select(e => ConvertirObjetoParaUI(e, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar empleados por cargo: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Empleado> BuscarPorEstado(bool estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<EmpleadoAD> empleadosAD = contexto.Empleado
                        .Where(e => e.Estado == estado)
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .ToList();
                    return empleadosAD.Select(e => ConvertirObjetoParaUI(e, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar empleados por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Empleado> BuscarPorRol(int idRol)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<EmpleadoAD> empleadosAD = contexto.Empleado
                        .Where(e => e.IdRol == idRol)
                        .OrderBy(e => e.Nombre)
                        .ThenBy(e => e.Apellidos)
                        .ToList();
                    return empleadosAD.Select(e => ConvertirObjetoParaUI(e, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar empleados por rol: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Empleado ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var empleadoAD = contexto.Empleado.FirstOrDefault(e => e.IdEmpleado == id);
                    return empleadoAD != null ? ConvertirObjetoParaUI(empleadoAD, contexto) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener empleado por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Empleado ConvertirObjetoParaUI(EmpleadoAD empleadoAD, Contexto contexto)
        {
            // Obtener el nombre del rol si existe la tabla Rol
            string nombreRol = "Sin rol";
            try
            {
                var rol = contexto.Rol?.FirstOrDefault(r => r.IdRol == empleadoAD.IdRol);
                if (rol != null)
                {
                    nombreRol = rol.Nombre;
                }
            }
            catch
            {
                // Si no existe la tabla Rol o hay error, usar valor por defecto
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
                Estado = empleadoAD.Estado
            };
        }
    }
}