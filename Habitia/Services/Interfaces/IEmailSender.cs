namespace Habitia.Services.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string destinatario, string asunto, string cuerpoHtml);
    }
}