using AromasWeb.Abstracciones.Logica.Bitacora;
using AromasWeb.Abstracciones.ModeloUI;
using System;

namespace AromasWeb.LogicaDeNegocio.Bitacoras
{
    public class CrearBitacora : ICrearBitacora
    {
        private readonly ICrearBitacora _crearBitacora;

        public CrearBitacora()
        {
            _crearBitacora = new AccesoADatos.Bitacoras.CrearBitacora();
        }

        // Implementación de la interfaz
        public int Crear(Bitacora bitacora)
        {
            return _crearBitacora.Crear(bitacora);
        }

        // Método de conveniencia — no es parte de la interfaz.
        // Es el que se usa desde los controllers y lógicas de negocio.
        public int RegistrarAccion(
            int idEmpleado,
            int idModulo,
            string accion,
            string tablaAfectada = null,
            string descripcion = null,
            string datosAnteriores = null,
            string datosNuevos = null)
        {
            var bitacora = new Bitacora
            {
                IdEmpleado = idEmpleado,
                IdModulo = idModulo,
                Accion = accion,
                TablaAfectada = tablaAfectada,
                Descripcion = descripcion,
                DatosAnteriores = datosAnteriores,
                DatosNuevos = datosNuevos,
                Fecha = DateTime.UtcNow
            };

            return _crearBitacora.Crear(bitacora);
        }
    }
}