using AromasWeb.Abstracciones.Logica.Bitacora;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Bitacoras
{
    public class ListarBitacoras : IListarBitacora
    {
        private readonly IListarBitacora _listarBitacoras;

        public ListarBitacoras()
        {
            _listarBitacoras = new AccesoADatos.Bitacoras.ListarBitacoras();
        }

        public List<Bitacora> Obtener()
        {
            return _listarBitacoras.Obtener();
        }

        public List<Bitacora> BuscarPorFiltros(
            string buscar,
            string filtroModulo,
            DateTime? fechaInicio,
            DateTime? fechaFin)
        {
            return _listarBitacoras.BuscarPorFiltros(buscar, filtroModulo, fechaInicio, fechaFin);
        }

        public Bitacora ObtenerPorId(int id)
        {
            if (id <= 0)
                return null;

            return _listarBitacoras.ObtenerPorId(id);
        }

        public List<Bitacora> ObtenerPorEmpleado(int idEmpleado)
        {
            return _listarBitacoras.ObtenerPorEmpleado(idEmpleado);
        }

        public List<Bitacora> ObtenerPorModulo(int idModulo)
        {
            return _listarBitacoras.ObtenerPorModulo(idModulo);
        }

        public List<Bitacora> ObtenerPorRangoDeFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return _listarBitacoras.ObtenerPorRangoDeFechas(fechaInicio, fechaFin);
        }
    }
}