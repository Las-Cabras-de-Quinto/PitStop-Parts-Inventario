using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Threading.Tasks;

namespace PitStop_Parts_Inventario.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor,
                           ILogger<EmailSender> logger)
        {
            Options = optionsAccessor.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Configuración para SendGrid, debe incluir la API key
        public AuthMessageSenderOptions Options { get; }

        // Método requerido por IEmailSender para enviar emails
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(Options.SendGridKey))
            {
                throw new InvalidOperationException("SendGrid API key is not configured.");
            }

            await Execute(Options.SendGridKey, subject, message, toEmail);
        }

        // Método interno que crea y envía el email usando SendGrid SDK
        public async Task Execute(string apiKey, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress("Joe@contoso.com", "Password Recovery"); // Cambia a tu email y nombre
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

            // Deshabilitar seguimiento de clics para privacidad y evitar problemas con enlaces de confirmación
            msg.SetClickTracking(false, false);

            var response = await client.SendEmailAsync(msg);

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation($"Email to {toEmail} queued successfully.");
            }
            else
            {
                var errorBody = await response.Body.ReadAsStringAsync();
                _logger.LogError($"Failed to send email to {toEmail}. StatusCode: {response.StatusCode}, Response: {errorBody}");
            }
        }
    }
}
