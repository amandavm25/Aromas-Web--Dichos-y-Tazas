using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AromasWeb.Abstracciones.Logica.TipoPromocion;

namespace AromasWeb.AccesoADatos.TiposPromociones
{
    public class EditarTiposPromociones : IEditarTiposPromociones
    {
        public void Ejecutar(Abstracciones.ModeloUI.TipoPromocion tipoPromocion)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var tipoPromocionAD = contexto.TipoPromocion
                        .FirstOrDefault(t => t.IdTipoPromocion == tipoPromocion.IdTipoPromocion);

                    if (tipoPromocionAD == null)
                        throw new Exception("Tipo de promoción no encontrado");

                    tipoPromocionAD.NombreTipo = tipoPromocion.Nombre;
                    tipoPromocionAD.Descripcion = tipoPromocion.Descripcion;
                    tipoPromocionAD.Estado = tipoPromocion.Estado;

                    contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al editar tipo de promoción: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
