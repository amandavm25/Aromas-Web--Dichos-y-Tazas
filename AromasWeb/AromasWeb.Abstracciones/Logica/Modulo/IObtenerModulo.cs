using ModuloUI = AromasWeb.Abstracciones.ModeloUI.Modulo;

namespace AromasWeb.Abstracciones.Logica.Modulo
{
    public interface IObtenerModulo
    {
        ModuloUI Obtener(int id);
    }
}