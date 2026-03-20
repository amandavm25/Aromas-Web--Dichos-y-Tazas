using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Permisos
{
    public class EliminarPermiso : IEliminarPermiso
    {
        public int Eliminar(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var permiso = contexto.Permiso.FirstOrDefault(p => p.IdPermiso == id);

                    if (permiso == null)
                    {
                        return 0;
                    }

                    // Validar que el permiso no esté asignado a algún rol
                    bool tieneRolesAsignados = contexto.RolPermiso.Any(rp => rp.IdPermiso == id);
                    if (tieneRolesAsignados)
                    {
                        throw new Exception("No se puede eliminar el permiso porque está asignado a uno o más roles");
                    }

                    contexto.Permiso.Remove(permiso);
                    int cantidadDeDatosEliminados = contexto.SaveChanges();

                    return cantidadDeDatosEliminados;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al eliminar permiso: {ex.Message}");
                    throw;
                }
            }
        }
    }
}