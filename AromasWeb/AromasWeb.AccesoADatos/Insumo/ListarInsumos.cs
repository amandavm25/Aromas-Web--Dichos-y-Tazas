using AromasWeb.Abstracciones.Logica.Insumo;
using AromasWeb.AccesoADatos.Modelos;
using System.Collections.Generic;
using System.Linq;

namespace AromasWeb.AccesoADatos.Insumos
{
    public class ListarInsumos : IListarInsumos
    {
        public List<Abstracciones.ModeloUI.Insumo> Obtener()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<InsumoAD> insumosAD = contexto.Insumo
                        .OrderBy(i => i.NombreInsumo)
                        .ToList();
                    return insumosAD.Select(i => ConvertirObjetoParaUI(i)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener insumos: {ex.Message}");
                    Console.WriteLine($"Stack trace: {ex.StackTrace}");

                    if (ex.InnerException != null)
                    {
                        Console.WriteLine($"Inner exception: {ex.InnerException.Message}");
                    }

                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Insumo> BuscarPorNombre(string nombre)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<InsumoAD> insumosAD = contexto.Insumo
                        .Where(i => i.NombreInsumo.ToLower().Contains(nombre.ToLower()))
                        .OrderBy(i => i.NombreInsumo)
                        .ToList();
                    return insumosAD.Select(i => ConvertirObjetoParaUI(i)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar insumos por nombre: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Insumo> BuscarPorCategoria(int idCategoria)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<InsumoAD> insumosAD = contexto.Insumo
                        .Where(i => i.IdCategoria == idCategoria)
                        .OrderBy(i => i.NombreInsumo)
                        .ToList();
                    return insumosAD.Select(i => ConvertirObjetoParaUI(i)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al buscar insumos por categoría: {ex.Message}");
                    throw;
                }
            }
        }

        public Abstracciones.ModeloUI.Insumo ObtenerPorId(int id)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    var insumoAD = contexto.Insumo.FirstOrDefault(i => i.IdInsumo == id);
                    return insumoAD != null ? ConvertirObjetoParaUI(insumoAD) : null;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener insumo por ID: {ex.Message}");
                    throw;
                }
            }
        }

        public List<Abstracciones.ModeloUI.Insumo> ObtenerBajoStock()
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    List<InsumoAD> insumosAD = contexto.Insumo
                        .Where(i => i.CantidadDisponible <= i.StockMinimo)
                        .OrderBy(i => i.NombreInsumo)
                        .ToList();
                    return insumosAD.Select(i => ConvertirObjetoParaUI(i)).ToList();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error al obtener insumos bajo stock: {ex.Message}");
                    throw;
                }
            }
        }

        private Abstracciones.ModeloUI.Insumo ConvertirObjetoParaUI(InsumoAD insumoAD)
        {
            return new Abstracciones.ModeloUI.Insumo
            {
                IdInsumo = insumoAD.IdInsumo,
                NombreInsumo = insumoAD.NombreInsumo,
                UnidadMedida = insumoAD.UnidadMedida,
                IdCategoria = insumoAD.IdCategoria,
                CostoUnitario = insumoAD.CostoUnitario,
                CantidadDisponible = insumoAD.CantidadDisponible,
                StockMinimo = insumoAD.StockMinimo,
                Estado = insumoAD.Estado,
                FechaCreacion = insumoAD.FechaCreacion,
                FechaActualizacion = insumoAD.FechaActualizacion
            };
        }

        public void Crear(Abstracciones.ModeloUI.Insumo insumo)

        {
            using (var contexto = new Contexto())
            {
                var entidad = new InsumoAD
                {
                    NombreInsumo = insumo.NombreInsumo,
                    UnidadMedida = insumo.UnidadMedida,
                    IdCategoria = insumo.IdCategoria,
                    CostoUnitario = insumo.CostoUnitario,
                    CantidadDisponible = insumo.CantidadDisponible,
                    StockMinimo = insumo.StockMinimo,
                    Estado = true,
                    FechaCreacion = DateTime.UtcNow,
                    FechaActualizacion = DateTime.UtcNow

                };

                contexto.Insumo.Add(entidad);
                contexto.SaveChanges();
            }
        }
    }

}