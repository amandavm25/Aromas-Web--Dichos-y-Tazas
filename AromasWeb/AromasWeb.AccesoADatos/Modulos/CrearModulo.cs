using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AromasWeb.AccesoADatos.Modulos
{
    public class CrearModulo : ICrearModulo
    {
        public async Task<int> Crear(Abstracciones.ModeloUI.Modulo modulo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Validar que el nombre no exista
                    var existente = contexto.Modulo
                        .FirstOrDefault(m => m.Nombre.ToLower() == modulo.Nombre.ToLower());

                    if (existente != null)
                    {
                        throw new Exception("Ya existe un módulo con ese nombre");
                    }

                    var moduloAD = new ModuloAD
                    {
                        Nombre = modulo.Nombre,
                        Descripcion = modulo.Descripcion,
                        Estado = modulo.Estado
                    };

                    contexto.Modulo.Add(moduloAD);
                    await contexto.SaveChangesAsync();

                    return moduloAD.IdModulo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear módulo: {ex.Message}");
                    throw;
                }
            }
        }
    }
}