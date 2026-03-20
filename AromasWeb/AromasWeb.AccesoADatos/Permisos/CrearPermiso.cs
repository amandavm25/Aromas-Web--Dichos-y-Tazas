using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Permisos
{
    public class CrearPermiso : ICrearPermiso
    {
        public int Crear(Abstracciones.ModeloUI.Permiso permiso)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Validar que el módulo exista
                    bool moduloExiste = contexto.Modulo.Any(m => m.IdModulo == permiso.IdModulo);
                    if (!moduloExiste)
                        throw new Exception("El módulo seleccionado no existe");

                    // Validar nombre duplicado en el mismo módulo
                    bool nombreExiste = contexto.Permiso
                        .Any(p => p.Nombre.ToLower() == permiso.Nombre.ToLower().Trim() &&
                                  p.IdModulo == permiso.IdModulo);
                    if (nombreExiste)
                        throw new Exception("Ya existe un permiso con ese nombre en el módulo seleccionado");

                    var permisoAGuardar = new PermisoAD
                    {
                        IdModulo = permiso.IdModulo,
                        Nombre = permiso.Nombre?.Trim(),
                        Descripcion = permiso.Descripcion?.Trim(),
                        Estado = permiso.Estado
                    };

                    contexto.Permiso.Add(permisoAGuardar);
                    int resultado = contexto.SaveChanges();
                    return resultado;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al crear permiso: {ex.Message}");
                    throw;
                }
            }
        }
    }
}