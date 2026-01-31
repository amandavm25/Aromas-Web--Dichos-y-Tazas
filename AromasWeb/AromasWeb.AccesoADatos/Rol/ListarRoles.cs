using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Roles
{
    public class ListarRoles : IListarRoles
    {
        public List<Abstracciones.ModeloUI.Rol> Obtener()
        {
            // USAR USING PARA DISPOSE AUTOMÁTICO DEL CONTEXTO
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RolAD> rolesAD = contexto.Rol
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return rolesAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    // Log del error
                    Console.WriteLine($"Error al obtener roles: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    // Si hay inner exception, también logearla
                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw; // Re-lanzar la excepción
                }
            }
        }

        public List<Abstracciones.ModeloUI.Rol> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RolAD> rolesAD = contexto.Rol
                        .Where(r => r.Nombre.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return rolesAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar roles por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Rol> BuscarPorEstado(bool estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<RolAD> rolesAD = contexto.Rol
                        .Where(r => r.Estado == estado)
                        .OrderBy(r => r.Nombre)
                        .ToList();
                    return rolesAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar roles por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Rol ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var rolAD = contexto.Rol.FirstOrDefault(r => r.IdRol == id);
                    return rolAD != null ? ConvertirObjetoParaUI(rolAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener rol por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Rol ConvertirObjetoParaUI(RolAD rolAD)
        {
            return new Abstracciones.ModeloUI.Rol
            {
                IdRol = rolAD.IdRol,
                Nombre = rolAD.Nombre,
                Descripcion = rolAD.Descripcion,
                Estado = rolAD.Estado
            };
        }
    }
}