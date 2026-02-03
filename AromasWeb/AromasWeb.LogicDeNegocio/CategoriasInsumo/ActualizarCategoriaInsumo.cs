using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.CategoriasInsumo
{
    public class ActualizarCategoriaInsumo : IActualizarCategoriaInsumo
    {
        private IActualizarCategoriaInsumo _actualizarCategoriaInsumo;

        public ActualizarCategoriaInsumo()
        {
            _actualizarCategoriaInsumo = new AccesoADatos.CategoriasInsumo.ActualizarCategoriaInsumo();
        }

        public int Actualizar(CategoriaInsumo categoriaInsumo)
        {
            int resultado = _actualizarCategoriaInsumo.Actualizar(categoriaInsumo);
            return resultado;
        }
    }
}