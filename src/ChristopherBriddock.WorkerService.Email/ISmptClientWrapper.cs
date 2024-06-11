using System.Net.Mail;

namespace ChristopherBriddock.WorkerService.Email;

public interface ISmtpClient : IDisposable
{
    Task SendMailAsync(MailMessage message);
}
