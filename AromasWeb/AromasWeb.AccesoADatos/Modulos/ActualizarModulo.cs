using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Modulos
{
    public class ActualizarModulo : IActualizarModulo
    {
        public int Actualizar(Abstracciones.ModeloUI.Modulo modulo)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var moduloAD = contexto.Modulo
                        .FirstOrDefault(m => m.IdModulo == modulo.IdModulo);

                    if (moduloAD == null)
                    {
                        throw new Exception("Módulo no encontrado");
                    }

                    // Validar nombre único (excluyendo el registro actual)
                    var existente = contexto.Modulo
                        .FirstOrDefault(m => m.Nombre.ToLower() == modulo.Nombre.ToLower()
                                           && m.IdModulo != modulo.IdModulo);

                    if (existente != null)
                    {
                        throw new Exception("Ya existe otro módulo con ese nombre");
                    }

                    // Actualizar campos
                    moduloAD.Nombre = modulo.Nombre;
                    moduloAD.Descripcion = modulo.Descripcion;
                    moduloAD.Estado = modulo.Estado;

                    contexto.SaveChanges();
                    return moduloAD.IdModulo;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al actualizar módulo: {ex.Message}");
                    throw;
                }
            }
        }
    }
}