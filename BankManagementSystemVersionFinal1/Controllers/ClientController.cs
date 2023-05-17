using BankManagementSystemVersionFinal1.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BankManagementSystemVersionFinal1.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                email = TempData["Email"] as string; // Retrieve email from TempData
                TempData.Keep("Email"); // Retain the TempData value for subsequent requests
            }

            if (string.IsNullOrEmpty(email))
            {
                return NotFound();
            }

            var customer = _context.Customers.FirstOrDefault(c => c.Mail == email);

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }


        public async Task<IActionResult> Details(int? id)
        {
            

            var customer = _context.Customers.Include(c => c.Accounts).Include(c => c.BankBranch).FirstOrDefault(c => c.CustomerId == id);
            var Accounts = await _context.Accounts
              .Where(t => t.AccountHolder.CustomerId == id)
              .ToListAsync();
            ViewBag.account = Accounts;

            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }
    }
}
