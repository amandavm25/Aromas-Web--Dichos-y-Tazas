using System.Collections.Generic;
using RolUI = AromasWeb.Abstracciones.ModeloUI.Rol;

namespace AromasWeb.Abstracciones.Logica.Rol
{
    public interface IListarRoles
    {
        List<RolUI> Obtener();
        List<RolUI> BuscarPorNombre(string nombre);
        List<RolUI> BuscarPorEstado(bool estado);
        RolUI ObtenerPorId(int id);
    }
}