using System.Threading.Tasks;

namespace BestFor.Services.Messaging
{
    public interface ISmsSender
    {
        void SendSmsAsync(string number, string message);
    }
}
