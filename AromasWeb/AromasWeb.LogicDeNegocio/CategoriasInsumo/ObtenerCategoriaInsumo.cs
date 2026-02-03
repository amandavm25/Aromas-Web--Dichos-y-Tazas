using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.CategoriasInsumo
{
    public class ObtenerCategoriaInsumo : IObtenerCategoriaInsumo
    {
        private IObtenerCategoriaInsumo _obtenerCategoriaInsumo;

        public ObtenerCategoriaInsumo()
        {
            _obtenerCategoriaInsumo = new AccesoADatos.CategoriasInsumo.ObtenerCategoriaInsumo();
        }

        public CategoriaInsumo Obtener(int id)
        {
            CategoriaInsumo categoria = _obtenerCategoriaInsumo.Obtener(id);
            return categoria;
        }
    }
}