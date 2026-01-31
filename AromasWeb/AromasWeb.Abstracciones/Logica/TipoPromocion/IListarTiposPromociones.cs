using TipoPromocionUI = AromasWeb.Abstracciones.ModeloUI.TipoPromocion;
using System.Collections.Generic;

namespace AromasWeb.Abstracciones.Logica.TipoPromocion
{
    public interface IListarTiposPromociones
    {
        List<TipoPromocionUI> Obtener();
        List<TipoPromocionUI> BuscarPorNombre(string nombre);
        TipoPromocionUI ObtenerPorId(int id);
    }
}