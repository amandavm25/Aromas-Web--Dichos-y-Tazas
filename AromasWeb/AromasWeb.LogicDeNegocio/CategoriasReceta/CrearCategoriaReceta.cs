using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.CategoriasReceta
{
    public class CrearCategoriaReceta : ICrearCategoriaReceta
    {
        private ICrearCategoriaReceta _crearCategoriaReceta;

        public CrearCategoriaReceta()
        {
            _crearCategoriaReceta = new AccesoADatos.CategoriasReceta.CrearCategoriaReceta();
        }

        public async Task<int> Crear(CategoriaReceta categoriaReceta)
        {
            int resultado = await _crearCategoriaReceta.Crear(categoriaReceta);
            return resultado;
        }
    }
}