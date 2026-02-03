using AromasWeb.Abstracciones.Logica.CategoriaReceta;
using AromasWeb.AccesoADatos.Modelos;
using System.Linq;

namespace AromasWeb.AccesoADatos.CategoriasReceta
{
    public class ObtenerCategoriaReceta : IObtenerCategoriaReceta
    {
        private Contexto _contexto;

        public ObtenerCategoriaReceta()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.CategoriaReceta Obtener(int id)
        {
            var categoriaAD = _contexto.CategoriaReceta
                .FirstOrDefault(c => c.IdCategoriaReceta == id);

            if (categoriaAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(categoriaAD);
        }

        private Abstracciones.ModeloUI.CategoriaReceta ConvertirObjetoParaUI(CategoriaRecetaAD categoriaAD)
        {
            return new Abstracciones.ModeloUI.CategoriaReceta
            {
                IdCategoriaReceta = categoriaAD.IdCategoriaReceta,
                Nombre = categoriaAD.Nombre,
                Descripcion = categoriaAD.Descripcion,
                Estado = categoriaAD.Estado
            };
        }
    }
}