using System.Threading.Tasks;
using Web.ViewModels;

namespace Web.Services.EmailService
{
    public interface IEmailService
    {
        void SendMail(EmailVm emailVm);
    }
}
