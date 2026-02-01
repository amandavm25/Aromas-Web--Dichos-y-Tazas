using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.Abstracciones.Logica.Inventario
{
    public interface IRegistrarInsumos
    {
        bool ExisteInsumo(string nombre);
        void Crear(AromasWeb.Abstracciones.ModeloUI.Insumo insumo);
    }
}
