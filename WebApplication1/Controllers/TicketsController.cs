using IncidentProject.Models;
using IncidentProject.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace IncidentProject.Controllers
{
    [SessionControl]
    [Route("[controller]/[action]")]
    public class TicketsController : Controller
    {
        private readonly ICustomerAccountRepo _customerAccountRepo;
        private readonly IncidentDbContext _context;
        public TicketsController(ICustomerAccountRepo customerAccountRepo, IncidentDbContext context)
        {
            _customerAccountRepo = customerAccountRepo;
            _context = context;
        }
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> OpenTickets()
        {
            var status = "OPENED";
            var result =  await _context.IncidentForm.ToListAsync();
            return View(result.Where(e => e.Status == status).ToList());  

        }

        public async Task<IActionResult> ClosedTickets()
        {
            var status = "CLOSED";
            var result = await _context.IncidentForm.ToListAsync();
            return View(result.Where(e => e.Status == status));

        }

        public async Task<IActionResult> InProgress()
        {
            var status = "PROCESSING";
            var result = await _context.IncidentForm.ToListAsync();
            return View(result.Where(e => e.Status == status));

        }

        public async Task<IActionResult> TicketsOnhold()
        {
            var status = "ON-HOLD";
            var result = await _context.IncidentForm.ToListAsync();
            return View(result.Where(e => e.Status == status));

        }

        public async Task<IActionResult> TicketList()
        {
            //var status = "Processing";
            var result = await _context.IncidentForm.ToListAsync();
            return View(result);

        }

        [HttpPost]
        public async Task<object> EditPage([FromBody] EditModel model)
        {
            var result = await _customerAccountRepo.EditCustomerPage(model);
            return result;
        }
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var customer = await _context.IncidentForm.FindAsync(id);
            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Category,AccountNo,Branch,ComType,Title,Description,Date,Status")] CustomerModel customer)
        {
            

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(customer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    
                }
                return RedirectToAction(nameof(OpenTickets));
            }
            return View(customer);
        }
    }
}
