using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PromocionUI = AromasWeb.Abstracciones.ModeloUI.Promocion;
using AromasWeb.AccesoADatos.Promocion;
using AromasWeb.Abstracciones.Logica.Promocion;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Promocion
{
    public class CrearPromociones : ICrearPromociones
    {
        private ICrearPromociones _crearPromocionesAD;

        public CrearPromociones()
        {
            _crearPromocionesAD = new AromasWeb.AccesoADatos.Promocion.CrearPromociones();
        }

        public void Ejecutar(PromocionUI promocion)
        {
            _crearPromocionesAD.Ejecutar(promocion);
        }
    }
}

