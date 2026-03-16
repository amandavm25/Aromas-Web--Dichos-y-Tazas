using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.Logica.TipoPromocion;
using AromasWeb.AccesoADatos.Modelos;

namespace AromasWeb.AccesoADatos.TiposPromociones
{
    public class CrearTiposPromociones : ICrearTiposPromociones
    {
        public void Ejecutar(Abstracciones.ModeloUI.TipoPromocion tipoPromocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var tipoPromocionAD = new TipoPromocionAD
                    {
                        NombreTipo = tipoPromocion.Nombre,
                        Descripcion = tipoPromocion.Descripcion,
                        Estado = true
                    };

                    contexto.TipoPromocion.Add(tipoPromocionAD);
                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al crear tipo de promoción: {ex.Message}");
                    throw;
                }
            }
        }
    }
}


