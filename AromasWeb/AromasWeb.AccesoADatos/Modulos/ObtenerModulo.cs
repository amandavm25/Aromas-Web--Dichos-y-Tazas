using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Modulos
{
    public class ObtenerModulo : IObtenerModulo
    {
        public Abstracciones.ModeloUI.Modulo Obtener(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var moduloAD = contexto.Modulo
                        .FirstOrDefault(m => m.IdModulo == id);

                    if (moduloAD == null)
                    {
                        return null;
                    }

                    return ConvertirObjetoParaUI(moduloAD);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener módulo: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Modulo ConvertirObjetoParaUI(ModuloAD moduloAD)
        {
            return new Abstracciones.ModeloUI.Modulo
            {
                IdModulo = moduloAD.IdModulo,
                Nombre = moduloAD.Nombre,
                Descripcion = moduloAD.Descripcion,
                Estado = moduloAD.Estado
            };
        }

        // Método estático para obtener ID por nombre (usado en bitácora)
        public static int ObtenerIdPorNombre(string nombreModulo)
        {
            var listar = new ListarModulos();
            var modulos = listar.BuscarPorNombre(nombreModulo);
            var modulo = modulos.FirstOrDefault(m =>
                m.Nombre.ToLower() == nombreModulo.ToLower() &&
                m.Estado == true);

            if (modulo == null)
                throw new Exception($"No se encontró el módulo '{nombreModulo}' en la base de datos.");

            return modulo.IdModulo;
        }
    }
}