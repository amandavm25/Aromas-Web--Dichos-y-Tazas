using System.Collections.Generic;
using CategoriaInsumoUI = AromasWeb.Abstracciones.ModeloUI.CategoriaInsumo;

namespace AromasWeb.Abstracciones.Logica.CategoriaInsumo
{
    public interface IListarCategoriasInsumo
    {
        List<CategoriaInsumoUI> Obtener();
        List<CategoriaInsumoUI> BuscarPorNombre(string nombre);
        CategoriaInsumoUI ObtenerPorId(int id);
    }
}