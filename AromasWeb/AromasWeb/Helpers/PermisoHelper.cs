using System.Collections.Generic;
using System.Text.Json;
using Microsoft.AspNetCore.Http;

namespace AromasWeb.Helpers
{
    public static class PermisoHelper
    {
        public static bool TienePermiso(ISession session, string nombrePermiso)
        {
            var tipo = session.GetString("UsuarioTipo");
            if (tipo == "admin") return true;
            if (tipo != "empleado") return false;

            try
            {
                var mapa = JsonSerializer.Deserialize<Dictionary<string, int>>(
                    session.GetString("MapaPermisos") ?? "{}"
                );
                var permisos = JsonSerializer.Deserialize<List<int>>(
                    session.GetString("PermisosRol") ?? "[]"
                );

                if (mapa != null && permisos != null && mapa.TryGetValue(nombrePermiso, out int id))
                    return permisos.Contains(id);
            }
            catch { }

            return false;
        }
    }
}