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

        // Email para RECUPERACIÓN de contraseña
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

                // Contenido en HTML
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

                return response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                       response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar email de recuperación: {ex.Message}");
                return false;
            }
        }

        // Email para VERIFICACIÓN de cuenta nueva
        public async Task<bool> EnviarCodigoVerificacion(string destinatario, string nombreDestinatario, string codigo)
        {
            try
            {
                var client = new SendGridClient(_apiKey);
                var from = new EmailAddress(_fromEmail, _fromName);
                var to = new EmailAddress(destinatario, nombreDestinatario);
                var subject = "¡Bienvenido a Tsuru! - Verifica tu Cuenta";

                // Contenido en texto plano
                var plainTextContent = $@"
¡Bienvenido a Tsuru, {nombreDestinatario}!

Estamos muy contentos de que te unas a nuestra comunidad.

Para completar tu registro y activar tu cuenta, por favor verifica tu correo electrónico ingresando el siguiente código:

    {codigo}

Este código expirará en 15 minutos.

Una vez verificado, podrás:
✓ Hacer reservaciones en segundos
✓ Acceder a promociones exclusivas
✓ Ver tu historial de visitas
✓ Recibir notificaciones importantes

Si no creaste esta cuenta, puedes ignorar este correo de manera segura.

¡Esperamos verte pronto!

Saludos,
El equipo de Tsuru
                ";

                // Contenido en HTML
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
        .code-box {{ background: white; border: 2px solid #B8A76A; border-radius: 8px; padding: 25px; text-align: center; margin: 25px 0; box-shadow: 0 4px 6px rgba(0,0,0,0.1); }}
        .code {{ font-size: 36px; font-weight: bold; color: #B8A76A; letter-spacing: 8px; }}
        .benefits {{ background: #fff; border-left: 4px solid #B8A76A; padding: 15px; margin: 20px 0; border-radius: 4px; }}
        .benefit-item {{ padding: 8px 0; display: flex; align-items: center; }}
        .check-icon {{ color: #27ae60; margin-right: 10px; font-size: 18px; }}
        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #666; }}
        .welcome-icon {{ font-size: 48px; margin-bottom: 10px; }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='welcome-icon'>🎉</div>
            <h1 style='margin: 0;'>¡Bienvenido a Tsuru!</h1>
            <p style='margin: 10px 0 0; opacity: 0.9;'>Estamos encantados de tenerte con nosotros</p>
        </div>
        <div class='content'>
            <p>Hola <strong>{nombreDestinatario}</strong>,</p>
            <p>Gracias por crear tu cuenta en <strong>Tsuru</strong>. Estamos muy contentos de que te unas a nuestra comunidad.</p>
            
            <p style='margin: 25px 0 15px; font-size: 16px;'><strong>Para activar tu cuenta, verifica tu correo con este código:</strong></p>
            
            <div class='code-box'>
                <p style='margin: 0 0 10px; color: #666; font-size: 14px;'>Tu código de verificación es:</p>
                <div class='code'>{codigo}</div>
                <p style='margin: 15px 0 0; color: #999; font-size: 13px;'>⏱️ Expira en 15 minutos</p>
            </div>

            <div class='benefits'>
                <p style='margin: 0 0 15px; font-weight: bold; color: #5C5B3E;'>Una vez verificada tu cuenta, podrás:</p>
                <div class='benefit-item'>
                    <span class='check-icon'>✓</span>
                    <span>Hacer reservaciones en segundos sin llenar formularios</span>
                </div>
                <div class='benefit-item'>
                    <span class='check-icon'>✓</span>
                    <span>Acceder a promociones y descuentos exclusivos</span>
                </div>
                <div class='benefit-item'>
                    <span class='check-icon'>✓</span>
                    <span>Ver tu historial completo de visitas</span>
                </div>
                <div class='benefit-item'>
                    <span class='check-icon'>✓</span>
                    <span>Recibir notificaciones importantes sobre tus reservas</span>
                </div>
            </div>

            <p style='margin-top: 25px; font-size: 14px; color: #666;'>Si no creaste esta cuenta, puedes ignorar este correo de manera segura.</p>

            <p style='margin-top: 30px;'><strong>¡Esperamos verte pronto!</strong></p>
            <p>Saludos,<br><strong>El equipo de Tsuru</strong></p>
        </div>
        <div class='footer'>
            <p>Este es un correo automático, por favor no respondas a este mensaje.</p>
            <p style='margin-top: 10px; color: #999;'>© 2024 Tsuru. Todos los derechos reservados.</p>
        </div>
    </div>
</body>
</html>
                ";

                var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
                var response = await client.SendEmailAsync(msg);

                return response.StatusCode == System.Net.HttpStatusCode.Accepted ||
                       response.StatusCode == System.Net.HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al enviar email de verificación: {ex.Message}");
                return false;
            }
        }
    }
}