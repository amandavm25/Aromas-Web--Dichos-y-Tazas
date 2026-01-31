using ReservaUI = AromasWeb.Abstracciones.ModeloUI.Reserva;
using System;
using System.Collections.Generic;

namespace AromasWeb.Abstracciones.Logica.Reserva
{
    public interface IListarReservas
    {
        List<ReservaUI> Obtener();
        List<ReservaUI> BuscarPorNombre(string nombre);
        List<ReservaUI> BuscarPorTelefono(string telefono);
        List<ReservaUI> BuscarPorEstado(string estado);
        List<ReservaUI> BuscarPorFecha(DateTime fecha);
        List<ReservaUI> ObtenerPorCliente(int idCliente);
        List<ReservaUI> ObtenerReservasHoy();
        List<ReservaUI> ObtenerReservasFuturas();
        List<ReservaUI> ObtenerReservasPasadas();
        List<ReservaUI> ObtenerReservasProximas(int dias);
        ReservaUI ObtenerPorId(int id);
    }
}