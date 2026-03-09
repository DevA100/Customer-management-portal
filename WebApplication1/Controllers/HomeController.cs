using CustomerIncidentProject.Models;
using IncidentProject.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;


namespace IncidentProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ICustomerAccountRepo _customerAccountRepo;

        public HomeController(ILogger<HomeController> logger, ICustomerAccountRepo customerAccountRepo)
        {
            _logger = logger;
            _customerAccountRepo = customerAccountRepo;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }



        [HttpGet]
        public async Task<IActionResult> GetAcctDetails(string  acctNo)
        {
            //var acctNos = acctNo;
            var value = new CustomerAccountRequest
            {
                 accountNo = acctNo
            };
            var result = await _customerAccountRepo.GetcustomerAccountDetails(value);
            return View("Index", result);
        }
    }
}
