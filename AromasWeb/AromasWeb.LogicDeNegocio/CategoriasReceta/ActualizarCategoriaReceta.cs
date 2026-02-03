using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.CategoriasReceta
{
    public class ActualizarCategoriaReceta : IActualizarCategoriaReceta
    {
        private IActualizarCategoriaReceta _actualizarCategoriaReceta;

        public ActualizarCategoriaReceta()
        {
            _actualizarCategoriaReceta = new AccesoADatos.CategoriasReceta.ActualizarCategoriaReceta();
        }

        public int Actualizar(CategoriaReceta categoriaReceta)
        {
            int resultado = _actualizarCategoriaReceta.Actualizar(categoriaReceta);
            return resultado;
        }
    }
}