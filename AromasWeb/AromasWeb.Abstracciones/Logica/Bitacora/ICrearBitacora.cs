using BitacoraUI = AromasWeb.Abstracciones.ModeloUI.Bitacora;

namespace AromasWeb.Abstracciones.Logica.Bitacora
{
    public interface ICrearBitacora
    {
        int Crear(BitacoraUI bitacora);
    }
}