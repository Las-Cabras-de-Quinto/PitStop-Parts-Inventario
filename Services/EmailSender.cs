using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Net;

namespace PitStop_Parts_Inventario.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly AuthMessageSenderOptions _options;

        public EmailSender(IOptions<AuthMessageSenderOptions> optionsAccessor, ILogger<EmailSender> logger)
        {
            _options = optionsAccessor?.Value ?? throw new ArgumentNullException(nameof(optionsAccessor));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            if (string.IsNullOrWhiteSpace(_options.ApiKey))
            {
                throw new InvalidOperationException("SendGrid API key is not configured.");
            }
            if (string.IsNullOrWhiteSpace(_options.FromAddress))
            {
                throw new InvalidOperationException("From email address is not configured.");
            }
            if (string.IsNullOrWhiteSpace(_options.FromName))
            {
                throw new InvalidOperationException("From email name is not configured.");
            }

            await Execute(_options.ApiKey, _options.FromAddress, _options.FromName, subject, message, toEmail);
        }

        private async Task Execute(string apiKey, string fromAddress, string fromName, string subject, string message, string toEmail)
        {
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromAddress, fromName);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);
            msg.SetClickTracking(false, false);

            try
            {
                var response = await client.SendEmailAsync(msg);
                var responseBody = await response.Body.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"Email to {toEmail} queued successfully.");
                }
                else
                {
                    HandleErrorResponse(response.StatusCode, responseBody, toEmail);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Network error occurred while sending email.");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                throw;
            }
        }

        private void HandleErrorResponse(HttpStatusCode statusCode, string responseBody, string toEmail)
        {
            switch (statusCode)
            {
                case HttpStatusCode.Unauthorized:
                    if (responseBody.Contains("Maximum credits exceeded", StringComparison.OrdinalIgnoreCase))
                    {
                        _logger.LogError($"SendGrid error for {toEmail}: Maximum credits exceeded. Consider upgrading your plan.");
                    }
                    else
                    {
                        _logger.LogError($"Unauthorized access for {toEmail}: {responseBody}");
                    }
                    break;

                case HttpStatusCode.Forbidden:
                    _logger.LogError($"Forbidden access for {toEmail}: {responseBody}");
                    break;

                case HttpStatusCode.TooManyRequests:
                    _logger.LogWarning($"Rate limit exceeded for {toEmail}: {responseBody}. Retrying...");
                    // Implement retry logic here
                    break;

                case HttpStatusCode.InternalServerError:
                    _logger.LogError($"Server error occurred while sending email to {toEmail}: {responseBody}");
                    break;

                default:
                    _logger.LogError($"Failed to send email to {toEmail}. StatusCode: {statusCode}, Response: {responseBody}");
                    break;
            }
        }
    }
}
