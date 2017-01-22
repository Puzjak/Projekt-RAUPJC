using System.Threading.Tasks;

namespace RAUPJC_Projekt.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
