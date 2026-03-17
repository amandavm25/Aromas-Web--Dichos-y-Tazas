using System.Collections.Generic;
using ModuloUI = AromasWeb.Abstracciones.ModeloUI.Modulo;

namespace AromasWeb.Abstracciones.Logica.Modulo
{
    public interface IListarModulos
    {
        List<ModuloUI> Obtener();
        List<ModuloUI> BuscarPorNombre(string nombre);
        List<ModuloUI> BuscarPorEstado(bool estado);
        ModuloUI ObtenerPorId(int id);
    }
}