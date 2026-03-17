using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Roles
{
    public class ActualizarRol : IActualizarRol
    {
        public int Actualizar(Abstracciones.ModeloUI.Rol rol)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var rolAD = contexto.Rol
                        .FirstOrDefault(r => r.IdRol == rol.IdRol);

                    if (rolAD == null)
                    {
                        throw new Exception("Rol no encontrado");
                    }

                    // Validar nombre único (excluyendo el registro actual)
                    var existente = contexto.Rol
                        .FirstOrDefault(r => r.Nombre.ToLower() == rol.Nombre.ToLower()
                                           && r.IdRol != rol.IdRol);

                    if (existente != null)
                    {
                        throw new Exception("Ya existe otro rol con ese nombre");
                    }

                    // Actualizar campos
                    rolAD.Nombre = rol.Nombre;
                    rolAD.Descripcion = rol.Descripcion;
                    rolAD.Estado = rol.Estado;

                    contexto.SaveChanges();
                    return rolAD.IdRol;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar rol: {ex.Message}");
                    throw;
                }
            }
        }
    }
}