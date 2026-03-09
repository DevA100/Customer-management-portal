using IncidentProject.Repository;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.Configuration;
using CustomerIncidentProject.Models;
using IncidentProject.Models;

namespace IncidentProject.Controllers
{
    [SessionControl]
    [Route("[controller]/[action]")]
    public class DashboardController : Controller
    {
        private readonly ICustomerAccountRepo _customerAccountRepo;
        //private readonly IConfiguration _configuration;
        public DashboardController(ICustomerAccountRepo customerAccountRepo)
        {
            _customerAccountRepo = customerAccountRepo;
        }


        public async Task<IActionResult> Index()
        {
            
            return View();
        }

        [HttpGet]
        public async Task<object> GetTicketsSummary()
        {
            var response = await _customerAccountRepo.FetchTicketsSummary();
            return response;
        }

        [HttpGet]
        public async Task<object> GetTicketsTotalCount()
        {
            var response = await _customerAccountRepo.FetchTicketsCount();
            return response;
        }

        [HttpGet]
        public async Task<object> GetPieChartSummary()
        {
            var response = await _customerAccountRepo.GetPieChartSummary();
            return response;
        }

        [HttpGet]
        public async Task<object> GetTicketsbyBranchSummary()
        {
            var response = await _customerAccountRepo.GetTicketByBranch();
            return response;
        }

        [HttpGet]
        public async Task<object> GetTop10TicketRecords()
        {
            var response = await _customerAccountRepo.GetTop10Records();
            return response;
        }


        [HttpPost]
        public async Task<object> GetTicketDetails([FromBody] CustomerAccountRequest model)
        {
            var response = await _customerAccountRepo.GetTicketInformation(model);
            return response;
        }
    }
}
