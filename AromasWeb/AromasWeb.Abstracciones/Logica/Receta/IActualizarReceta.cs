using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecetaUI = AromasWeb.Abstracciones.ModeloUI.Receta;

namespace AromasWeb.Abstracciones.Logica.Receta
{
    public interface IActualizarReceta
    {
        int Actualizar(RecetaUI receta);
    }
}