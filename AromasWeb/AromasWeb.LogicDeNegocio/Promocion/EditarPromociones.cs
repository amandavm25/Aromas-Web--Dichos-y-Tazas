using AromasWeb.Abstracciones.Logica.Promocion;
using PromocionUI = AromasWeb.Abstracciones.ModeloUI.Promocion;
using AromasWeb.Abstracciones.ModeloUI;


namespace AromasWeb.LogicaDeNegocio.Promocion
{
    public class EditarPromociones : IEditarPromociones
    {
        private IEditarPromociones _editarPromocionesAD;

        public EditarPromociones()
        {
            _editarPromocionesAD = new AromasWeb.AccesoADatos.Promocion.EditarPromociones();
        }

        public void Ejecutar(PromocionUI promocion)
        {
            _editarPromocionesAD.Ejecutar(promocion);
        }
    }
}