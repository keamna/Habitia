using Habitia.Services.Interfaces;
using System.Net.Http.Json;

namespace Habitia.Services
{
    public class BrevoEmailSender : IEmailSender
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        private readonly ILogger<BrevoEmailSender> _logger;

        public BrevoEmailSender(string apiKey, ILogger<BrevoEmailSender> logger)
        {
            _apiKey = apiKey;
            _httpClient = new HttpClient();
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var url = "https://api.brevo.com/v3/smtp/email";

            var payload = new
            {
                sender = new { email = "keanymenaberrocal13@gmail.com", name = "Habitia" },
                to = new[] { new { email = email } },
                subject = subject,
                htmlContent = htmlMessage
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = JsonContent.Create(payload)
            };

            request.Headers.Add("api-key", _apiKey);
            request.Headers.Add("Accept", "application/json");

            try
            {
                var response = await _httpClient.SendAsync(request);
                var responseContent = await response.Content.ReadAsStringAsync();

                _logger.LogInformation($"[BREVO] Status: {response.StatusCode}");

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError($"❌ [BREVO ERROR] {response.StatusCode}: {responseContent}");
                    throw new InvalidOperationException(
                        $"Error Brevo ({response.StatusCode}): {responseContent}");
                }

                _logger.LogInformation($"✅ [BREVO] Email enviado a {email}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"❌ [BREVO EXCEPTION] {ex.Message}");
                throw new InvalidOperationException($"Error al enviar email: {ex.Message}", ex);
            }
        }
    }
}