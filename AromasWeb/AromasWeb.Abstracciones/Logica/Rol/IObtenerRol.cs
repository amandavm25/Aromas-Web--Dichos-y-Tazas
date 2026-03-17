using RolUI = AromasWeb.Abstracciones.ModeloUI.Rol;

namespace AromasWeb.Abstracciones.Logica.Rol
{
    public interface IObtenerRol
    {
        RolUI Obtener(int id);
    }
}