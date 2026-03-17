using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Roles
{
    public class ObtenerRol : IObtenerRol
    {
        public Abstracciones.ModeloUI.Rol Obtener(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var rolAD = contexto.Rol
                        .FirstOrDefault(r => r.IdRol == id);

                    if (rolAD == null)
                    {
                        return null;
                    }

                    return ConvertirObjetoParaUI(rolAD);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener rol: {ex.Message}");
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

        // Método estático para obtener ID por nombre (usado en bitácora)
        public static int ObtenerIdPorNombre(string nombreRol)
        {
            var listar = new ListarRoles();
            var roles = listar.BuscarPorNombre(nombreRol);
            var rol = roles.FirstOrDefault(r =>
                r.Nombre.ToLower() == nombreRol.ToLower() &&
                r.Estado == true);

            if (rol == null)
                throw new Exception($"No se encontró el rol '{nombreRol}' en la base de datos.");

            return rol.IdRol;
        }
    }
}