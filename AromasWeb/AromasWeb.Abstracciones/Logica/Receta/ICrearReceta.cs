//using AromasWeb.Abstracciones.ModeloUI;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AromasWeb.Abstracciones.Logica.Receta
//{
//    public interface ICrearReceta
//    {
//        int Crear(Receta receta);
//    }
//}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RecetaUI = AromasWeb.Abstracciones.ModeloUI.Receta;

namespace AromasWeb.Abstracciones.Logica.Receta
{
    public interface ICrearReceta
    {
        Task<int> Crear(RecetaUI laReceta);
    }
}