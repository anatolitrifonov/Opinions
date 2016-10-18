using System.Threading.Tasks;

namespace BestFor.Services.Messaging
{
    public interface IEmailSender
    {
        // Send email to someone
        void SendEmailAsync(string email, string subject, string message);

        // Send email to ourself
        void SendEmailAsync(string subject, string message);
    }

}
