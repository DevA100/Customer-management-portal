using CustomerIncidentProject.Models;
using Dapper;
using IncidentProject.Models;
using IncidentProject.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace IncidentProject.Repository
{
    public class LoginAccountRepo : ILoginAccount
    {
        private readonly IConfiguration _configuration;
        private readonly IApiImplementor _apiImplementor;
        public readonly connectionUrl _url;
        public LoginAccountRepo(IConfiguration configuration, IApiImplementor apiImplementor, IOptions<connectionUrl> url)
        {
            _configuration = configuration;
            _apiImplementor = apiImplementor;
            _url = url.Value;
        }
        public async Task<object> LoginRegister(UserRequest model)
        {
            var parameters = new UserRequest { staffemail = model.staffemail, password = model.password };
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "select * from yourtable where staffemail = @staffemail and password = @password";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query, parameters);
                return result;
            }
        }

        
        public async Task<object> Login(UserRequest model)
        {
            var parameters = new UserRequest { staffemail = model.staffemail, password = model.password};
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "select * from login where staffemail = @staffemail and password = @password";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.QueryAsync(query, parameters);  
                return result;
            }
        }

        public async Task<object> PassReset(UserRequest model)
        {
            Log.Information("NewPassword {@request}", model);
            var parameters = new UserRequest { password = "password", staffemail = model.staffemail, status = 1 };
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "UPDATE login SET password = @password,status = @status WHERE staffemail = @staffemail";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(query, parameters);
                return result;
            }
            return null;
        }

        public async Task<object> PasswordChange(UserRequest model)
        {
            Log.Information("NewPassword {@request}", model);
            var parameters = new UserRequest { password = model.password, staffemail = model.staffemail, status = 1 };
            var constring = _configuration.GetSection("ConnectionStrings").GetSection("DefaultConnection").Value;
            var query = "UPDATE login SET password = @password,status = @status WHERE staffemail = @staffemail";
            using (var connection = new SqlConnection(constring))
            {
                connection.Open();
                var result = await connection.ExecuteAsync(query, parameters);
                return result;
            }
            return null;
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
    }
}
