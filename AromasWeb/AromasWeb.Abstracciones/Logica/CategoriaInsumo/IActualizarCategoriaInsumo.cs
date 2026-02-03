using CategoriaInsumoUI = AromasWeb.Abstracciones.ModeloUI.CategoriaInsumo;

namespace AromasWeb.Abstracciones.Logica.CategoriaInsumo
{
    public interface IActualizarCategoriaInsumo
    {
        int Actualizar(CategoriaInsumoUI categoriaInsumo);
    }
}