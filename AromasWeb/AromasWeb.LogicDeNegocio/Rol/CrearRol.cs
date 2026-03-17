using AromasWeb.Abstracciones.Logica.Rol;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.Roles
{
    public class CrearRol : ICrearRol
    {
        private ICrearRol _crearRol;

        public CrearRol()
        {
            _crearRol = new AccesoADatos.Roles.CrearRol();
        }

        public async Task<int> Crear(Rol rol)
        {
            return await _crearRol.Crear(rol);
        }
    }
}