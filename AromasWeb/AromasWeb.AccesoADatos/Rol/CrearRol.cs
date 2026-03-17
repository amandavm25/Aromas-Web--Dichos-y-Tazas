using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.Roles
{
    public class CrearRol : ICrearRol
    {
        public async Task<int> Crear(Abstracciones.ModeloUI.Rol rol)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Validar que el nombre no exista
                    var existente = contexto.Rol
                        .FirstOrDefault(r => r.Nombre.ToLower() == rol.Nombre.ToLower());

                    if (existente != null)
                    {
                        throw new Exception("Ya existe un rol con ese nombre");
                    }

                    var rolAD = new RolAD
                    {
                        Nombre = rol.Nombre,
                        Descripcion = rol.Descripcion,
                        Estado = rol.Estado
                    };

                    contexto.Rol.Add(rolAD);
                    await contexto.SaveChangesAsync();

                    return rolAD.IdRol;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear rol: {ex.Message}");
                    throw;
                }
            }
        }
    }
}