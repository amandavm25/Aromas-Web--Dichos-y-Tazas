using PromocionUI = AromasWeb.Abstracciones.ModeloUI.Promocion;
using System;
using System.Collections.Generic;

namespace AromasWeb.Abstracciones.Logica.Promocion
{
    public interface IListarPromociones
    {
        List<PromocionUI> Obtener();
        List<PromocionUI> BuscarPorNombre(string nombre);
        List<PromocionUI> BuscarPorTipo(int idTipoPromocion);
        List<PromocionUI> BuscarPorVigencia(string vigencia);
        PromocionUI ObtenerPorId(int id);
        List<PromocionUI> ObtenerVigentes();
        List<PromocionUI> ObtenerPorFecha(DateTime fecha);
    }
}