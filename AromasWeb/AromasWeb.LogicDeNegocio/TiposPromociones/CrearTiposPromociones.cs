using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using TipoPromocionUI = AromasWeb.Abstracciones.ModeloUI.TipoPromocion;

namespace AromasWeb.LogicaDeNegocio.TiposPromociones
{
    public class CrearTiposPromociones : ICrearTiposPromociones
    {
        private ICrearTiposPromociones _crearTiposPromocionesAD;

        public CrearTiposPromociones()
        {
            _crearTiposPromocionesAD = new AromasWeb.AccesoADatos.TiposPromociones.CrearTiposPromociones();
        }

        public void Ejecutar(TipoPromocionUI tipoPromocion)
        {
            _crearTiposPromocionesAD.Ejecutar(tipoPromocion);
        }
    }
}
