using Habitia.Services.Interfaces;
using System.Net;
using System.Net.Mail;

namespace Habitia.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly string _host;
        private readonly int _port;
        private readonly bool _enableSSL;
        private readonly string _userName;
        private readonly string _password;

        public EmailSender(string host, int port, bool enableSSL, string userName, string password)
        {
            _host = host;
            _port = port;
            _enableSSL = enableSSL;
            _userName = userName;
            _password = password;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            try
            {
                using (var client = new SmtpClient(_host, _port))
                {
                    client.EnableSsl = _enableSSL;
                    client.Credentials = new NetworkCredential(_userName, _password);

                    var message = new MailMessage
                    {
                        From = new MailAddress(_userName),
                        Subject = subject,
                        Body = htmlMessage,
                        IsBodyHtml = true
                    };

                    message.To.Add(email);

                    await client.SendMailAsync(message);

                    Console.WriteLine($"✅ [EMAIL] Enviado a {email} - Asunto: {subject}");
                }
            }
            catch (SmtpException ex)
            {
                Console.WriteLine($"❌ [SMTP ERROR] {ex.Message} - Código: {ex.StatusCode}");
                throw new InvalidOperationException($"Error al enviar email SMTP: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ [EMAIL ERROR] {ex.Message}");
                throw new InvalidOperationException($"Error al enviar email: {ex.Message}", ex);
            }
        }
    }
}