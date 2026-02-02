using AromasWeb.Abstracciones.Logica.Receta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.Recetas
{
    public class EliminarReceta : Abstracciones.Logica.Receta.IEliminarReceta
    {
        private IEliminarReceta _eliminarReceta;

        public EliminarReceta()
        {
            _eliminarReceta = new AccesoADatos.Receta.EliminarReceta();
        }

        public int Eliminar(int id)
        {
            int resultado = _eliminarReceta.Eliminar(id);
            return resultado;
        }
    }
}