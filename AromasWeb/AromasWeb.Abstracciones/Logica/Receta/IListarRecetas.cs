using System.Collections.Generic;
using RecetaUI = AromasWeb.Abstracciones.ModeloUI.Receta;

namespace AromasWeb.Abstracciones.Logica.Receta
{
    public interface IListarRecetas
    {
        List<RecetaUI> Obtener();
        List<RecetaUI> BuscarPorNombre(string nombre);
        List<RecetaUI> BuscarPorCategoria(int idCategoria);
        RecetaUI ObtenerPorId(int id);
        List<RecetaUI> ObtenerDisponibles();
        List<RecetaUI> ObtenerNoDisponibles();
    }
}