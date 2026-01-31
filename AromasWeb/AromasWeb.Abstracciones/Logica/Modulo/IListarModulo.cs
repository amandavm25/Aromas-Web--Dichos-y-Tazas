using ModuloUI = AromasWeb.Abstracciones.ModeloUI.Modulo;
using System.Collections.Generic;

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