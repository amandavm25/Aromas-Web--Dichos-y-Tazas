using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.ModeloUI;

namespace AromasWeb.LogicaDeNegocio.Receta
{
    public class CrearReceta : ICrearReceta
    {
        private ICrearReceta _crearReceta;

        public CrearReceta()
        {
            _crearReceta = new AccesoADatos.Receta.CrearReceta();
        }

        public int Crear(Receta receta)
        {
            int resultado = _crearReceta.Crear(receta);
            return resultado;
        }
    }
}