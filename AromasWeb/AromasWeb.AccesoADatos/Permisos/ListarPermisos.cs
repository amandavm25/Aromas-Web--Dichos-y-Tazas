using AromasWeb.Abstracciones.Logica.Permiso;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Permisos
{
    public class ListarPermisos : IListarPermisos
    {
        public List<Abstracciones.ModeloUI.Permiso> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var permisosAD = contexto.Permiso
                        .OrderBy(p => p.Nombre)
                        .ToList();

                    return permisosAD.Select(p => ConvertirConModulo(p, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener permisos: {ex.Message}");
                    if (ex.InnerException != null)
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Permiso> ObtenerPorModulo(int idModulo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var permisosAD = contexto.Permiso
                        .Where(p => p.IdModulo == idModulo)
                        .OrderBy(p => p.Nombre)
                        .ToList();

                    return permisosAD.Select(p => ConvertirConModulo(p, contexto)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener permisos por módulo: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Permiso> ObtenerPorRol(int idRol)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var idsPermisos = contexto.RolPermiso
                        .Where(rp => rp.IdRol == idRol)
                        .Select(rp => rp.IdPermiso)
                        .ToList();

                    var permisosAD = contexto.Permiso
                        .Where(p => idsPermisos.Contains(p.IdPermiso))
                        .OrderBy(p => p.Nombre)
                        .ToList();

                    return permisosAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener permisos por rol: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Permiso ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var permisoAD = contexto.Permiso.FirstOrDefault(p => p.IdPermiso == id);
                    if (permisoAD == null) return null;
                    return ConvertirConModulo(permisoAD, contexto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener permiso por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public bool AsignarPermisosARol(int idRol, List<int> idsPermisos)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Eliminar permisos actuales del rol
                    var permisosActuales = contexto.RolPermiso
                        .Where(rp => rp.IdRol == idRol)
                        .ToList();

                    contexto.RolPermiso.RemoveRange(permisosActuales);

                    // Agregar los nuevos permisos
                    foreach (var idPermiso in idsPermisos)
                    {
                        contexto.RolPermiso.Add(new RolPermisoAD
                        {
                            IdRol = idRol,
                            IdPermiso = idPermiso
                        });
                    }

                    // SaveChanges ya maneja la transacción internamente
                    contexto.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR ASIGNAR: {ex.Message}");
                    Console.WriteLine($"INNER: {ex.InnerException?.Message}");
                    Console.WriteLine($"STACK: {ex.StackTrace}");
                    return false;
                }
            }
        }

        public List<int> ObtenerPermisosDeRol(int idRol)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    return contexto.RolPermiso
                        .Where(rp => rp.IdRol == idRol)
                        .Select(rp => rp.IdPermiso)
                        .ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener permisos de rol: {ex.Message}");
                    throw;
                }
            }
        }

        // Convierte e incluye el módulo relacionado
        private Abstracciones.ModeloUI.Permiso ConvertirConModulo(PermisoAD permisoAD, Contexto contexto)
        {
            var permiso = ConvertirObjetoParaUI(permisoAD);

            var modulo = contexto.Modulo.FirstOrDefault(m => m.IdModulo == permisoAD.IdModulo);
            if (modulo != null)
            {
                permiso.Modulo = new Abstracciones.ModeloUI.Modulo
                {
                    IdModulo = modulo.IdModulo,
                    Nombre = modulo.Nombre,
                    Descripcion = modulo.Descripcion,
                    Estado = modulo.Estado
                };
            }

            return permiso;
        }

        private Abstracciones.ModeloUI.Permiso ConvertirObjetoParaUI(PermisoAD permisoAD)
        {
            return new Abstracciones.ModeloUI.Permiso
            {
                IdPermiso = permisoAD.IdPermiso,
                IdModulo = permisoAD.IdModulo,
                Nombre = permisoAD.Nombre,
                Descripcion = permisoAD.Descripcion,
                Estado = permisoAD.Estado
            };
        }
    }
}