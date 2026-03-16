using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.ModeloUI;
using TipoPromocionUI = AromasWeb.Abstracciones.ModeloUI.TipoPromocion;

namespace AromasWeb.Abstracciones.Logica.TipoPromocion
{
    public interface IEditarTiposPromociones
    {
        void Ejecutar(TipoPromocionUI tipoPromocion);
    }
}