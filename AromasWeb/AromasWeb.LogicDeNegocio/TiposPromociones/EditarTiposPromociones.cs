using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using TipoPromocionUI = AromasWeb.Abstracciones.ModeloUI.TipoPromocion;

namespace AromasWeb.LogicaDeNegocio.TiposPromociones
{
    public class EditarTiposPromociones : IEditarTiposPromociones
    {
        private IEditarTiposPromociones _editarTiposPromocionesAD;

        public EditarTiposPromociones()
        {
            _editarTiposPromocionesAD = new AromasWeb.AccesoADatos.TiposPromociones.EditarTiposPromociones();
        }

        public void Ejecutar(TipoPromocionUI tipoPromocion)
        {
            _editarTiposPromocionesAD.Ejecutar(tipoPromocion);
        }
    }
}
