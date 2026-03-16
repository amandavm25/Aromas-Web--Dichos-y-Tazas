using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.ModeloUI;
using PromocionUI = AromasWeb.Abstracciones.ModeloUI.Promocion;


namespace AromasWeb.Abstracciones.Logica.Promocion
{
    public interface ICrearPromociones
    {
        void Ejecutar(PromocionUI promocion);
    }
}
