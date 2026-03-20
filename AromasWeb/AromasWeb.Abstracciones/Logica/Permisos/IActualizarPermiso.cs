using PermisoUI = AromasWeb.Abstracciones.ModeloUI.Permiso;

namespace AromasWeb.Abstracciones.Logica.Permiso
{
    public interface IActualizarPermiso
    {
        int Actualizar(PermisoUI permiso);
    }
}