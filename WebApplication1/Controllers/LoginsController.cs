using IncidentProject.Models;
using IncidentProject.Repository;
using IncidentProject.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;
using System.Threading.Tasks;

namespace IncidentProject.Controllers
{
    public class LoginsController : Controller
    {
        private readonly ILoginAccount _loginAccount;
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IncidentDbContext _context;
        private readonly IMemoryCache _memoryCache;
        
        public LoginsController(ILoginAccount loginAccount, 
            ICustomerAccountRepo customerAccountRepo, IncidentDbContext context, IMemoryCache memoryCache)
        {
            _loginAccount = loginAccount;
            _customerAccountRepo = customerAccountRepo;
            _context = context;
            _memoryCache = memoryCache;
          
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<object> Login([FromBody] UserRequest model)
        {
            string userDetailsObject = (model.staffemail);//JsonConvert.SerializeObject(model.staffemail);
            _memoryCache.Set("loggedinUser", userDetailsObject);
            HttpContext.Session.SetString("Userdetails", userDetailsObject);
            var result = await _loginAccount.Login(model);              
            TempData["welcome"] = model.staffemail;
            return result;
            //return RedirectToAction("Index","Dashboard",result);
        }

        [NonAdminOperationalControl]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<string> SaveUserInformation([FromBody] UserRequest model)
        {
            var result = await _customerAccountRepo.SaveUserRecord(model);
            return result;
        }

        public async Task<object> UserList()
        {
            //var status = "Processing";
            var result = await _customerAccountRepo.GetUserList();
            return result;

        }

        [HttpPost]
        public async Task<object> EditUser([FromBody] EditModel model)
        {
            //var status = "Processing";
            var result = await _customerAccountRepo.EditUserPage(model);
            return result;

        }

        [HttpPost]
        public async Task<string> UpdateUserInformation([FromBody] UserRequest model)
        {
            var result = await _customerAccountRepo.UpdateUserRecord(model);
            return result;
        }


        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.login.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpPost]
        public async Task<object> PasswordReset([FromBody] UserRequest model)
        {
            model.staffemail.ToLower();
            Log.Information("PasswordReset {@request}", model);
            var result = await _loginAccount.PassReset(model);
            Log.Information("UserResponse {@result}", result);

            var emailResponse = new EmailRequest
            {
                Recepient = model.staffemail,
                Title = "Password Reset Notification",
                Body = $"Your Password has been reset to default succesffully" + "<br>" + "------------------------------------------------" + "<br>" +
                "Use this: password to login and change afterwards " + "<br>" + "------------------------------------------------" + "<br>" +
                    "App link '<a href='http://192.168.224.35/'>Go to the site</a>'"
            };

            EmailResponse sendemail = await _loginAccount.SendEmailToCustomer(emailResponse);

            return result;
            //return RedirectToAction("Index","Dashboard", result);
        }


        [HttpPost]
        public async Task<object> NewPassword([FromBody] UserRequest model)
        {
            var result = await _loginAccount.PasswordChange(model);
            Log.Information("NewPassword {@result}", result);
            return result;
            //return RedirectToAction("Index","Dashboard", result);
        }


    }
}
