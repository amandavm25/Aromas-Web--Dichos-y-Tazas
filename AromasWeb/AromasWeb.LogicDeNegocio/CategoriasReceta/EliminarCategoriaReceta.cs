using AromasWeb.Abstracciones.Logica.CategoriaReceta;

namespace AromasWeb.LogicaDeNegocio.CategoriasReceta
{
    public class EliminarCategoriaReceta : IEliminarCategoriaReceta
    {
        private IEliminarCategoriaReceta _eliminarCategoriaReceta;

        public EliminarCategoriaReceta()
        {
            _eliminarCategoriaReceta = new AccesoADatos.CategoriasReceta.EliminarCategoriaReceta();
        }

        public int Eliminar(int id)
        {
            int resultado = _eliminarCategoriaReceta.Eliminar(id);
            return resultado;
        }
    }
}