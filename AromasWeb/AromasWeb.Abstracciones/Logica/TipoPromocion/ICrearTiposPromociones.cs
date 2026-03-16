using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TipoPromocionUI = AromasWeb.Abstracciones.ModeloUI.TipoPromocion;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.Abstracciones.Logica.TipoPromocion
{
    public interface ICrearTiposPromociones
    {
        void Ejecutar(TipoPromocionUI tipoPromocion);
    }
}
