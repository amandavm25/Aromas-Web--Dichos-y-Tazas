using System.Threading.Tasks;
using CategoriaRecetaUI = AromasWeb.Abstracciones.ModeloUI.CategoriaReceta;

namespace AromasWeb.Abstracciones.Logica.CategoriaReceta
{
    public interface ICrearCategoriaReceta
    {
        Task<int> Crear(CategoriaRecetaUI categoriaReceta);
    }
}