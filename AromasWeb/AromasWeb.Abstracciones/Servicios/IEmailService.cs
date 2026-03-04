using System.Threading.Tasks;

namespace AromasWeb.Abstracciones.Servicios
{
    public interface IEmailService
    {
        Task<bool> EnviarCodigoRecuperacion(string destinatario, string nombreDestinatario, string codigo);
    }
}