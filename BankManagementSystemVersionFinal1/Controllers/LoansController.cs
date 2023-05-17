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
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        public IActionResult Index()
        {
            var loans = _context.Loans
               .Include(l => l.Account)
               .ThenInclude(a =>a.AccountHolder)
               .ToList();

            return View(loans);
        }

        // GET: Loans/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Loans == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans
                .Include(t => t.Transactions)
                .Include(l => l.Account)
                .ThenInclude(a => a.AccountHolder)
                .FirstOrDefaultAsync(m => m.LoanId == id);
            if (loan == null)
            {
                return NotFound();
            }

            

            return View(loan);
        }

        // GET: Loans/Create
        public async Task<IActionResult> Create(int? id)
        {
            if (id == null || _context.Accounts == null)
            {
                return NotFound();
            }

            var account = await _context.Accounts
            .Include(a => a.AccountHolder)
            .FirstOrDefaultAsync(a => a.AccountId == id);
            if (account == null)
            {
                return NotFound();
            }

            var loan = new Loan { Account = account };
            return View(loan);

        }

        // POST: Loans/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int id, [Bind("Account,Amount")] Loan loan)
        {
            if (id != loan.Account.AccountId)
            {
                return NotFound();
            }

            loan.Account = await _context.Accounts
             .Include(a => a.AccountHolder)
             .FirstOrDefaultAsync(a => a.AccountId == id);



            loan.LoanStatus = Loan.LoanStatusEnum.AwaitingApproval;
            _context.Add(loan);
            await _context.SaveChangesAsync();

            var accountHolderId = loan.Account.AccountHolder.CustomerId;
            return RedirectToAction("Details", "Client", new { id = accountHolderId });



            // return View(loan);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Loans == null)
            {
                return NotFound();
            }

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return NotFound();
            }
            return View(loan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LoanId,InterstRate,StartingDate,EndingDate,LoanStatus")] Loan loan)
        {
            
            if (id != loan.LoanId)
            {
                return NotFound();
            }

            
                try
                {
                    var existingLoan = await _context.Loans.FindAsync(id);
                    if (existingLoan == null)
                    {
                        return NotFound();
                    }

                    existingLoan.LoanStatus = loan.LoanStatus;
                    existingLoan.InterstRate = loan.InterstRate;
                    existingLoan.StartingDate = loan.StartingDate;
                    existingLoan.EndingDate = loan.EndingDate;

                _context.Update(existingLoan);
                    await _context.SaveChangesAsync();
            }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.LoanId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            
            //  return View(loan);
        }




        private bool LoanExists(int id)
        {
          return (_context.Loans?.Any(e => e.LoanId == id)).GetValueOrDefault();
        }
    }
}
