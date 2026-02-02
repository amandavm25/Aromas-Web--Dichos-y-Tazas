//using AromasWeb.Abstracciones.Logica.Receta;
//using AromasWeb.AccesoADatos.Modelos;
//using Microsoft.EntityFrameworkCore;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace AromasWeb.AccesoADatos.Receta
//{
//    internal class EliminarReceta
//    {
//    }
//}


using AromasWeb.Abstracciones.Logica.Receta;
using AromasWeb.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace AromasWeb.AccesoADatos.Receta
{
    public class EliminarReceta : IEliminarReceta
    {
        private Contexto _contexto;

        public EliminarReceta()
        {
            _contexto = new Contexto();
        }

        public int Eliminar(int id)
        {
            try
            {
                RecetaAD recetaAEliminar = _contexto.Receta
                    .FirstOrDefault(r => r.IdReceta == id);

                if (recetaAEliminar == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Receta con ID {id} no encontrada");
                    return 0;
                }

                // Guardar información antes de eliminar para registro
                var infoReceta = new
                {
                    recetaAEliminar.IdReceta,
                    recetaAEliminar.Nombre,
                    recetaAEliminar.IdCategoriaReceta,
                    recetaAEliminar.Descripcion,
                    recetaAEliminar.CantidadPorciones,
                    recetaAEliminar.PrecioVenta,
                    recetaAEliminar.CostoTotal,
                    recetaAEliminar.Disponibilidad
                };

                _contexto.Receta.Remove(recetaAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                // Aquí podrías agregar auditoría si la implementas
                // if (cantidadDeDatosEliminados > 0)
                // {
                //     _auditoria.RegistrarEliminacion("Receta", id, infoReceta);
                // }

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("No se puede eliminar la receta porque tiene registros relacionados (ingredientes, pedidos, etc.)", dbEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar receta: {ex.Message}");
                throw;
            }
        }
    }
}