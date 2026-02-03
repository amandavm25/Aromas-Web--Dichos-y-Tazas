using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.CategoriasInsumo
{
    public class CrearCategoriaInsumo : ICrearCategoriaInsumo
    {
        private ICrearCategoriaInsumo _crearCategoriaInsumo;

        public CrearCategoriaInsumo()
        {
            _crearCategoriaInsumo = new AccesoADatos.CategoriasInsumo.CrearCategoriaInsumo();
        }

        public async Task<int> Crear(CategoriaInsumo categoriaInsumo)
        {
            int resultado = await _crearCategoriaInsumo.Crear(categoriaInsumo);
            return resultado;
        }
    }
}