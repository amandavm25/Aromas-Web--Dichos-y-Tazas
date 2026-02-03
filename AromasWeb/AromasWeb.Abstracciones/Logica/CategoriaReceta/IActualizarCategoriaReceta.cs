using CategoriaRecetaUI = AromasWeb.Abstracciones.ModeloUI.CategoriaReceta;

namespace AromasWeb.Abstracciones.Logica.CategoriaReceta
{
    public interface IActualizarCategoriaReceta
    {
        int Actualizar(CategoriaRecetaUI categoriaReceta);
    }
}