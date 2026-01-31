using PermisoUI = AromasWeb.Abstracciones.ModeloUI.Permiso;
using System.Collections.Generic;

namespace AromasWeb.Abstracciones.Logica.Permiso
{
    public interface IListarPermisos
    {
        List<PermisoUI> Obtener();
        List<PermisoUI> ObtenerPorModulo(int idModulo);
        List<PermisoUI> ObtenerPorRol(int idRol);
        PermisoUI ObtenerPorId(int id);
        bool AsignarPermisosARol(int idRol, List<int> idsPermisos);
        List<int> ObtenerPermisosDeRol(int idRol);
    }
}