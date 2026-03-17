using AromasWeb.Abstracciones.Logica.Modulo;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.Modulos
{
    public class CrearModulo : ICrearModulo
    {
        private ICrearModulo _crearModulo;

        public CrearModulo()
        {
            _crearModulo = new AccesoADatos.Modulos.CrearModulo();
        }

        public async Task<int> Crear(Modulo modulo)
        {
            return await _crearModulo.Crear(modulo);
        }
    }
}