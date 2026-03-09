using CustomerIncidentProject.Models;
using IncidentProject.Models;
using System.Threading.Tasks;
using static IncidentProject.Repository.CustomerAccountRepo;

namespace IncidentProject.Repository
{
    public interface ICustomerAccountRepo
    {        
        Task<CustomerAccountResponse> GetcustomerAccountDetails(CustomerAccountRequest model);
        Task<SmsResponse> SendSMSToCustomer(SmsRequest model);   
        //Task<EmailResponse> SendEmailToCustomer(string Recipient, string Title, string Body, string Attachments);   
        Task<EmailResponse> SendEmailToCustomer(EmailRequest model);   
        Task<string> SaveCustomerRecord(CustomerModel model);
        Task<string> UpdateCustomerRecord(CustomerModel model);
        Task<object> FetchTicketsSummary();
        Task<object> FetchTicketsCount();
        Task<object> GetEveryDbRecords();
        Task<object> GetTop10Records();
        Task<object> GetOpenTickets();
        Task<PieResponse> GetPieChartSummary();
        Task<object> GetTicketInformation(CustomerAccountRequest model);
        Task<BarResponse> GetTicketByBranch();
        Task<object> EditCustomerPage(EditModel model);
        Task<object> EditUserPage(EditModel model);
        Task<CommenTitleVm> GetTitle(string ticketNo);
        Task<string> SaveUserRecord(UserRequest model);
        Task<object> GetUserList();
        Task<string> UpdateUserRecord(UserRequest model);
        Task<string> smsService(string acctNo, string phoneNo, string content );

    }
}
