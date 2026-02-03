using AromasWeb.Abstracciones.Logica.CategoriaInsumo;

namespace AromasWeb.LogicaDeNegocio.CategoriasInsumo
{
    public class EliminarCategoriaInsumo : IEliminarCategoriaInsumo
    {
        private IEliminarCategoriaInsumo _eliminarCategoriaInsumo;

        public EliminarCategoriaInsumo()
        {
            _eliminarCategoriaInsumo = new AccesoADatos.CategoriasInsumo.EliminarCategoriaInsumo();
        }

        public int Eliminar(int id)
        {
            int resultado = _eliminarCategoriaInsumo.Eliminar(id);
            return resultado;
        }
    }
}