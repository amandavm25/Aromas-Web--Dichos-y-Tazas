using AromasWeb.Abstracciones.Servicios;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace AromasWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _apiKey;
        private readonly string _fromEmail;
        private readonly string _fromName;

        public EmailService(IConfiguration configuration)
        {
            _apiKey = configuration["EmailSettings:SendGridApiKey"];
            _fromEmail = configuration["EmailSettings:FromEmail"];
            _fromName = configuration["EmailSettings:FromName"];
        }

        public async Task<bool> EnviarCodigoRecuperacion(string destinatario, string nombreDestinatario, string codigo)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_fromEmail, _fromName);
                var to = new EmailAddress(destinatario, nombreDestinatario);
                var subject = "Código de Recuperación de Contraseña - Tsuru";

                // Contenido en texto plano
                var plainTextContent = $@"
Hola {nombreDestinatario},

Recibimos una solicitud para restablecer la contraseña de tu cuenta en Tsuru.

Tu código de verificación es:

    {codigo}

Este código expirará en 15 minutos.

Si no solicitaste este cambio, puedes ignorar este correo de manera segura.

Saludos,
El equipo de Tsuru
                ";

                // Contenido en HTML (más bonito)
                var htmlContent = $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <style>
        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
        .header {{ background: linear-gradient(135deg, #8F8E6A 0%, #5C5B3E 100%); color: white; padding: 30px; text-align: center; border-radius: 10px 10px 0 0; }}
        .content {{ background: #f9f9f9; padding: 30px; border-radius: 0 0 10px 10px; }}
        .code-box {{ background: white; border: 2px solid #8F8E6A; border-radius: 8px; padding: 20px; text-align: center; margin: 20px 0; }}
        .code {{ font-size: 32px; font-weight: bold; color: #5C5B3E; letter-spacing: 5px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
        .warning {{ background: #fff3cd; border-left: 4px solid #ffc107; padding: 15px; margin: 20px 0; border-radius: 4px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <h1 style='margin: 0;'>🔐 Recuperación de Contraseña</h1>
        </div>
        <div class='content'>
            <p>Hola <strong>{nombreDestinatario}</strong>,</p>
            <p>Recibimos una solicitud para restablecer la contraseña de tu cuenta en <strong>Tsuru</strong>.</p>
            
            <div class='code-box'>
                <p style='margin: 0 0 10px; color: #666;'>Tu código de verificación es:</p>
                <div class='code'>{codigo}</div>
            </div>

            <div class='warning'>
                <p style='margin: 0;'><strong>⏱️ Importante:</strong> Este código expirará en <strong>15 minutos</strong>.</p>
            </div>

            <p>Si no solicitaste este cambio, puedes ignorar este correo de manera segura. Tu cuenta permanece protegida.</p>

            <p style='margin-top: 30px;'>Saludos,<br><strong>El equipo de Tsuru</strong></p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
        </div>
    </div>
</body>
</html>
                ";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                // SendGrid devuelve StatusCode entre 200-299 si fue exitoso
                return response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                       response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar email: {ex.Message}");
                return false;
            }
        }
    }
}