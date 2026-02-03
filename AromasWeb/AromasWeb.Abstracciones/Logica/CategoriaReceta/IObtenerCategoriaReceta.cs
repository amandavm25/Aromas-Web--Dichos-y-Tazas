using CategoriaRecetaUI = AromasWeb.Abstracciones.ModeloUI.CategoriaReceta;

namespace AromasWeb.Abstracciones.Logica.CategoriaReceta
{
    public interface IObtenerCategoriaReceta
    {
        CategoriaRecetaUI Obtener(int id);
    }
}