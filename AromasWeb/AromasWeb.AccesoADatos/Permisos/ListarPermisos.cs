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
                    var permisosAD = contexto.Permiso.ToList();
                    var permisos = new List<Abstracciones.ModeloUI.Permiso>();

                    foreach (var permisoAD in permisosAD)
                    {
                        var permiso = ConvertirObjetoParaUI(permisoAD);

                        // Obtener módulo
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

                        permisos.Add(permiso);
                    }

                    return permisos;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener permisos: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

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
                        .ToList();

                    var permisos = new List<Abstracciones.ModeloUI.Permiso>();

                    foreach (var permisoAD in permisosAD)
                    {
                        var permiso = ConvertirObjetoParaUI(permisoAD);

                        // Obtener módulo
                        var modulo = contexto.Modulo.FirstOrDefault(m => m.IdModulo == permisoAD.IdModulo);
                        if (modulo != null)
                        {
                            permiso.Modulo = new Abstracciones.ModeloUI.Modulo
                            {
                                IdModulo = modulo.IdModulo,
                                Nombre = modulo.Nombre
                            };
                        }

                        permisos.Add(permiso);
                    }

                    return permisos;
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

                    if (permisoAD == null)
                        return null;

                    var permiso = ConvertirObjetoParaUI(permisoAD);

                    // Obtener módulo
                    var modulo = contexto.Modulo.FirstOrDefault(m => m.IdModulo == permisoAD.IdModulo);
                    if (modulo != null)
                    {
                        permiso.Modulo = new Abstracciones.ModeloUI.Modulo
                        {
                            IdModulo = modulo.IdModulo,
                            Nombre = modulo.Nombre
                        };
                    }

                    return permiso;
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
                using (var transaction = contexto.Database.BeginTransaction())
                {
                    try
                    {
                        // Eliminar permisos actuales
                        var permisosActuales = contexto.RolPermiso
                            .Where(rp => rp.IdRol == idRol)
                            .ToList();

                        contexto.RolPermiso.RemoveRange(permisosActuales);

                        // Agregar nuevos permisos
                        foreach (var idPermiso in idsPermisos)
                        {
                            contexto.RolPermiso.Add(new RolPermisoAD
                            {
                                IdRol = idRol,
                                IdPermiso = idPermiso
                            });
                        }

                        contexto.SaveChanges();
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        Console.WriteLine($"Error al asignar permisos: {ex.Message}");
                        return false;
                    }
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

        private Abstracciones.ModeloUI.Permiso ConvertirObjetoParaUI(PermisoAD permisoAD)
        {
            return new Abstracciones.ModeloUI.Permiso
            {
                IdPermiso = permisoAD.IdPermiso,
                IdModulo = permisoAD.IdModulo,
                Nombre = permisoAD.Nombre
            };
        }
    }
}