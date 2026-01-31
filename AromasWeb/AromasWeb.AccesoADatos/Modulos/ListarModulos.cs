using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Modulos
{
    public class ListarModulos : IListarModulos
    {
        public List<Abstracciones.ModeloUI.Modulo> Obtener()
        {
            // USAR USING PARA DISPOSE AUTOMÁTICO DEL CONTEXTO
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ModuloAD> modulosAD = contexto.Modulo
                        .OrderBy(m => m.Nombre)
                        .ToList();
                    return modulosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    // Log del error
                    Console.WriteLine($"Error al obtener módulos: {ex.Message}");
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

        public List<Abstracciones.ModeloUI.Modulo> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ModuloAD> modulosAD = contexto.Modulo
                        .Where(m => m.Nombre.ToLower().Contains(nombre.ToLower()) ||
                                   (m.Descripcion != null && m.Descripcion.ToLower().Contains(nombre.ToLower())))
                        .OrderBy(m => m.Nombre)
                        .ToList();
                    return modulosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar módulos por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Modulo> BuscarPorEstado(bool estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ModuloAD> modulosAD = contexto.Modulo
                        .Where(m => m.Estado == estado)
                        .OrderBy(m => m.Nombre)
                        .ToList();
                    return modulosAD.Select(m => ConvertirObjetoParaUI(m)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar módulos por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Modulo ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var moduloAD = contexto.Modulo.FirstOrDefault(m => m.IdModulo == id);
                    return moduloAD != null ? ConvertirObjetoParaUI(moduloAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener módulo por ID: {ex.Message}");
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
    }
}