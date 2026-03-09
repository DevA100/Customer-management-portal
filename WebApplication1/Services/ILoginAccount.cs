using IncidentProject.Models;
using System.Threading.Tasks;

namespace IncidentProject.Services
{
    public interface ILoginAccount
    {
        Task<object> LoginRegister(UserRequest model);
        Task<object> Login(UserRequest model);
        Task<object> PassReset(UserRequest model);
        Task<object> PasswordChange(UserRequest model);
        Task<EmailResponse> SendEmailToCustomer(EmailRequest model);
    }
}
