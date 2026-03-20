using PermisoUI = AromasWeb.Abstracciones.ModeloUI.Permiso;

namespace AromasWeb.Abstracciones.Logica.Permiso
{
    public interface ICrearPermiso
    {
        int Crear(PermisoUI permiso);
    }
}