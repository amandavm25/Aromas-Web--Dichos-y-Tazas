using CategoriaInsumoUI = AromasWeb.Abstracciones.ModeloUI.CategoriaInsumo;

namespace AromasWeb.Abstracciones.Logica.CategoriaInsumo
{
    public interface IObtenerCategoriaInsumo
    {
        CategoriaInsumoUI Obtener(int id);
    }
}