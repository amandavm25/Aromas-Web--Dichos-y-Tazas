using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Clientes
{
    public class ListarClientes : IListarClientes
    {
        public List<Abstracciones.ModeloUI.Cliente> Obtener()
        {
            // USAR USING PARA DISPOSE AUTOMÁTICO DEL CONTEXTO
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ClienteAD> clientesAD = contexto.Cliente
                        .OrderBy(c => c.Nombre)
                        .ThenBy(c => c.Apellidos)
                        .ToList();
                    return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    // Log del error
                    Console.WriteLine($"Error al obtener clientes: {ex.Message}");
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

        public List<Abstracciones.ModeloUI.Cliente> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ClienteAD> clientesAD = contexto.Cliente
                        .Where(c => (c.Nombre + " " + c.Apellidos).ToLower().Contains(nombre.ToLower()))
                        .OrderBy(c => c.Nombre)
                        .ThenBy(c => c.Apellidos)
                        .ToList();
                    return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar clientes por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Cliente> BuscarPorIdentificacion(string identificacion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ClienteAD> clientesAD = contexto.Cliente
                        .Where(c => c.Identificacion.Contains(identificacion))
                        .OrderBy(c => c.Nombre)
                        .ThenBy(c => c.Apellidos)
                        .ToList();
                    return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar clientes por identificación: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Cliente> BuscarPorTelefono(string telefono)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ClienteAD> clientesAD = contexto.Cliente
                        .Where(c => c.Telefono.Contains(telefono))
                        .OrderBy(c => c.Nombre)
                        .ThenBy(c => c.Apellidos)
                        .ToList();
                    return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar clientes por teléfono: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Cliente> BuscarPorEstado(bool estado)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<ClienteAD> clientesAD = contexto.Cliente
                        .Where(c => c.Estado == estado)
                        .OrderBy(c => c.Nombre)
                        .ThenBy(c => c.Apellidos)
                        .ToList();
                    return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar clientes por estado: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Cliente ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var clienteAD = contexto.Cliente.FirstOrDefault(c => c.IdCliente == id);
                    return clienteAD != null ? ConvertirObjetoParaUI(clienteAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener cliente por ID: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Cliente ConvertirObjetoParaUI(ClienteAD clienteAD)
        {
            return new Abstracciones.ModeloUI.Cliente
            {
                IdCliente = clienteAD.IdCliente,
                Identificacion = clienteAD.Identificacion,
                Nombre = clienteAD.Nombre,
                Apellidos = clienteAD.Apellidos,
                Correo = clienteAD.Correo,
                Telefono = clienteAD.Telefono,
                Estado = clienteAD.Estado,
                FechaRegistro = clienteAD.FechaRegistro,
                UltimaReserva = null // Se puede calcular con una consulta adicional si es necesario
            };
        }
    }
}