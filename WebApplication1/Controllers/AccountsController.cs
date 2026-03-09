using CustomerIncidentProject.Models;
using IncidentProject.Models;
using IncidentProject.Repository;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;
using Serilog;
using System;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace IncidentProject.Controllers
{
    [SessionControl]
    [Route("[controller]/[action]")]
    public class AccountsController : Controller
    {
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IncidentDbContext _context;
        public SmsRequest response;
        private readonly IMemoryCache _memoryCache;
        public AccountsController(ICustomerAccountRepo customerAccountRepo, IncidentDbContext context, IMemoryCache memoryCache)
        {
            _customerAccountRepo = customerAccountRepo;
            _context = context;
            _memoryCache = memoryCache;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public async Task<CustomerAccountResponse> GetAcctDetails([FromBody] CustomerAccountRequest model)
        {

            var result = await _customerAccountRepo.GetcustomerAccountDetails(model);

            if(result == null) { } else { 
            var sendSMS = await _customerAccountRepo.smsService(result.accountNumber, result.mainPhone, result.email);
            }

           
            return result;
        }


        [HttpPost]
        public async Task<IActionResult> SaveCustomerInformation(CustomerModel model, IFormFile FormFile)
        {

            var user = HttpContext.Session.GetString("Userdetails");
            

            var filePath = "";
            var dbPath = "";
            var filenames = "";
            if (FormFile != null)
            {
                filenames = FormFile.FileName.Replace(" ", "-");
                var filename = ContentDispositionHeaderValue.Parse(FormFile.ContentDisposition).FileName.Replace(" ", "-");


                var MainPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Uploads");

                if (!Directory.Exists(MainPath))
                {
                    Directory.CreateDirectory(MainPath);
                }

                
                filePath = Path.Combine(MainPath, filenames);
                dbPath = Path.Combine("Uploads", filenames);
                using (System.IO.Stream stream = new FileStream(filePath, FileMode.Create))
                {
                    await FormFile.CopyToAsync(stream);
                }

                string extension = Path.GetExtension(filename);
            }


            SmsRequest smsModel = (SmsRequest)_memoryCache.Get("acctInfo");

            smsModel.Content = $"Dear Customer, ticket ({model.Title})  with the tracker no {smsModel.TicketNo} has been OPENED. we are attending to your request.";
            EmailRequest emailRequest = (EmailRequest)_memoryCache.Get("emailInfo");

            emailRequest.Body = $"Dear Customer, ticket ({model.Title})  with the tracker no {smsModel.TicketNo} has been OPENED. we are attending to your request.";

            var vmmodel = new CustomerModel { TicketNo = smsModel.TicketNo, LoggedInUser = user, Status = model.Status, AccountNo = model.AccountNo, AccountName = model.AccountName, Title = model.Title,
              Branch = model.Branch, Category = model.Category, ComType = model.ComType, DateLogged = model.DateLogged, Description = model.Description, Id = model.Id, DocPath = dbPath};
            if (ModelState.IsValid)
            {
                var result = await _customerAccountRepo.SaveCustomerRecord(vmmodel);

                if(!String.IsNullOrEmpty(result))
                {
                    var res = await _customerAccountRepo.SendSMSToCustomer(smsModel);
                    var emailres = await _customerAccountRepo.SendEmailToCustomer(emailRequest);
                }
                else
                {
                    return View("Index"); 
                }

                ViewBag.message = "Record saved successfully";
                return View("Index", result);
            }
            return View("Index");

        }

        [HttpPost]
        public async Task<string> UpdateCustomerInformation([FromBody] CustomerModel model)
        {
            var user = HttpContext.Session.GetString("Userdetails");
            model.LoggedInUser = user;
            var result = await _customerAccountRepo.UpdateCustomerRecord(model);
            Log.Information("Update customer information {@response}", result);

            var acctNo = new CustomerAccountRequest { accountNo = model.AccountNo };

            var acctRec = await _customerAccountRepo.GetcustomerAccountDetails(acctNo);
            Log.Information("AcctRec {@response}", acctRec);

            CommenTitleVm getTicket = await _customerAccountRepo.GetTitle(model.TicketNo);
           
            string phoneNo = acctRec.mainPhone;
            phoneNo = phoneNo.Remove(0, 1);
            response = new SmsRequest
            {

                AccountNo = acctRec.accountNumber,

                PhoneNumber = $"234{phoneNo}",
                Content = $"Dear Customer, the status of your ticket ({getTicket.Title}) with incident no {model.TicketNo} is {model.Status}."
            };

            var emailResponse = new EmailRequest { Recepient = acctRec.email, Title = "Notification", Body = $"Dear Customer, the status of your ticket ({getTicket.Title}) with incident no {model.TicketNo} is {model.Status}. <br> UPDATE: ({getTicket.Comments})", Attachments ="" };

            var res = await _customerAccountRepo.SendSMSToCustomer(response);
            Log.Information("SendSMSToCustomer - {@response}", res);
            var sendemail = _customerAccountRepo.SendEmailToCustomer(emailResponse);
            return result;


        }

        public IActionResult Login()
        {
            return View();
        }


    }
}
