using AromasWeb.Abstracciones.Logica.Asistencia;
using AromasWeb.Abstracciones.ModeloUI;
using System;
using System.Collections.Generic;

namespace AromasWeb.LogicaDeNegocio.Asistencias
{
    public class ListarAsistencias : IListarAsistencias
    {
        private IListarAsistencias _listarAsistencias;

        public ListarAsistencias()
        {
            _listarAsistencias = new AccesoADatos.Asistencias.ListarAsistencias();
        }

        public List<Asistencia> Obtener()
        {
            return _listarAsistencias.Obtener();
        }

        public List<Asistencia> BuscarPorEmpleado(int idEmpleado)
        {
            return _listarAsistencias.BuscarPorEmpleado(idEmpleado);
        }

        public List<Asistencia> BuscarPorFecha(DateTime fecha)
        {
            return _listarAsistencias.BuscarPorFecha(fecha);
        }

        public List<Asistencia> BuscarPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            return _listarAsistencias.BuscarPorRangoFechas(fechaInicio, fechaFin);
        }

        public Asistencia ObtenerPorId(int id)
        {
            return _listarAsistencias.ObtenerPorId(id);
        }

        public bool PuedeRegistrarEntrada(int idEmpleado, DateTime fecha)
        {
            return !_listarAsistencias.ExisteEntradaAbierta(idEmpleado, fecha);
        }

        public void RegistrarEntrada(Asistencia asistencia)
        {
            _listarAsistencias.CrearEntrada(asistencia);
        }

        public Asistencia ObtenerEntradaAbierta(int idEmpleado)
        {
            return _listarAsistencias.ObtenerEntradaAbierta(idEmpleado);
        }

        public void RegistrarSalida(int idAsistencia, TimeSpan horaSalida)
        {
            _listarAsistencias.RegistrarSalida(idAsistencia, horaSalida);
        }

        public bool ExisteEntradaAbierta(int idEmpleado, DateTime fecha)
        {
            return _listarAsistencias.ExisteEntradaAbierta(idEmpleado, fecha);
        }

        public void CrearEntrada(Asistencia asistencia)
        {
            _listarAsistencias.CrearEntrada(asistencia);
        }

        public List<Asistencia> ObtenerHistorialPorEmpleado(int idEmpleado)
        {
            return _listarAsistencias.ObtenerHistorialPorEmpleado(idEmpleado);
        }
    }
}