using System.Threading.Tasks;
using CategoriaInsumoUI = AromasWeb.Abstracciones.ModeloUI.CategoriaInsumo;

namespace AromasWeb.Abstracciones.Logica.CategoriaInsumo
{
    public interface ICrearCategoriaInsumo
    {
        Task<int> Crear(CategoriaInsumoUI categoriaInsumo);
    }
}