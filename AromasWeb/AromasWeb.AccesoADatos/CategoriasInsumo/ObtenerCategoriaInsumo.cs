using AromasWeb.Abstracciones.Logica.CategoriaInsumo;
using AromasWeb.AccesoADatos.Modelos;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasInsumo
{
    public class ObtenerCategoriaInsumo : IObtenerCategoriaInsumo
    {
        private Contexto _contexto;

        public ObtenerCategoriaInsumo()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.CategoriaInsumo Obtener(int id)
        {
            var categoriaAD = _contexto.CategoriaInsumo
                .FirstOrDefault(c => c.IdCategoria == id);

            if (categoriaAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(categoriaAD);
        }

        private Abstracciones.ModeloUI.CategoriaInsumo ConvertirObjetoParaUI(CategoriaInsumoAD categoriaAD)
        {
            return new Abstracciones.ModeloUI.CategoriaInsumo
            {
                IdCategoria = categoriaAD.IdCategoria,
                NombreCategoria = categoriaAD.NombreCategoria,
                Descripcion = categoriaAD.Descripcion,
                Estado = categoriaAD.Estado
            };
        }
    }
}