using CustomerIncidentProject.Models;
using Dapper;
using IncidentProject.Models;
using IncidentProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace IncidentProject.Repository
{
    public class CustomerAccountRepo : ICustomerAccountRepo
    {
        private readonly IConfiguration _configuration;
        private readonly IApiImplementor _apiImplementor;
        public readonly connectionUrl _url;
        private readonly IMemoryCache _memoryCache;
        public CustomerAccountRepo(IConfiguration configuration, IApiImplementor apiImplementor, IOptions<connectionUrl> url, IMemoryCache memoryCache)
        {
            _configuration = configuration;
            _apiImplementor = apiImplementor;
            _url = url.Value;
            _memoryCache = memoryCache;
        }


        public async Task<CustomerAccountResponse> GetcustomerAccountDetails(CustomerAccountRequest model)
        {
            try
            {
                Log.Information("API Request {@response}", model);
                var response = await _apiImplementor.GetApiService(_url.IPBase, _url.GetAccountDetails, model);
                CustomerAccountResponse result = JsonConvert.DeserializeObject<CustomerAccountResponse>(response.responseContent);
                Log.Information("API Response {@response}", result);
                return result;
            }
            catch (Exception ex)
            {
                return new CustomerAccountResponse();
            }
        }

        public async Task<string> SaveCustomerRecord(CustomerModel model)
        {

            ApiResponseObject res = new ApiResponseObject();
            var response = "";
            try
            {
                var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                var parameters = new CustomerModel
                {
                    Category = model.Category,
                    AccountNo = model.AccountNo,
                    Branch = model.Branch,
                    ComType = model.ComType,
                    Title = model.Title,
                    Description = model.Description,
                    DateLogged = DateTime.Now.ToString() + " opened by " + model.LoggedInUser, 
                    DateAssignedForProcessing = DateTime.Now.ToString(),
                    DateClosed = DateTime.Now.ToString(), 
                    //Status = "Processing"
                    Status = "OPENED",
                    TicketNo = model.TicketNo,
                    AccountName = model.AccountName,
                    LoggedInUser = model.LoggedInUser,
                    DocPath = model.DocPath,
                };

                var query = "INSERT INTO [dbo].[a table]([Category],[AccountNo],[Branch],[ComType],[Title],[Description],[DateLogged],[DateAssignedForProcessing],[DateClosed],[Status],[TicketNo],[AccountName],[LoggedInUser],[DocPath]) VALUES (@Category,@AccountNo,@Branch,@ComType,@Title,@Description,@DateLogged,@DateAssignedForProcessing,@DateClosed,@Status,@TicketNo,@AccountName,@LoggedInUser,@DocPath)";
                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(query, parameters);

                    if (result == 1)
                        response = "Record saved succesfully";
                    else
                        response = "Insert Failed. Check your inputs and try again";
                }
                //res.responseContent = response;
            }
            catch (Exception)
            {

            }
            return response;
        }

        public async Task<object> FetchTicketsSummary()
        {
            List<object> response = new List<object>();
            try
            {
                var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                var query = "Select Status, count(*) as NumberOfTickets from yourtable group by Status";
                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();
                    var result = await connection.QueryAsync(query);
                    response = result.ToList();
                }
            }
            catch (Exception)
            {

                throw;
            }
            return response.ToList();
        }

        public async Task<object> GetTicketInformation(CustomerAccountRequest model)
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "select * from yourtable where AccountNo = @AccountNo";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query, model);
                return result;
            }
        }


        public async Task<PieResponse> GetPieChartSummary()
        {
            PieResponse response = new PieResponse();

            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;

            var query = "Select Category, COUNT(*) as count from yourtable group by Category";

            using (var connection = new SqlConnection(constring))
            {
                connection.Open();

                var result = await connection.QueryAsync<PieResponseData>(query);

                var data = result.ToList();

                response.Categories = data.Select(x => x.Category).ToArray();
                response.Values = data.Select(x => x.Count).ToArray();

                return response;
            }
        }

        public async Task<BarResponse> GetTicketByBranch()
        {
            BarResponse response = new BarResponse();

            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;

            var query = "Select Branch, COUNT(*) as count from yourtable group by Branch";

            using (var connection = new SqlConnection(constring))
            {
                connection.Open();

                var result = await connection.QueryAsync<BarResonseData>(query);

                var data = result.ToList();

                response.Branches = data.Select(x => x.Branch).ToArray();
                response.Values = data.Select(x => x.Count).ToArray();

                return response;
            }
        }

        public async Task<object> GetEveryDbRecords()
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "select * from yourtable";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query);
                return result;
            }
        }

        public async Task<object> GetTop10Records()
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "SELECT TOP (5) * FROM yourtable order by Id desc";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query);
                return result;
            }
        }

        public async Task<object> GetOpenTickets()
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "Select * from yourtable where Status = 'Open'";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query);
                return result;
            }
        }

        public async Task<SmsResponse> SendSMSToCustomer(SmsRequest model)
        {
            try
            {

                var response = await _apiImplementor.PostApiService(_url.IPBase, _url.SendIncidentSMStoCustomer, model);
                SmsResponse result = JsonConvert.DeserializeObject<SmsResponse>(response.responseContent);
                return result;
            }
            catch (Exception ex)
            {
                //ex.Message();
                return new SmsResponse();
            }
        }

        public async Task<object> EditCustomerPage(EditModel model)
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "SELECT * FROM yourtable Where Id = @Id";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query, model);
                return result;
            }
        }

        public async Task<string> UpdateCustomerRecord(CustomerModel model)
        {
            ApiResponseObject res = new ApiResponseObject();
            var response = "";
            try
            {
                var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                var parameters = new CustomerModel
                {
                    Status = model.Status,                   
                    Comments = model.Comments,
                    Id = model.Id,
                    LoggedInUser = model.LoggedInUser,
                    DateClosed = DateTime.Now.ToString() + " closed by " + model.LoggedInUser
                };

                var parameters2 = new CustomerModel
                {
                    Status = model.Status,
                    Comments = model.Comments,
                    Id = model.Id,
                    LoggedInUser = model.LoggedInUser,
                    DateAssignedForProcessing = DateTime.Now.ToString() + " processed by " + model.LoggedInUser

                };


                if (model.Status == "CLOSED")
                {
                    var query = "UPDATE [dbo].[yourtable] SET Status = @Status, Comments = @Comments, DateClosed = @DateClosed, LoggedInUser = @LoggedInUser Where Id = @Id";
                    using (var connection = new SqlConnection(constring))
                    {
                        connection.Open();
                        var result = await connection.ExecuteAsync(query, parameters);

                        if (result == 1)
                            response = "Record updated succesfully";

                    }
                }
                else
                {
                    var query = "UPDATE [dbo].[yourtable] SET Status = @Status, Comments = @Comments, DateAssignedForProcessing = @DateAssignedForProcessing, LoggedInUser = @LoggedInUser Where Id = @Id";
                    using (var connection = new SqlConnection(constring))
                    {
                        connection.Open();
                        var result = await connection.ExecuteAsync(query, parameters2);

                        if (result == 1)
                            response = "Record updated succesfully";

                    }
                }


            }
            catch (Exception)
            {

            }
            return response;
        }

        public async Task<string> SaveUserRecord(UserRequest model)
        {
            ApiResponseObject res = new ApiResponseObject();
            var response = "";
            try
            {
                var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
                var parameters = new UserRequest
                {
                    staffemail = model.staffemail,
                    password = model.password,
                    status = 1,//"1",
                    last_login_date = DateTime.Now.ToString(),
                    name = model.name,
                    branch = model.branch

                };

                var query = "INSERT INTO [dbo].[login]([staffemail],[password],[status],[last_login_date],[name],[branch]) VALUES (@staffemail,@password,@status,@last_login_date,@name, @branch)";
                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(query, model);

                    if (result == 1)
                        response = "user saved succesfully";
                    else
                        response = "Insert Failed. Check your inputs and try again";
                }
                //res.responseContent = response;
            }
            catch (Exception)
            {

            }
            return response;
        }



        public async Task<object> GetUserList()
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "Select * from login";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query);
                return result;
            }
        }

        public async Task<object> EditUserPage(EditModel model)
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "SELECT * FROM login Where Id = @Id";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query, model);
                return result;
            }
        }

        public async Task<string> UpdateUserRecord(UserRequest model)
        {
            ApiResponseObject res = new ApiResponseObject();
            var response = "";
            try
            {
                var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
              

                var query = "UPDATE [dbo].[login] SET staffemail = @staffemail, name = @name, status = @status Where staffemail = @staffemail";
                using (var connection = new SqlConnection(constring))
                {
                    connection.Open();
                    var result = await connection.ExecuteAsync(query, model);

                    if (result == 1)
                        response = "Record updated succesfully";

                }
                //res.responseContent = response;
            }
            catch (Exception)
            {

            }
            return response;
        }

        public async Task<object> FetchTicketsCount()
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "Select  count(*) as NumberOfCount from yourtable";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query);
                return result;
            }
        }

        public async Task<EmailResponse> SendEmailToCustomer(EmailRequest model)
        {
            try
            {

                var response = await _apiImplementor.PostEmailApiService(_url.IPBase, _url.SendIncidentEmailtoCustomer, model);
                EmailResponse result = JsonConvert.DeserializeObject<EmailResponse>(response.responseContent);
                return result;
            }
            catch (Exception ex)
            {
                //ex.Message();
                return new EmailResponse();
            }
        }

        public async Task<CommenTitleVm> GetTitle(string ticketNo)
        {
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            //var titlr = "";
            //var comments = "";
            CommenTitleVm dy = new CommenTitleVm();
            var query = $"Select Title, Comments from yourtable where TicketNo = @TicketNo";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();

                var result = await connection.QueryAsync(query, new { TicketNo = ticketNo });
                foreach (var value in result)
                {
                    dy = new CommenTitleVm
                    {
                         Title = value.Title,
                         Comments = value.Comments
                    };
                    //titlr = value.Title;
                    //comments = value.Comments;
                }
                return dy;
            }
        }

        public async Task<string> smsService(string acctNo, string phoneNo, string email)
        {
            string phone = phoneNo;
            phone = phone.Remove(0, 1);
            var radval = IncidentRandomValue.RandomString(8);
            var autoRanTicket = $"email-INF-{radval}";


            var response = new SmsRequest
            {

                AccountNo = acctNo,
                TicketNo = autoRanTicket,
                PhoneNumber = $"234{phone}",
                Content = ""
            };


            var emailResponse = new EmailRequest { Recepient = email, Title = "Notification", Body = "", Attachments = String.Empty };


            _memoryCache.Set("acctInfo", response);
            _memoryCache.Set("emailInfo", emailResponse);//, cacheValue, cacheEntryOptions);
            return response.ToString();
        }
    }
}
