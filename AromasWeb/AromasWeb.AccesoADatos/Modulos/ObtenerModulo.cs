using AromasWeb.Abstracciones.Logica.Modulo;
using System.Linq;

namespace AromasWeb.AccesoADatos.Modulos
{
    // Resuelve el ID de un módulo por nombre usando el ListarModulos existente.
    // Uso: ObtenerModulo.ObtenerIdPorNombre("Empleados")
    public static class ObtenerModulo
    {
        public static int ObtenerIdPorNombre(string nombreModulo)
        {
            var listar = new ListarModulos();
            var modulos = listar.BuscarPorNombre(nombreModulo);
            var modulo = modulos.FirstOrDefault(m =>
                               m.Nombre.ToLower() == nombreModulo.ToLower() &&
                               m.Estado == true);

            if (modulo == null)
                throw new System.Exception($"No se encontró el módulo '{nombreModulo}' en la base de datos.");

            return modulo.IdModulo;
        }
    }
}