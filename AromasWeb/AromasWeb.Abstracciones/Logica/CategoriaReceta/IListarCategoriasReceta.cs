using System.Collections.Generic;
using CategoriaRecetaUI = AromasWeb.Abstracciones.ModeloUI.CategoriaReceta;

namespace AromasWeb.Abstracciones.Logica.CategoriaReceta
{
    public interface IListarCategoriasReceta
    {
        List<CategoriaRecetaUI> Obtener();
        List<CategoriaRecetaUI> BuscarPorNombre(string nombre);
        CategoriaRecetaUI ObtenerPorId(int id);
    }
}