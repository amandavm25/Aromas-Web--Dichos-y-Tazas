using AromasWeb.Abstracciones.Logica.Reserva;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Reservas
{
    public class ListarReservas : IListarReservas
    {
        private IListarReservas _listarReservas;

        public ListarReservas()
        {
            _listarReservas = new AccesoADatos.Reservas.ListarReservas();
        }

        public List<Reserva> Obtener()
        {
            return _listarReservas.Obtener();
        }

        public List<Reserva> BuscarPorNombre(string nombre)
        {
            return _listarReservas.BuscarPorNombre(nombre);
        }

        public List<Reserva> BuscarPorTelefono(string telefono)
        {
            return _listarReservas.BuscarPorTelefono(telefono);
        }

        public List<Reserva> BuscarPorEstado(string estado)
        {
            return _listarReservas.BuscarPorEstado(estado);
        }

        public List<Reserva> BuscarPorFecha(DateTime fecha)
        {
            return _listarReservas.BuscarPorFecha(fecha);
        }

        public List<Reserva> ObtenerPorCliente(int idCliente)
        {
            return _listarReservas.ObtenerPorCliente(idCliente);
        }

        public List<Reserva> ObtenerReservasHoy()
        {
            return _listarReservas.ObtenerReservasHoy();
        }

        public List<Reserva> ObtenerReservasFuturas()
        {
            return _listarReservas.ObtenerReservasFuturas();
        }

        public List<Reserva> ObtenerReservasPasadas()
        {
            return _listarReservas.ObtenerReservasPasadas();
        }

        public List<Reserva> ObtenerReservasProximas(int dias)
        {
            return _listarReservas.ObtenerReservasProximas(dias);
        }

        public Reserva ObtenerPorId(int id)
        {
            return _listarReservas.ObtenerPorId(id);
        }
    }
}