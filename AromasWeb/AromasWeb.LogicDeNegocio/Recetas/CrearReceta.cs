using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.Abstracciones.ModeloUI;
using AromasWeb.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AromasWeb.LogicaDeNegocio.Recetas
{
    public class CrearReceta : ICrearReceta
    {
        private ICrearReceta _crearReceta;

        public CrearReceta()
        {
            _crearReceta = new AccesoADatos.Receta.CrearReceta();
        }

        public async Task<int> Crear(Abstracciones.ModeloUI.Receta laReceta)
        {
            int resultado = await _crearReceta.Crear(laReceta);
            return resultado;
        }
    }
}