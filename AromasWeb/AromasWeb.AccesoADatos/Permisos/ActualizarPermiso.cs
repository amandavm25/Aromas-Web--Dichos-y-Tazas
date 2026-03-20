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

                    // Validar que el módulo exista
                    bool moduloExiste = contexto.Modulo.Any(m => m.IdModulo == permiso.IdModulo);
                    if (!moduloExiste)
                        throw new Exception("El módulo seleccionado no existe");

                    // Validar nombre duplicado excluyendo el propio registro
                    bool nombreExiste = contexto.Permiso
                        .Any(p => p.Nombre.ToLower() == permiso.Nombre.ToLower().Trim() &&
                                  p.IdModulo == permiso.IdModulo &&
                                  p.IdPermiso != permiso.IdPermiso);
                    if (nombreExiste)
                        throw new Exception("Ya existe otro permiso con ese nombre en el módulo seleccionado");

                    permisoExistente.IdModulo = permiso.IdModulo;
                    permisoExistente.Nombre = permiso.Nombre?.Trim();
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