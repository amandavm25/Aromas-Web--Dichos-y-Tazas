using AromasWeb.Abstracciones.Logica.Cliente;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Linq;
using System.Threading.Tasks;
using ClienteUI = AromasWeb.Abstracciones.ModeloUI.Cliente;

namespace AromasWeb.AccesoADatos.Clientes
{
    public class CrearCliente : ICrearCliente
    {
        private Contexto _contexto;

        public CrearCliente()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Crear(ClienteUI cliente)
        {
            try
            {
                // Validar que no exista otro cliente con la misma identificación
                bool identificacionExiste = _contexto.Cliente
                    .Any(c => c.Identificacion == cliente.Identificacion);

                if (identificacionExiste)
                {
                    throw new Exception("Ya existe un cliente registrado con esa identificación");
                }

                // Validar que no exista otro cliente con el mismo correo
                bool correoExiste = _contexto.Cliente
                    .Any(c => c.Correo == cliente.Correo);

                if (correoExiste)
                {
                    throw new Exception("Ya existe un cliente registrado con ese correo electrónico");
                }

                // Convertir el objeto de UI a AD
                var clienteAD = ConvertirObjetoParaAD(cliente);

                // Agregar al contexto
                _contexto.Cliente.Add(clienteAD);

                // Guardar cambios
                int cantidadDeDatosInsertados = await _contexto.SaveChangesAsync();

                return cantidadDeDatosInsertados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al crear cliente: {ex.Message}");
                throw;
            }
        }

        private ClienteAD ConvertirObjetoParaAD(ClienteUI cliente)
        {
            // Convertir fecha a UTC
            DateTime fechaRegistroUtc = cliente.FechaRegistro;
            if (fechaRegistroUtc == DateTime.MinValue)
            {
                fechaRegistroUtc = DateTime.UtcNow;
            }
            else if (fechaRegistroUtc.Kind == DateTimeKind.Unspecified)
            {
                fechaRegistroUtc = DateTime.SpecifyKind(fechaRegistroUtc, DateTimeKind.Utc);
            }
            else if (fechaRegistroUtc.Kind == DateTimeKind.Local)
            {
                fechaRegistroUtc = fechaRegistroUtc.ToUniversalTime();
            }

            return new ClienteAD
            {
                Identificacion = cliente.Identificacion?.Trim(),
                Nombre = cliente.Nombre?.Trim(),
                Apellidos = cliente.Apellidos?.Trim(),
                Correo = cliente.Correo?.Trim().ToLower(),
                Telefono = cliente.Telefono?.Trim(),
                Contrasena = cliente.Contrasena,
                Estado = cliente.Estado,
                FechaRegistro = fechaRegistroUtc
            };
        }
    }
}