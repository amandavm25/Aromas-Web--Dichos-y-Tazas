using AromasWeb.Abstracciones.Logica.Bitacora;
using AromasWeb.AccesoADatos.Modelos;
using System;

namespace AromasWeb.AccesoADatos.Bitacoras
{
    public class CrearBitacora : ICrearBitacora
    {
        public int Crear(Abstracciones.ModeloUI.Bitacora bitacora)
        {
            using (var contexto = new Contexto())
            {
                try
                {
                    // Convertir fecha a UTC si es necesario
                    DateTime fechaUtc = bitacora.Fecha == default ? DateTime.UtcNow : bitacora.Fecha;
                    if (fechaUtc.Kind == DateTimeKind.Unspecified)
                        fechaUtc = DateTime.SpecifyKind(fechaUtc, DateTimeKind.Utc);
                    else if (fechaUtc.Kind == DateTimeKind.Local)
                        fechaUtc = fechaUtc.ToUniversalTime();

                    var bitacoraAD = new BitacoraAD
                    {
                        IdEmpleado = bitacora.IdEmpleado,
                        IdModulo = bitacora.IdModulo,
                        Accion = bitacora.Accion?.Trim(),
                        TablaAfectada = bitacora.TablaAfectada?.Trim(),
                        Descripcion = bitacora.Descripcion?.Trim(),
                        DatosAnteriores = bitacora.DatosAnteriores,
                        DatosNuevos = bitacora.DatosNuevos,
                        Fecha = fechaUtc
                    };

                    contexto.Bitacora.Add(bitacoraAD);
                    return contexto.SaveChanges();
                }
                catch (Exception ex)
                {
                    // La bitácora nunca debe interrumpir el flujo principal
                    System.Diagnostics.Debug.WriteLine($"Error al guardar bitácora: {ex.Message}");
                    return 0;
                }
            }
        }
    }
}