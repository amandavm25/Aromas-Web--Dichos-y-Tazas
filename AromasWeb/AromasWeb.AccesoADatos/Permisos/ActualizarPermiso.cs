using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Permisos
{
    public class ActualizarPermiso : IActualizarPermiso
    {
        public int Actualizar(Abstracciones.ModeloUI.Permiso permiso)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var permisoExistente = contexto.Permiso
                        .FirstOrDefault(p => p.IdPermiso == permiso.IdPermiso);

                    if (permisoExistente == null)
                        return 0;

                    // Validar que el nombre no sea modificado
                    if (!string.Equals(permisoExistente.Nombre, permiso.Nombre, StringComparison.OrdinalIgnoreCase))
                        throw new Exception("No se puede modificar el nombre del permiso");

                    // Validar que el módulo exista
                    bool moduloExiste = contexto.Modulo.Any(m => m.IdModulo == permiso.IdModulo);
                    if (!moduloExiste)
                        throw new Exception("El módulo seleccionado no existe");

                    // Actualizar solo los campos permitidos (deshabilitando cambio de nombre)
                    permisoExistente.IdModulo = permiso.IdModulo;
                    permisoExistente.Descripcion = permiso.Descripcion?.Trim();
                    permisoExistente.Estado = permiso.Estado;

                    int resultado = contexto.SaveChanges();
                    return resultado;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al actualizar permiso: {ex.Message}");
                    throw;
                }
            }
        }
    }
}