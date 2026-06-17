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
            using (var contexto = new Contexto())
            {
                try
                {
                    // Validar que no exista otro cliente con la misma identificación
                    string identificacionNormalizada = NormalizarIdentificacion(cliente.Identificacion);
                    bool identificacionExiste = contexto.Cliente
                        .Any(c => c.Identificacion == identificacionNormalizada);

                    if (identificacionExiste)
                    {
                        throw new Exception("Ya existe un cliente registrado con esa identificación");
                    }

                    // Validar que no exista otro cliente con el mismo correo
                    bool correoExiste = contexto.Cliente
                        .Any(c => c.Correo == cliente.Correo);

                    if (correoExiste)
                    {
                        throw new Exception("Ya existe un cliente registrado con ese correo electrónico");
                    }

                    // Convertir el objeto de UI a AD
                    var clienteAD = ConvertirObjetoParaAD(cliente);

                    // Agregar al contexto
                    contexto.Cliente.Add(clienteAD);

                    // Guardar cambios
                    int cantidadDeDatosInsertados = await contexto.SaveChangesAsync();

                    return cantidadDeDatosInsertados;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error al crear cliente: {ex.Message}");
                    throw;
                }
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
                Identificacion = NormalizarIdentificacion(cliente.Identificacion),
                Nombre = cliente.Nombre?.Trim(),
                Apellidos = cliente.Apellidos?.Trim(),
                Correo = cliente.Correo?.Trim().ToLower(),
                Telefono = cliente.Telefono?.Trim(),
                Contrasena = cliente.Contrasena,
                Estado = cliente.Estado,
                FechaRegistro = fechaRegistroUtc
            };
        }

        private string NormalizarIdentificacion(string identificacion)
        {
            if (string.IsNullOrWhiteSpace(identificacion))
                return identificacion;
            // Elimina guiones y espacios, deja solo dígitos (y letras para pasaportes)
            return identificacion.Trim().Replace("-", "").Replace(" ", "");
        }
    }
}