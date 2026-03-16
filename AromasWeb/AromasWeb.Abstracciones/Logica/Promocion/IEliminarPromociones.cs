using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.Abstracciones.Logica.Promocion
{
    public interface IEliminarPromociones
    {
        void Ejecutar(int idpromocion);
    }
}
