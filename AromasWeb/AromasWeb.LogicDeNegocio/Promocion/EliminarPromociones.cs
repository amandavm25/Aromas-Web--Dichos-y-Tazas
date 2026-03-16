
using AromasWeb.Abstracciones.Logica.Promocion;


namespace AromasWeb.LogicaDeNegocio.Promocion
{
    public class EliminarPromociones : IEliminarPromociones
    {
        private IEliminarPromociones _eliminarPromocionesAD;

        public EliminarPromociones()
        {
            _eliminarPromocionesAD = new AccesoADatos.Promocion.EliminarPromociones();
        }

        public void Ejecutar(int idPromocion)
        {
            _eliminarPromocionesAD.Ejecutar(idPromocion);
        }
    }
}