using System.Threading.Tasks;

namespace RAUPJC_Projekt.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);
    }
}
