using System.Collections.Generic;
using InsumoUI = AromasWeb.Abstracciones.ModeloUI.Insumo;

namespace AromasWeb.Abstracciones.Logica.Insumo
{
    public interface IListarInsumos
    {
        List<InsumoUI> Obtener();
        List<InsumoUI> BuscarPorNombre(string nombre);
        List<InsumoUI> BuscarPorCategoria(int idCategoria);
        InsumoUI ObtenerPorId(int id);
        List<InsumoUI> ObtenerBajoStock();

        void Crear(InsumoUI insumo);

        void Actualizar(InsumoUI insumo);

    }
}