using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BankManagementSystemVersionFinal1.Data;
using BankManagementSystemVersionFinal1.Models;

namespace BankManagementSystemVersionFinal1.Controllers
{
    public class CustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Customers
        public IActionResult Index()
        {
            var customers = _context.Customers
               .Include(c => c.BankBranch)
               //.ThenInclude(bb => bb.Manager)
               .ToList();

            return View(customers);
            
        }

        // GET: Customers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

           
            var customer = await _context.Customers
                .Include(m => m.BankBranch)
                .FirstOrDefaultAsync(m => m.CustomerId == id);



            var Accounts = await _context.Accounts
               .Where(t => t.AccountHolder.CustomerId == id)
               .ToListAsync();

            //var checkingaccount = new List<Account>();
            //var savingaccount = new List<Account>();
            //foreach (var account in Accounts)
            //{


            //    if (account is  CheckingAccount)
            //    {
            //        checkingaccount.Add(account);
            //    }
            //    else if (account is  SavingAccount)
            //    {
            //        savingaccount.Add(account);
            //    }
            //}


            //ViewBag.checkingaccount = checkingaccount;
            //ViewBag.savingaccount = savingaccount;
            ViewBag.account = Accounts;


            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // GET: Customers/Create
        public IActionResult Create()
        {
            ViewBag.BranchList = new SelectList(_context.BankBranches, "BranchId", "Name");
            return View();
        }

        // POST: Customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CustomerId,Cin,Name,Address,PhoneNumber,Mail,BranchId")] Customer customer)
        {
            
                _context.Add(customer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
         
        }

        // GET: Customers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers.Include(c => c.BankBranch).FirstOrDefaultAsync(c => c.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }
            ViewBag.BranchList = new SelectList(_context.BankBranches, "BranchId", "Name");

            return View(customer);
        }

        // POST: Customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CustomerId,Cin,Name,Address,PhoneNumber,Mail,BranchId")] Customer customer)
        {
            if (id != customer.CustomerId)
            {
                return NotFound();
            }

            
                var bankBranch = await _context.BankBranches.FindAsync(customer.BranchId);
                if (bankBranch == null)
                {
                    ModelState.AddModelError("BranchId", "Invalid Bank Branch");
                    ViewBag.BranchList = new SelectList(_context.BankBranches, "BranchId", "Name", customer.BranchId);
                    return View(customer);
                }

                customer.BankBranch = bankBranch;
                _context.Update(customer);
                await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));

            //return RedirectToAction(nameof(Details), new { id = customer.CustomerId });
        }

        // GET: Customers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Customers == null)
            {
                return NotFound();
            }

            var customer = await _context.Customers
                .FirstOrDefaultAsync(m => m.CustomerId == id);
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Customers == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customers'  is null.");
            }
            var customer = await _context.Customers.FindAsync(id);
            if (customer != null)
            {
                _context.Customers.Remove(customer);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CustomerExists(int id)
        {
          return (_context.Customers?.Any(e => e.CustomerId == id)).GetValueOrDefault();
        }


    }
}
