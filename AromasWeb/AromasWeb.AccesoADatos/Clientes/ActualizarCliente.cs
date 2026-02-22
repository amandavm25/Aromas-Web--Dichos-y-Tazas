using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.AccesoADatos.Clientes
{
    public class ActualizarCliente : IActualizarCliente
    {
        private Contexto _contexto;

        public ActualizarCliente()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(ClienteUI cliente)
        {
            try
            {
                var clienteExistente = _contexto.Cliente
                    .FirstOrDefault(c => c.IdCliente == cliente.IdCliente);

                if (clienteExistente == null)
                {
                    return 0;
                }

                // Validar que no exista otro cliente con la misma identificación
                bool identificacionExiste = _contexto.Cliente
                    .Any(c => c.Identificacion == cliente.Identificacion &&
                              c.IdCliente != cliente.IdCliente);
                if (identificacionExiste)
                {
                    throw new Exception("Ya existe otro cliente registrado con esa identificación");
                }

                // Validar que no exista otro cliente con el mismo correo
                bool correoExiste = _contexto.Cliente
                    .Any(c => c.Correo == cliente.Correo &&
                              c.IdCliente != cliente.IdCliente);
                if (correoExiste)
                {
                    throw new Exception("Ya existe otro cliente registrado con ese correo electrónico");
                }

                // Actualizar campos
                clienteExistente.Identificacion = cliente.Identificacion?.Trim();
                clienteExistente.Nombre = cliente.Nombre?.Trim();
                clienteExistente.Apellidos = cliente.Apellidos?.Trim();
                clienteExistente.Correo = cliente.Correo?.Trim().ToLower();
                clienteExistente.Telefono = cliente.Telefono?.Trim();
                clienteExistente.Estado = cliente.Estado;

                // Solo actualizar contraseña si se proporcionó una nueva
                if (!string.IsNullOrWhiteSpace(cliente.Contrasena))
                {
                    clienteExistente.Contrasena = cliente.Contrasena;
                }

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar cliente: {ex.Message}");
                throw;
            }
        }
    }
}